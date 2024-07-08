using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using CompletionResponse = (string Completion, decimal);

namespace OpenAIDemo.Services;

public class OpenAIClient(HttpClient client)
{
    Lazy<string> _requestBody = new Lazy<string>(LoadRequestBody);
    Lazy<string> _requestBodyWithImage = new Lazy<string>(LoadRequestBodyWithImage);

    public async Task<CompletionResponse> SendCompletionRequestAsync(string system, string user, double temperature, bool jsonMode, byte[]? file = null)
    {
        string requestTemplate = file is null ? _requestBody.Value : _requestBodyWithImage.Value;
        string requestBody = requestTemplate.Replace("{{system}}", Encode(system))
                                            .Replace("{{user}}", Encode(user))
                                            .Replace("{{temperature}}", temperature.ToString("0.0", CultureInfo.InvariantCulture))
                                            .Replace("{{file}}", file is null ? string.Empty : Convert.ToBase64String(file))
                                            .Replace("{{response_format}}", jsonMode ? "json_object" : "text");

        StringContent content = new(requestBody, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync("chat/completions", content);

        return await ParseCompletionResponse(response);
    }

    static async Task<CompletionResponse> ParseCompletionResponse(HttpResponseMessage response)
    {
        string completion = await response.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(completion);

        try
        {
            string message = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "Errore";
            int promptTokens = doc.RootElement.GetProperty("usage").GetProperty("prompt_tokens").GetInt32();
            int completionTokens = doc.RootElement.GetProperty("usage").GetProperty("completion_tokens").GetInt32();

            return (message,
                Math.Max(0.001m,
                0.03m * completionTokens / 1000m +
                0.01m * promptTokens / 1000m));
        }
        catch (Exception ex)
        {
            throw new Exception(doc.RootElement.GetProperty("error").GetProperty("message").GetString(), ex);
        }
    }

    static string Encode(string input)
    {
        return JsonEncodedText.Encode(input).Value;
    }

    static string LoadRequestBody()
    {
        return LoadResource("OpenAIDemo.Services.OpenAIRequest.json");
    }

    static string LoadRequestBodyWithImage()
    {
        return LoadResource("OpenAIDemo.Services.OpenAIRequestWithImage.json");
    }

    static string LoadResource(string name)
    {
        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name) ??
                              throw new KeyNotFoundException($"Resource {name} not found");
        using StreamReader reader = new StreamReader(stream);
        string result = reader.ReadToEnd();
        return result;
    }
}
