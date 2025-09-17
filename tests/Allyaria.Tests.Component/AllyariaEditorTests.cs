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
    public void BackgroundImage_Overrides_RegionBackgrounds_And_UsesOverlay50()
    {
        // Arrange
        SetupSanitizer();

        var theme = new AeTheme(
            AeThemeType.Dark,
            BackgroundImage: "paper.png"
        );

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Theme, theme));

        // Act
        var containerStyle = cut.Find("div.ae-editor").GetAttribute("style") ?? string.Empty;
        var toolbarStyle = cut.Find("#ae-toolbar").GetAttribute("style") ?? string.Empty;
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;
        var statusStyle = cut.Find("#ae-status").GetAttribute("style") ?? string.Empty;

        // Assert (image layer present with 50% overlay; region backgrounds ignored/transparent)
        Assert.Contains("background-image:", containerStyle);
        Assert.Contains("linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5))", containerStyle); // Dark overlay50
        Assert.Contains("url(\"paper.png\")", containerStyle);

        Assert.Contains("background-color: transparent", toolbarStyle);
        Assert.Contains("background-color: transparent", wrapperStyle);
        Assert.Contains("background-color: transparent", statusStyle);
    }

    [Fact]
    public void ContentBackground_Explicit_Overrides_Default()
    {
        // Arrange
        SetupSanitizer();

        var theme = new AeTheme(
            AeThemeType.Light,
            ContentBackground: "#ffeeee"
        );

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Theme, theme));

        // Act
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;

        // Assert
        Assert.Contains("background-color: #ffeeee", wrapperStyle);
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

    private void SetupSanitizer()
        => JSInterop.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(string.Empty);

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
    public void Theme_Light_Defaults_Applied()
    {
        // Arrange
        SetupSanitizer();

        var cut = RenderComponent<AllyariaEditor>(p => p
            .Add(x => x.Theme, new AeTheme(AeThemeType.Light))
        );

        // Act
        var container = cut.Find("div.ae-editor");
        var toolbar = cut.Find("#ae-toolbar");
        var content = cut.Find("#ae-content");
        var wrapper = cut.Find(".ae-content-wrapper");
        var status = cut.Find("#ae-status");

        var containerStyle = container.GetAttribute("style") ?? string.Empty;
        var toolbarStyle = toolbar.GetAttribute("style") ?? string.Empty;
        var contentStyle = content.GetAttribute("style") ?? string.Empty;
        var wrapperStyle = wrapper.GetAttribute("style") ?? string.Empty;
        var statusStyle = status.GetAttribute("style") ?? string.Empty;

        // Assert (Light defaults from GetDefaults)
        Assert.Contains("border: 1px solid #d0d7de", containerStyle); // border default
        Assert.Contains("background-color: #f6f8fa", toolbarStyle); // toolbar bg
        Assert.Contains("background-color: #ffffff", wrapperStyle); // content bg
        Assert.Contains("background-color: #f6f8fa", statusStyle); // status bg

        Assert.Contains("color: #24292f", toolbarStyle); // toolbar fg
        Assert.Contains("color: #24292f", contentStyle); // content fg
        Assert.Contains("caret-color: #24292f", contentStyle); // caret
        Assert.Contains("color: #24292f", statusStyle); // status fg
    }

    [Fact]
    public void Theme_Runtime_Switch_Updates_Immediately()
    {
        // Arrange
        SetupSanitizer();

        var cut = RenderComponent<AllyariaEditor>(p =>
            p.Add(x => x.Theme, new AeTheme(AeThemeType.Light))
        );

        // Verify initial (Light)
        var wrapper = cut.Find(".ae-content-wrapper");
        Assert.Contains("background-color: #ffffff", wrapper.GetAttribute("style") ?? string.Empty);

        // Act: switch to Dark at runtime
        cut.SetParametersAndRender(p =>
            p.Add(x => x.Theme, new AeTheme(AeThemeType.Dark))
        );

        // Assert: updates without the reload
        var wrapperStyleAfter = wrapper.GetAttribute("style") ?? string.Empty;
        Assert.Contains("background-color: #0f1115", wrapperStyleAfter);
    }

    [Fact]
    public void Theme_System_Resolves_To_HighContrast_When_SystemIsHC()
    {
        // Arrange
        SetupSanitizer();
        JSInterop.Setup<string>("Allyaria_Editor_detectSystemTheme", _ => true).SetResult("hc");

        var cut = RenderComponent<AllyariaEditor>(p =>
            p.Add(x => x.Theme, new AeTheme(AeThemeType.System))
        );

        // Act (first render triggers detection + StateHasChanged)
        var toolbarStyle = cut.Find("#ae-toolbar").GetAttribute("style") ?? string.Empty;
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;
        var contentStyle = cut.Find("#ae-content").GetAttribute("style") ?? string.Empty;

        // Assert (HighContrast defaults: black bg, white fg)
        Assert.Contains("background-color: #000000", toolbarStyle);
        Assert.Contains("background-color: #000000", wrapperStyle);
        Assert.Contains("color: #ffffff", contentStyle);
    }

    [Fact]
    public void Transparent_Removes_Backgrounds_Parent_ShowsThrough()
    {
        // Arrange
        SetupSanitizer();

        var theme = new AeTheme(
            AeThemeType.Light,
            BackgroundImage: "paper.png" // should be ignored when Transparent
        );

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Theme, theme));

        // Act
        var containerStyle = cut.Find("div.ae-editor").GetAttribute("style") ?? string.Empty;
        var toolbarStyle = cut.Find("#ae-toolbar").GetAttribute("style") ?? string.Empty;
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;
        var statusStyle = cut.Find("#ae-status").GetAttribute("style") ?? string.Empty;

        // Assert
        Assert.DoesNotContain("background-image:", containerStyle);
        Assert.Contains("background-color: transparent", toolbarStyle);
        Assert.Contains("background-color: transparent", wrapperStyle);
        Assert.Contains("background-color: transparent", statusStyle);
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
