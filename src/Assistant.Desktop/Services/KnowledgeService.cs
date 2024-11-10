using Assistant.Desktop.Data;
using Assistant.Desktop.Entities;
using Assistant.Desktop.Entities.Vector;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;

using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Assistant.Desktop.Services;

public class KnowledgeService(
    IVectorStore vectorStore, 
    ITextEmbeddingGenerationService textEmbeddingGenerationService, 
    QdrantClient client,
    ApplicationDbContext context) : IKnowledgeService
{
    public async Task SaveKnowledgeAsync(Knowledge knowledge, CancellationToken cancellationToken = default)
    {
        if (!await context.KnowledgeCategories.AnyAsync(
                x => x.Name == knowledge.Category,
                cancellationToken: cancellationToken))
        {
            await context.KnowledgeCategories.AddAsync(new KnowledgeCategory() { Name = knowledge.Category, }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken: cancellationToken);
        }
        
        IVectorStoreRecordCollection<ulong, Knowledge> collection = vectorStore.GetCollection<ulong, Knowledge>(nameof(Knowledge));
        await collection.CreateCollectionIfNotExistsAsync(cancellationToken);

        knowledge.DescriptionEmbedding =
            await textEmbeddingGenerationService.GenerateEmbeddingAsync(knowledge.Description, cancellationToken: cancellationToken);

        await collection.UpsertAsync(knowledge, cancellationToken: cancellationToken);
    }
    
    public async Task<List<Knowledge>> SearchKnowledgeAsync(string query, CancellationToken cancellationToken = default)
    {
        IVectorStoreRecordCollection<ulong, Knowledge> collection = vectorStore.GetCollection<ulong, Knowledge>(nameof(Knowledge));
        await collection.CreateCollectionIfNotExistsAsync(cancellationToken);
    
        var embedding = await textEmbeddingGenerationService.GenerateEmbeddingAsync(query, cancellationToken: cancellationToken);
        VectorSearchResults<Knowledge> searchResults =
            await collection.VectorizedSearchAsync(embedding, new VectorSearchOptions() { Top = 3 }, cancellationToken);

        return searchResults.Results
            .ToBlockingEnumerable(cancellationToken: cancellationToken)
            .Where(x => x.Score > 0.25)
            .Select(x => x.Record).ToList();
    }
    
    public async Task<List<Knowledge>> GetKnowledgeListAsync(CancellationToken cancellationToken = default)
    {
        IVectorStoreRecordCollection<ulong, Knowledge> collection = vectorStore.GetCollection<ulong, Knowledge>(nameof(Knowledge));
        await collection.CreateCollectionIfNotExistsAsync(cancellationToken);
        ScrollResponse items =
            await client.ScrollAsync(nameof(Knowledge), limit: 100, cancellationToken: cancellationToken);

        List<Knowledge> knowledgeList = new();
        foreach (RetrievedPoint point in items.Result)
        {
            PointId? id = point.Id;
            point.Payload.TryGetValue("knowledge_category", out Value category);
            point.Payload.TryGetValue("knowledge_description", out Value description);

            if (id == null || category == null || description == null)
            {
                continue;
            }

            Knowledge knowledge = new Knowledge
            {
                KnowledgeId = Guid.Parse(id.Uuid),
                Category = category.StringValue,
                Description = description.StringValue
            };
                
            knowledgeList.Add(knowledge);
        }

        return knowledgeList;
    }
    
    public async Task<List<string>> GetKnowledgeCategories()
    {
        return await context.KnowledgeCategories.Select(x => x.Name).ToListAsync();
    }
}

public interface IKnowledgeService
{
    Task SaveKnowledgeAsync(Knowledge knowledge, CancellationToken cancellationToken = default);
    Task<List<Knowledge>> GetKnowledgeListAsync(CancellationToken cancellationToken = default);
    Task<List<Knowledge>> SearchKnowledgeAsync(string query, CancellationToken cancellationToken = default);
    Task<List<string>> GetKnowledgeCategories();
}