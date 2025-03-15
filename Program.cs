using CommandAI.Services;
using CommandAI.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CommandAI;

public class Program
{
    private static readonly HttpClient client = new() { Timeout = TimeSpan.FromMinutes(5) };
    private static Configuration? _config;

    public static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: cmai <path_to_directory>");
            return;
        }

        var configService = new ConfigurationService();
        _config = configService.LoadConfiguration();

        string directoryPath = args[0];
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("Directory not found.");
            return;
        }

        var gitIgnoreService = new GitIgnoreService();
        gitIgnoreService.LoadGitIgnore(directoryPath);

        var codeFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories)
            .Where(file => !gitIgnoreService.ShouldIgnore(file))
            .ToArray();

        foreach (string file in codeFiles)
        {
            Console.WriteLine($"Reviewing: {file}\n");
            string code = await File.ReadAllTextAsync(file);
            string review = await GetCodeReview(code);
            Console.WriteLine(review);
            Console.WriteLine("--------------------------------------\n");
        }
    }

    static async Task<string> GetCodeReview(string code)
    {
        if (_config == null)
            throw new InvalidOperationException("Configuration is not initialized.");

        var requestBody = new
        {
            model = _config.Model,  // Use configured model
            prompt = $"Review the following code...",
            stream = false
        };

        string jsonRequest = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await client.PostAsync(_config.Endpoint, content);  // Use configured endpoint and timeout
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseString);
            if (jsonResponse?.response == null)
            {
                Environment.Exit(1);
                return "No response from the AI model.";
            }
            return jsonResponse.response.ToString();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network error: {ex.Message} Exiting...");
            Environment.Exit(1);
            return "Failed to retrieve review due to network issue. Exiting...";
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON parsing error: {ex.Message} Exiting...");
            Environment.Exit(1);
            return "Failed to parse the AI model's response. Exiting...";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled error: {ex.Message} Exiting...");
            Environment.Exit(1);
            return "An unexpected error occurred during review. Exiting...";
        }
    }
}
