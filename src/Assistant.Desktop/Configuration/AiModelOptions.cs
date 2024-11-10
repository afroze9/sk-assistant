namespace Assistant.Desktop.Configuration;

public class AiModelOptions
{
    public required string AudioToTextModel { get; set; }
    public required string ChatCompletionModel { get; set; }
    public required string EmbeddingGenerationModel { get; set; }
    public required string Key { get; set; }
}