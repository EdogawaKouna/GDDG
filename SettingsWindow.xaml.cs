using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameDesignDocGenerator.Services;

namespace GameDesignDocGenerator.UI;

public partial class SettingsWindow : Window
{
    private readonly ConfigManager _config;

    public SettingsWindow()
    {
        InitializeComponent();
        _config = new ConfigManager();

        // 如果已有 Key，自动填入
        var existingKey = _config.LoadApiKey();
        if (!string.IsNullOrWhiteSpace(existingKey))
        {
            txtApiKey.Text = existingKey;
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        var apiKey = txtApiKey.Text.Trim();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            txtError.Text = "❌ 请输入 API Key";
            return;
        }

        if (!apiKey.StartsWith("sk-"))
        {
            txtError.Text = "❌ Key 格式不正确，应以 sk- 开头";
            return;
        }

        _config.SaveApiKey(apiKey);
        DialogResult = true;
        Close();
    }
}
