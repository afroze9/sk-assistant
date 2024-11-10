using Assistant.Desktop.Entities.Vector;

using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;

using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Assistant.Desktop.Services;

public class KnowledgeService(IVectorStore vectorStore, ITextEmbeddingGenerationService textEmbeddingGenerationService, QdrantClient client) : IKnowledgeService
{
    public async Task AddKnowledgeAsync(Knowledge knowledge, CancellationToken cancellationToken = default)
    {
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
            .Where(x => x.Score > 0.5)
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
}

public interface IKnowledgeService
{
    Task AddKnowledgeAsync(Knowledge knowledge, CancellationToken cancellationToken = default);
    Task<List<Knowledge>> GetKnowledgeListAsync(CancellationToken cancellationToken = default);
    Task<List<Knowledge>> SearchKnowledgeAsync(string query, CancellationToken cancellationToken = default);
}