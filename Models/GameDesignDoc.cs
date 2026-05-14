namespace GameDesignDocGenerator.Models;

public class GameDesignDoc
{
    public string GameTitle { get; set; } = "";
    public string CoreRules { get; set; } = "";
    public string EconomySystem { get; set; } = "";
    public string CombatFramework { get; set; } = "";
    public string ProgressionSystem { get; set; } = "";
    public string Monetization { get; set; } = "";
    public string CompetitorReference { get; set; } = "";

    // 增加：用于导出
    public string Version { get; set; } = "1.0";
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}