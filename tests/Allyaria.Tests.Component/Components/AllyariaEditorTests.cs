using Allyaria.Editor.Abstractions.Models;
using Allyaria.Editor.Abstractions.Types;
using Allyaria.Editor.Components;
using Allyaria.Tests.Component.Helpers;

namespace Allyaria.Tests.Component.Components;

public sealed class AllyariaEditorTests : TestContext
{
    [Fact]
    public void Aria_Name_Defaults_From_Resx_When_No_Overrides()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>();
        var content = cut.Find("#ae-content");

        // Should have aria-label with default
        content.GetAttribute("aria-label").Should().Be("Editor content");
    }

    [Fact]
    public void BackgroundImage_Overrides_RegionBackgrounds_And_UsesOverlay50()
    {
        // Arrange
        JSInterop.SetupSanitizeLabelledBy();

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
        containerStyle.Should().Contain("background-image:");
        containerStyle.Should().Contain("linear-gradient(rgba(0,0,0,0.5), rgba(0,0,0,0.5))"); // Dark overlay50
        containerStyle.Should().Contain("url(\"paper.png\")");

        toolbarStyle.Should().Contain("background-color: transparent");
        wrapperStyle.Should().Contain("background-color: transparent");
        statusStyle.Should().Contain("background-color: transparent");
    }

    [Fact]
    public void ContentBackground_Explicit_Overrides_Default()
    {
        // Arrange
        JSInterop.SetupSanitizeLabelledBy();

        var theme = new AeTheme(
            AeThemeType.Light,
            ContentBackground: "#ffeeee"
        );

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Theme, theme));

        // Act
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;

        // Assert
        wrapperStyle.Should().Contain("background-color: #ffeeee");
    }

    [Fact]
    public void Default_Size_Is_400x300()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>();
        var container = cut.Find("div.ae-editor");

        var style = container.GetAttribute("style") ?? string.Empty;
        style.Should().Contain("width: 400px");
        style.Should().Contain("height: 300px");
    }

    [Fact]
    public void HeightZero_Fills_Parent()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Height, 0));
        var container = cut.Find("div.ae-editor");

        var style = container.GetAttribute("style") ?? string.Empty;
        style.Should().Contain("height: 100%");
    }

    [Fact]
    public void Invalid_LabelledBy_Falls_Back_To_Default_Label()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(
                x => x.AeLabels, new AeLabels(ContentLabelledById: "badId")
            )
        );

        var content = cut.Find("#ae-content");
        content.GetAttribute("aria-label").Should().Be("Editor content");
    }

    [Fact]
    public void Programmatic_Update_Displays_Text()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Text, "Initial"));
        cut.SetParametersAndRender(p => p.Add(x => x.Text, "Updated"));

        var content = cut.Find("#ae-content");
        content.InnerHtml.Should().Contain("Updated");
    }

    [Fact]
    public void Role_Is_Textbox()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>();
        var content = cut.Find("#ae-content");

        content.GetAttribute("role").Should().Be("textbox");
    }

    [Fact]
    public void Stable_Region_IDs_Render()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>();
        cut.Find("#ae-toolbar").Should().NotBeNull();
        cut.Find("#ae-content").Should().NotBeNull();
        cut.Find("#ae-status").Should().NotBeNull();
    }

    [Fact]
    public void Theme_Light_Defaults_Applied()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p => p
            .Add(x => x.Theme, new AeTheme(AeThemeType.Light))
        );

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

        // Light defaults from GetDefaults
        containerStyle.Should().Contain("border: 1px solid #d0d7de");
        toolbarStyle.Should().Contain("background-color: #f6f8fa");
        wrapperStyle.Should().Contain("background-color: #ffffff");
        statusStyle.Should().Contain("background-color: #f6f8fa");

        toolbarStyle.Should().Contain("color: #24292f");
        contentStyle.Should().Contain("color: #24292f");
        contentStyle.Should().Contain("caret-color: #24292f");
        statusStyle.Should().Contain("color: #24292f");
    }

    [Fact]
    public void Theme_Runtime_Switch_Updates_Immediately()
    {
        // Arrange
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p =>
            p.Add(x => x.Theme, new AeTheme(AeThemeType.Light))
        );

        // Verify initial (Light)
        var wrapper = cut.Find(".ae-content-wrapper");
        wrapper.GetAttribute("style")!.Should().Contain("background-color: #ffffff");

        // Act: switch to Dark at runtime
        cut.SetParametersAndRender(p =>
            p.Add(x => x.Theme, new AeTheme(AeThemeType.Dark))
        );

        // Assert: updates without reload
        var wrapperStyleAfter = wrapper.GetAttribute("style") ?? string.Empty;
        wrapperStyleAfter.Should().Contain("background-color: #0f1115");
    }

    [Fact]
    public void Theme_System_Resolves_To_HighContrast_When_SystemIsHC()
    {
        // Arrange
        JSInterop.SetupSanitizeLabelledBy();
        JSInterop.SetupDetectSystemTheme("hc");

        var cut = RenderComponent<AllyariaEditor>(p =>
            p.Add(x => x.Theme, new AeTheme(AeThemeType.System))
        );

        // Act (first render triggers detection + StateHasChanged)
        var toolbarStyle = cut.Find("#ae-toolbar").GetAttribute("style") ?? string.Empty;
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;
        var contentStyle = cut.Find("#ae-content").GetAttribute("style") ?? string.Empty;

        // Assert (HighContrast defaults: black bg, white fg)
        toolbarStyle.Should().Contain("background-color: #000000");
        wrapperStyle.Should().Contain("background-color: #000000");
        contentStyle.Should().Contain("color: #ffffff");
    }

    [Fact]
    public void Transparent_Removes_Backgrounds_Parent_ShowsThrough()
    {
        // Arrange
        JSInterop.SetupSanitizeLabelledBy();

        var theme = new AeTheme(
            AeThemeType.Light,
            true,
            BackgroundImage: "paper.png" // should be ignored when Transparent
        );

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Theme, theme));

        // Act
        var containerStyle = cut.Find("div.ae-editor").GetAttribute("style") ?? string.Empty;
        var toolbarStyle = cut.Find("#ae-toolbar").GetAttribute("style") ?? string.Empty;
        var wrapperStyle = cut.Find(".ae-content-wrapper").GetAttribute("style") ?? string.Empty;
        var statusStyle = cut.Find("#ae-status").GetAttribute("style") ?? string.Empty;

        // Assert
        containerStyle.Should().NotContain("background-image:");
        toolbarStyle.Should().Contain("background-color: transparent");
        wrapperStyle.Should().Contain("background-color: transparent");
        statusStyle.Should().Contain("background-color: transparent");
    }

    [Fact]
    public void Valid_AriaLabelledBy_Takes_Precedence()
    {
        // Simulate sanitizer returning a valid id, regardless of input
        JSInterop.SetupSanitizeLabelledBy("heading1");

        var cut = RenderComponent<AllyariaEditor>(p =>
            p.Add(x => x.AeLabels, new AeLabels(ContentLabelledById: "heading1"))
        );

        var content = cut.Find("#ae-content");
        content.GetAttribute("aria-labelledby").Should().Be("heading1");
        content.GetAttribute("aria-label").Should().BeNull();
    }

    [Fact]
    public void Whitespace_Override_Falls_Back_To_Default()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.AeLabels, new AeLabels("   ")));
        var container = cut.Find("div.ae-editor");

        container.GetAttribute("aria-label").Should().Be("Editor");
    }

    [Fact]
    public void WidthZero_Fills_Parent()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaEditor>(p => p.Add(x => x.Width, 0));
        var container = cut.Find("div.ae-editor");

        var style = container.GetAttribute("style") ?? string.Empty;
        style.Should().Contain("width: 100%");
    }
}
