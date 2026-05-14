using System.Text;
using System.Text.Json;
using System.Net.Http;

namespace GameDesignDocGenerator.Services;

public class LlmService
{
    private readonly string _apiKey;
    private readonly string _apiUrl = "https://api.deepseek.com/chat/completions";
    private readonly HttpClient _httpClient;

    public LlmService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<string> ChatAsync(string systemPrompt, string userMessage)
    {
        var request = new
        {
            model = "deepseek-chat",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            temperature = 0.7,
            max_tokens = 4096
        };

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_apiUrl, content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var result = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return result ?? "";
        }
        catch (HttpRequestException ex)
        {
            return $"⚠️ API请求失败: {ex.Message}";
        }
        catch (JsonException ex)
        {
            return $"⚠️ 响应解析失败: {ex.Message}";
        }
    }
}