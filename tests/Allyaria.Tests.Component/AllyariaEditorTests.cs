using Allyaria.Editor.Abstractions;
using Allyaria.Editor.Components;

namespace Allyaria.Tests.Component;

public class EditorContainerTests : TestContext
{
    [Fact]
    public void Aria_Name_Defaults_From_Resx_When_No_Overrides()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>();
        var content = cut.Find("#ae-content");

        // Should have aria-label with default
        Assert.Equal("Editor content", content.GetAttribute("aria-label"));
    }

    [Fact]
    public void Default_Size_Is_400x300()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>();
        var container = cut.Find("div.ae-editor");

        var style = container.GetAttribute("style");
        Assert.Contains("width: 400px", style);
        Assert.Contains("height: 300px", style);
    }

    [Fact]
    public async Task Focus_And_Blur_Events_Fire()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var focused = false;
        var blurred = false;

        var cut = RenderComponent<AllyariaEditor>(p => p
            .Add(x => x.OnFocus, EventCallback.Factory.Create(this, () => focused = true))
            .Add(x => x.OnBlur, EventCallback.Factory.Create(this, () => blurred = true))
        );

        var content = cut.Find("#ae-content");

        await content.TriggerEventAsync("onfocus", new FocusEventArgs());
        await content.TriggerEventAsync("onblur", new FocusEventArgs());

        Assert.True(focused);
        Assert.True(blurred);
    }

    [Fact]
    public void HeightZero_Fills_Parent()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Height, 0));
        var container = cut.Find("div.ae-editor");

        var style = container.GetAttribute("style");
        Assert.Contains("height: 100%", style);
    }

    [Fact]
    public void Invalid_LabelledBy_Falls_Back_To_Default_Label()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(
                x => x.AeLabels, new AeLabels(ContentLabelledById: "badId")
            )
        );

        var content = cut.Find("#ae-content");

        Assert.Equal("Editor content", content.GetAttribute("aria-label"));
    }

    [Fact]
    public void Placeholder_Shown_And_Announced_When_Text_Empty()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>(p => p
            .Add(x => x.Text, string.Empty)
            .Add(x => x.Placeholder, "Start typing...")
        );

        var placeholder = cut.Find("#ae-placeholder");
        Assert.Equal("Start typing...", placeholder.TextContent.Trim());

        var content = cut.Find("#ae-content");
        Assert.Contains("ae-placeholder", content.GetAttribute("aria-describedby"));
    }

    [Fact]
    public void Programmatic_Update_Displays_Text()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Text, "Initial"));
        cut.SetParametersAndRender(p => p.Add(x => x.Text, "Updated"));

        var content = cut.Find("#ae-content");
        Assert.Contains("Updated", content.InnerHtml);
    }

    [Fact]
    public void Role_Is_Textbox()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>();
        var content = cut.Find("#ae-content");

        Assert.Equal("textbox", content.GetAttribute("role"));
    }

    [Fact]
    public void Stable_Region_IDs_Render()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>();
        Assert.NotNull(cut.Find("#ae-toolbar"));
        Assert.NotNull(cut.Find("#ae-content"));
        Assert.NotNull(cut.Find("#ae-status"));
    }

    [Fact]
    public async Task Typing_Updates_Binding_And_Fires_TextChanged()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        string? updated = null;

        var cut = RenderComponent<AllyariaEditor>(parameters => parameters
            .Add(p => p.Text, "Hello")
            .Add(p => p.TextChanged, v => updated = v)
        );

        // Simulate JS returning edited innerHTML
        JSInterop.Setup<string>("Allyaria_Editor_getInnerHtml", _ => true).SetResult("Hello world");

        var content = cut.Find("#ae-content");
        await content.TriggerEventAsync("oninput", new ChangeEventArgs());

        Assert.Equal("Hello world", updated);

        // DOM should reflect updated text
        Assert.Contains("Hello world", content.InnerHtml);
    }

    [Fact]
    public void Valid_AriaLabelledBy_Takes_Precedence()
    {
        // Simulate sanitizer returning a valid id, regardless of input
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true)
            .SetResult("heading1");

        var cut = RenderComponent<AllyariaEditor>(p =>
            p.Add(x => x.AeLabels, new AeLabels(ContentLabelledById: "heading1"))
        );

        var content = cut.Find("#ae-content");
        Assert.Equal("heading1", content.GetAttribute("aria-labelledby"));
        Assert.Null(content.GetAttribute("aria-label"));
    }

    [Fact]
    public void Whitespace_Override_Falls_Back_To_Default()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.AeLabels, new AeLabels("   ")));
        var container = cut.Find("div.ae-editor");

        Assert.Equal("Editor", container.GetAttribute("aria-label"));
    }

    [Fact]
    public void WidthZero_Fills_Parent()
    {
        JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Width, 0));
        var container = cut.Find("div.ae-editor");

        var style = container.GetAttribute("style");
        Assert.Contains("width: 100%", style);
    }
}
