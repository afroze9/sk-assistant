using Elsa.Workflows;
using Elsa.Workflows.Activities;
using Elsa.Workflows.Contracts;


namespace Assistant.Server.Workflows;

public class TestWorkflow : IWorkflow
{
    public ValueTask BuildAsync(
        IWorkflowBuilder builder,
        CancellationToken cancellationToken = new CancellationToken())
    {
        builder
            .Root = new Sequence { Activities = { new HelloActivity(), } };

        return ValueTask.CompletedTask;
    }
}

public class HelloActivity : IActivity
{
    public ValueTask<bool> CanExecuteAsync(ActivityExecutionContext context) => new(true);

    public ValueTask ExecuteAsync(ActivityExecutionContext context)
    {
        Console.WriteLine("Hello");
        return ValueTask.CompletedTask;
    }

    public string Id { get; set; } = "HelloActivity";
    public string NodeId { get; set; } = "HelloNode";
    public string? Name { get; set; } = "Hello Activity";
    public string Type { get; set; } = "HelloType";
    public int Version { get; set; } = 1;
    public IDictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();
    public IDictionary<string, object> SyntheticProperties { get; set; } = new Dictionary<string, object>();
    public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
}