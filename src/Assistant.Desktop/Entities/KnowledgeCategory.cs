namespace Assistant.Desktop.Entities;

public class KnowledgeCategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
}