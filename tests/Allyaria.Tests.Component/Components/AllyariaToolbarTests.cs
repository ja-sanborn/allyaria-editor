using Allyaria.Editor.Abstractions.Models;
using Allyaria.Editor.Abstractions.Types;
using Allyaria.Editor.Components;
using Allyaria.Editor.Helpers;
using Allyaria.Tests.Component.Helpers;

namespace Allyaria.Tests.Component.Components;

public sealed class AllyariaToolbarTests : TestContext
{
    [Fact]
    public void Renders_Status_With_Default_Aria_Label()
    {
        JSInterop.SetupSanitizeLabelledBy(); // force aria-label fallback

        var cut = RenderComponent<AllyariaToolbar>(ps =>
            {
                ps.Add(p => p.ToolbarType, AeToolbarType.Status);
                ps.Add(p => p.AeLabels, new AeLabels());
                ps.Add(p => p.ToolbarStyle, string.Empty);
                ps.Add(p => p.StatusStyle, "background-color:blue;");
                ps.Add(p => p.JsInterop, new EditorJsInterop(this.GetRequiredJsRuntime()));
            }
        );

        var status = cut.Find("#ae-status");
        status.ClassName.Should().Be("ae-status");
        status.GetAttribute("aria-label").Should().Be("Editor status");
    }

    [Fact]
    public void Renders_Toolbar_With_Default_Aria_Label()
    {
        JSInterop.SetupSanitizeLabelledBy(); // force aria-label fallback

        var cut = RenderComponent<AllyariaToolbar>(ps =>
            {
                ps.Add(p => p.ToolbarType, AeToolbarType.Toolbar);
                ps.Add(p => p.AeLabels, new AeLabels());
                ps.Add(p => p.ToolbarStyle, "background-color:red;");
                ps.Add(p => p.StatusStyle, string.Empty);
                ps.Add(p => p.JsInterop, new EditorJsInterop(this.GetRequiredJsRuntime()));
            }
        );

        var toolbar = cut.Find("#ae-toolbar");
        toolbar.ClassName.Should().Be("ae-toolbar");
        toolbar.GetAttribute("aria-label").Should().Be("Editor toolbar");
    }
}
