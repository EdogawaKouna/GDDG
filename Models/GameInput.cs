namespace GameDesignDocGenerator.Models;

public class GameInput
{
    public string GameType { get; set; } = "";
    public string Setting { get; set; } = "";
    public string CoreFeature { get; set; } = "";
    public string TargetPlatform { get; set; } = "";

    public override string ToString()
    {
        return $"游戏类型: {GameType}\n背景设定: {Setting}\n核心特色: {CoreFeature}\n目标平台: {TargetPlatform}";
    }
}