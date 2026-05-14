using System.Text.Json;
using GameDesignDocGenerator.Models;
using System.IO;

namespace GameDesignDocGenerator.Services;

public class DocFormatter
{
    public void PrintSection(string title, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"\n{"".PadRight(60, '=')}");
        Console.WriteLine($"  {title}");
        Console.WriteLine($"{"".PadRight(60, '=')}");
        Console.ResetColor();
    }

    public void PrintColored(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    /// <summary>导出为 Markdown（增强版，增加版本号和目录）</summary>
    public void ExportToMarkdown(GameDesignDoc doc)
    {
        var md = $@"# {doc.GameTitle} - 游戏策划文档

> **版本**: {doc.Version} | **生成时间**: {doc.GeneratedAt:yyyy-MM-dd HH:mm:ss}

---

## 📑 目录

1. [核心玩法规则](#一核心玩法规则)
2. [经济系统](#二经济系统)
3. [战斗/数值框架](#三战斗数值框架)
4. [成长系统](#四成长系统)
5. [商业化方案](#五商业化方案)
6. [竞品分析](#六竞品分析)

---

## 一、核心玩法规则

{doc.CoreRules}

---

## 二、经济系统

{doc.EconomySystem}

---

## 三、战斗/数值框架

{doc.CombatFramework}

---

## 四、成长系统

{doc.ProgressionSystem}

---

## 五、商业化方案

{doc.Monetization}

---

## 六、竞品分析

{doc.CompetitorReference}

---

*文档由 AI 游戏策划生成器 v{doc.Version} 自动生成*
*生成时间: {doc.GeneratedAt:yyyy-MM-dd HH:mm:ss}*
*SDK: DeepSeek Chat | Model: deepseek-chat*
";

        File.WriteAllText("game_design_doc.md", md, System.Text.Encoding.UTF8);
    }

    /// <summary>导出为 JSON（结构化数据，可被程序读取）</summary>
    public void ExportToJson(GameDesignDoc doc)
    {
        var json = JsonSerializer.Serialize(new
        {
            title = doc.GameTitle,
            version = doc.Version,
            generatedAt = doc.GeneratedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            modules = new
            {
                coreRules = doc.CoreRules,
                economySystem = doc.EconomySystem,
                combatFramework = doc.CombatFramework,
                progressionSystem = doc.ProgressionSystem,
                monetization = doc.Monetization,
                competitorReference = doc.CompetitorReference
            }
        }, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        File.WriteAllText("game_design_doc.json", json, System.Text.Encoding.UTF8);
    }

    /// <summary>导出为纯文本（通用格式）</summary>
    public void ExportToText(GameDesignDoc doc)
    {
        var text = $@"========================================
{doc.GameTitle} - 游戏策划文档
版本: {doc.Version} | 生成时间: {doc.GeneratedAt:yyyy-MM-dd HH:mm:ss}
========================================

【核心玩法规则】
{doc.CoreRules}

【经济系统】
{doc.EconomySystem}

【战斗/数值框架】
{doc.CombatFramework}

【成长系统】
{doc.ProgressionSystem}

【商业化方案】
{doc.Monetization}

【竞品分析】
{doc.CompetitorReference}

========================================
文档由 AI 游戏策划生成器 v{doc.Version} 自动生成
========================================
";

        File.WriteAllText("game_design_doc.txt", text, System.Text.Encoding.UTF8);
    }

    /// <summary>导出所有格式</summary>
    public void ExportAll(GameDesignDoc doc)
    {
        ExportToMarkdown(doc);
        ExportToJson(doc);
        ExportToText(doc);
        Console.WriteLine("\n📁 已导出三种格式:");
        Console.WriteLine("   - game_design_doc.md   (Markdown)");
        Console.WriteLine("   - game_design_doc.json (JSON)");
        Console.WriteLine("   - game_design_doc.txt  (纯文本)");
    }
}