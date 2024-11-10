using Microsoft.Extensions.VectorData;

namespace Assistant.Desktop.Entities.Vector;

public class Knowledge
{
    [VectorStoreRecordKey]
    public Guid KnowledgeId { get; set; } = Guid.NewGuid();
    
    [VectorStoreRecordData(IsFilterable = true, StoragePropertyName = "knowledge_category")]
    public string Category { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true, StoragePropertyName = "knowledge_description")]
    public string Description { get; set; }

    [VectorStoreRecordVector(3072, DistanceFunction.CosineSimilarity, IndexKind.Hnsw, StoragePropertyName = "knowledge_description_embedding")]
    public ReadOnlyMemory<float>? DescriptionEmbedding { get; set; }
}