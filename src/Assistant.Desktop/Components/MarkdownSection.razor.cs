using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Assistant.Desktop.Components;

public partial class MarkdownSection : FluentComponentBase
{
    private bool _markdownChanged = false;
    private string? _content;

    [Inject]
    protected IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets or sets the Markdown content 
    /// </summary>
    [Parameter]
    public string? Content
    {
        get => _content;
        set
        {
            if (_content is not null && !_content.Equals(value))
            {
                _markdownChanged = true;
            }
            _content = value;
        }
    }

    public MarkupString HtmlContent { get; private set; }

    protected override void OnInitialized()
    {
        if (Content is null)
        {
            throw new ArgumentException("You need to provide Content parameter");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender || _markdownChanged)
        {
            _markdownChanged = false;

            // create markup from markdown source
            HtmlContent = await MarkdownToMarkupStringAsync();
            StateHasChanged();
        }
    }

    /// <summary>
    /// Converts markdown, provided in Content or from markdown file stored as a static asset, to MarkupString for rendering.
    /// </summary>
    /// <returns>MarkupString</returns>
    private async Task<MarkupString> MarkdownToMarkupStringAsync()
    {
        string? markdown = Content;
        return ConvertToMarkupString(markdown);
    }
    private static MarkupString ConvertToMarkupString(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            var builder = new MarkdownPipelineBuilder()
                    .UseAdvancedExtensions()
                    .Use<MarkdownSectionPreCodeExtension>();

            var pipeline = builder.Build();

            // Convert markdown string to HTML
            var html = Markdown.ToHtml(value, pipeline);

            // Return sanitized HTML as a MarkupString that Blazor can render
            return new MarkupString(html);
        }

        return new MarkupString();
    }
}