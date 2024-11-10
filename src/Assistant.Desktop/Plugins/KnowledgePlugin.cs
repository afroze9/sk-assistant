using System.ComponentModel;

using Assistant.Desktop.Entities.Vector;
using Assistant.Desktop.Services;

using Microsoft.SemanticKernel;

namespace Assistant.Desktop.Plugins;

public class KnowledgePlugin(IKnowledgeService knowledgeService)
{
    [KernelFunction(name:"search_knowledge")]
    [Description("Search for saved knowledge")]
    [return:Description("List of saved knowledge")]
    public async Task<List<Knowledge>> SearchKnowledgeAsync(string query)
    {
        // Search knowledge
        return await knowledgeService.SearchKnowledgeAsync(query);
    }

    [KernelFunction(name:"get_knowledge_categories")]
    [Description("Gets a list of knowledgebase categories")]
    [return:Description("List of knowledgebase categories")]
    public async Task<List<string>> GetKnowledgeCategories()
    {
        return await knowledgeService.GetKnowledgeCategories();
    }

    [KernelFunction(name:"save_knowledge")]
    [Description("Save knowledge for future use")]
    public async Task SaveKnowledgeAsync(string category, string description)
    {
        var knowledge = new Knowledge() { Category = category, Description = description, };
        await knowledgeService.SaveKnowledgeAsync(knowledge);
    }
}