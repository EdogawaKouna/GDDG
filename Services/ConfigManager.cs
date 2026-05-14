using System.IO;
using System.Text.Json;

namespace GameDesignDocGenerator.Services;

public class ConfigManager
{
    private readonly string _configPath;

    public ConfigManager()
    {
        // 配置文件放在 exe 同级目录
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
    }

    public string? LoadApiKey()
    {
        try
        {
            if (!File.Exists(_configPath))
                return null;

            var json = File.ReadAllText(_configPath);
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("ApiKey").GetString();
        }
        catch
        {
            return null;
        }
    }

    public void SaveApiKey(string apiKey)
    {
        var json = JsonSerializer.Serialize(new { ApiKey = apiKey }, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(_configPath, json);
    }

    public bool HasApiKey()
    {
        var key = LoadApiKey();
        return !string.IsNullOrWhiteSpace(key) && key.StartsWith("sk-");
    }
}