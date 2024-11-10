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
}