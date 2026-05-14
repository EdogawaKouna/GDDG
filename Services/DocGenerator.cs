using GameDesignDocGenerator.Models;
using GameDesignDocGenerator.Prompts;

namespace GameDesignDocGenerator.Services;

public class DocGenerator
{
    private readonly LlmService _llm;

    public DocGenerator(LlmService llm)
    {
        _llm = llm;
    }

    public async Task<GameDesignDoc> GenerateFullDoc(GameInput input, IProgress<int>? progress = null)
    {
        var doc = new GameDesignDoc();
        var step = 0;

        doc.GameTitle = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("title", input));
        progress?.Report(++step);

        doc.CoreRules = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("coreRules", input));
        progress?.Report(++step);

        doc.EconomySystem = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("economy", input));
        progress?.Report(++step);

        doc.CombatFramework = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("combat", input));
        progress?.Report(++step);

        doc.ProgressionSystem = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("progression", input));
        progress?.Report(++step);

        doc.Monetization = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("monetization", input));
        progress?.Report(++step);

        doc.CompetitorReference = await _llm.ChatAsync(PromptTemplates.SystemPrompt, PromptTemplates.GetModulePrompt("competitor", input));
        progress?.Report(++step);

        return doc;
    }

    public async Task<string> IterateModule(string moduleName, GameDesignDoc currentDoc, string changeRequest, GameInput input)
    {
        var currentContent = moduleName switch
        {
            "coreRules" => currentDoc.CoreRules,
            "economy" => currentDoc.EconomySystem,
            "combat" => currentDoc.CombatFramework,
            "progression" => currentDoc.ProgressionSystem,
            "monetization" => currentDoc.Monetization,
            "competitor" => currentDoc.CompetitorReference,
            _ => ""
        };

        var prompt = PromptTemplates.GetIterationPrompt(moduleName, currentContent, changeRequest, input);
        return await _llm.ChatAsync(PromptTemplates.SystemPrompt, prompt);
    }
}