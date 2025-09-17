using Allyaria.Editor.Abstractions.Models;
using Allyaria.Editor.Components;
using Allyaria.Editor.Helpers;
using Allyaria.Tests.Component.Helpers;

namespace Allyaria.Tests.Component.Components;

public class AllyariaContentTests : TestContext
{
    [Fact]
    public void Focus_And_Blur_Bubble_To_Parent_Handlers()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var focused = false;
        var blurred = false;

        var cut = RenderComponent<AllyariaContent>(ps =>
            {
                ps.Add(p => p.Text, string.Empty);
                ps.Add(p => p.AeLabels, new AeLabels());
                ps.Add(p => p.JsInterop, new EditorJsInterop(this.GetRequiredJsRuntime()));
                ps.Add(p => p.OnFocus, EventCallback.Factory.Create(this, () => focused = true));
                ps.Add(p => p.OnBlur, EventCallback.Factory.Create(this, () => blurred = true));
            }
        );

        var content = cut.Find("#ae-content");
        content.Focus();
        content.Blur();

        Assert.True(focused);
        Assert.True(blurred);
    }

    [Fact]
    public void Input_Updates_Text_And_Raises_TextChanged()
    {
        JSInterop.SetupSanitizeLabelledBy();
        JSInterop.SetupGetInnerHtml("<p>Hello</p>");

        string? updated = null;

        var cut = RenderComponent<AllyariaContent>(ps =>
            {
                ps.Add(p => p.Text, string.Empty);
                ps.Add(p => p.AeLabels, new AeLabels());
                ps.Add(p => p.JsInterop, new EditorJsInterop(this.GetRequiredJsRuntime()));
                ps.Add(p => p.TextChanged, value => { updated = value; });
            }
        );

        cut.Find("#ae-content").Input(
            new ChangeEventArgs
            {
                Value = ""
            }
        );

        Assert.Equal("<p>Hello</p>", updated);
    }

    [Fact]
    public void Shows_Placeholder_When_Text_Empty()
    {
        JSInterop.SetupSanitizeLabelledBy();

        var cut = RenderComponent<AllyariaContent>(ps =>
            {
                ps.Add(p => p.Text, string.Empty);
                ps.Add(p => p.Placeholder, "ToolbarType here...");
                ps.Add(p => p.ContentWrapperStyle, string.Empty);
                ps.Add(p => p.ContentStyle, string.Empty);
                ps.Add(p => p.PlaceholderStyle, string.Empty);
                ps.Add(p => p.AeLabels, new AeLabels());
                ps.Add(p => p.JsInterop, new EditorJsInterop(this.GetRequiredJsRuntime()));
            }
        );

        var placeholder = cut.Find("#ae-placeholder");
        Assert.Equal("ToolbarType here...", placeholder.TextContent);
        var content = cut.Find("#ae-content");
        Assert.Equal("ae-placeholder", content.GetAttribute("aria-describedby"));
    }
}
