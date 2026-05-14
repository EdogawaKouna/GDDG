using System.Windows;
using System.Windows.Controls;
using GameDesignDocGenerator.Models;
using GameDesignDocGenerator.Services;
using GameDesignDocGenerator.Prompts;

namespace GameDesignDocGenerator.UI;

public partial class MainWindow : Window
{
    private readonly LlmService? _llm;
    private readonly DocGenerator? _generator;
    private readonly DocFormatter _formatter;
    private readonly ConfigManager _config;
    private GameDesignDoc? _currentDoc;
    private GameInput? _currentInput;

    public MainWindow()
    {
        InitializeComponent();
        _config = new ConfigManager();
        _formatter = new DocFormatter();

        // 检查 API Key
        if (!_config.HasApiKey())
        {
            // 弹出设置窗口
            var settingsWindow = new SettingsWindow();
            if (settingsWindow.ShowDialog() != true)
            {
                // 用户没设置就关闭
                Application.Current.Shutdown();
                return;
            }
        }

        // 用配置文件中的 Key 初始化
        var apiKey = _config.LoadApiKey();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            MessageBox.Show("API Key 配置失败，请重新启动。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
            return;
        }

        _llm = new LlmService(apiKey);
        _generator = new DocGenerator(_llm);

        txtStatus.Text = "✅ 已就绪，请输入游戏设定后点击生成";
    }

    private async void BtnGenerate_Click(object sender, RoutedEventArgs e)
    {
        if (_generator == null) return;

        // 收集输入
        var input = new GameInput
        {
            GameType = txtGameType.Text.Trim(),
            Setting = txtSetting.Text.Trim(),
            CoreFeature = txtCoreFeature.Text.Trim(),
            TargetPlatform = txtPlatform.Text.Trim()
        };

        if (string.IsNullOrWhiteSpace(input.GameType) || string.IsNullOrWhiteSpace(input.Setting))
        {
            MessageBox.Show("请至少填写游戏类型和背景设定！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _currentInput = input;

        // 禁用按钮，显示进度条
        btnGenerate.IsEnabled = false;
        btnIterate.IsEnabled = false;
        progressBar.Visibility = Visibility.Visible;
        progressBar.Value = 0;
        txtStatus.Text = "⏳ 正在生成...";

        try
        {
            var progress = new Progress<int>(value =>
            {
                Dispatcher.Invoke(() => progressBar.Value = value);
            });

            var doc = await Task.Run(() => _generator.GenerateFullDoc(input, progress));
            _currentDoc = doc;

            Dispatcher.Invoke(() =>
            {
                this.Title = $"🎮 {doc.GameTitle} - 游戏机制文案生成器";
                txtCoreRules.Text = doc.CoreRules;
                txtEconomy.Text = doc.EconomySystem;
                txtCombat.Text = doc.CombatFramework;
                txtProgression.Text = doc.ProgressionSystem;
                txtMonetization.Text = doc.Monetization;
                txtCompetitor.Text = doc.CompetitorReference;

                tabOutput.SelectedIndex = 0;
                txtStatus.Text = "✅ 生成完成！";
                progressBar.Visibility = Visibility.Collapsed;
                btnGenerate.IsEnabled = true;
                btnIterate.IsEnabled = true;
            });
        }
        catch (Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                txtStatus.Text = $"❌ 生成失败: {ex.Message}";
                progressBar.Visibility = Visibility.Collapsed;
                btnGenerate.IsEnabled = true;
            });
        }
    }

    private async void BtnIterate_Click(object sender, RoutedEventArgs e)
    {
        if (_generator == null) return;

        if (_currentDoc == null || _currentInput == null)
        {
            MessageBox.Show("请先生成文档！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var changeRequest = txtChange.Text.Trim();
        if (string.IsNullOrWhiteSpace(changeRequest))
        {
            MessageBox.Show("请输入修改要求！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var moduleMap = new Dictionary<int, string>
        {
            [0] = "coreRules",
            [1] = "economy",
            [2] = "combat",
            [3] = "progression",
            [4] = "monetization",
            [5] = "competitor"
        };

        var moduleKey = moduleMap[cmbModule.SelectedIndex];
        var moduleNames = new Dictionary<string, string>
        {
            ["coreRules"] = "核心玩法规则",
            ["economy"] = "经济系统",
            ["combat"] = "战斗/数值框架",
            ["progression"] = "成长系统",
            ["monetization"] = "商业化方案",
            ["competitor"] = "竞品分析"
        };

        btnIterate.IsEnabled = false;
        txtStatus.Text = $"⏳ 正在修改【{moduleNames[moduleKey]}】...";

        try
        {
            var updatedContent = await Task.Run(() => _generator.IterateModule(moduleKey, _currentDoc, changeRequest, _currentInput));

            Dispatcher.Invoke(() =>
            {
                var textBoxMap = new Dictionary<string, TextBlock>
                {
                    ["coreRules"] = txtCoreRules,
                    ["economy"] = txtEconomy,
                    ["combat"] = txtCombat,
                    ["progression"] = txtProgression,
                    ["monetization"] = txtMonetization,
                    ["competitor"] = txtCompetitor
                };

                switch (moduleKey)
                {
                    case "coreRules": _currentDoc.CoreRules = updatedContent; break;
                    case "economy": _currentDoc.EconomySystem = updatedContent; break;
                    case "combat": _currentDoc.CombatFramework = updatedContent; break;
                    case "progression": _currentDoc.ProgressionSystem = updatedContent; break;
                    case "monetization": _currentDoc.Monetization = updatedContent; break;
                    case "competitor": _currentDoc.CompetitorReference = updatedContent; break;
                }

                textBoxMap[moduleKey].Text = updatedContent;
                txtStatus.Text = "✅ 修改完成！";
                btnIterate.IsEnabled = true;
                txtChange.Clear();
            });
        }
        catch (Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                txtStatus.Text = $"❌ 修改失败: {ex.Message}";
                btnIterate.IsEnabled = true;
            });
        }
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        if (_currentDoc == null)
        {
            MessageBox.Show("请先生成文档！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            _formatter.ExportAll(_currentDoc);
            txtStatus.Text = "✅ 已导出 game_design_doc.md / .json / .txt";
            MessageBox.Show("导出完成！\n文件保存在程序运行目录下。", "导出成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"导出失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void txtCoreFeature_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}