using System.Globalization;
using Allyaria.Editor.Resources;

namespace Allyaria.Tests.Unit.Resources;

public sealed class EditorResourcesTests
{
    private static void WithUiCulture(string cultureName, Action action)
    {
        var original = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo(cultureName);
            action();
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }

    [Fact]
    public void EditorContentLabel_WhenResourceMissing_FallsBackToDefault()
        => WithUiCulture(
            "zz-ZZ", () =>
            {
                EditorResources.EditorContentLabel.Should().NotBeNullOrWhiteSpace()
                    .And.Be("Editor content");
            }
        );

    [Fact]
    public void EditorLabel_WhenResourceMissing_FallsBackToDefault()
        => WithUiCulture(
            "zz-ZZ", () =>
            {
                EditorResources.EditorLabel.Should().NotBeNullOrWhiteSpace()
                    .And.Be("Editor");
            }
        );

    [Fact]
    public void EditorStatusLabel_WhenResourceMissing_FallsBackToDefault()
        => WithUiCulture(
            "zz-ZZ", () =>
            {
                EditorResources.EditorStatusLabel.Should().NotBeNullOrWhiteSpace()
                    .And.Be("Editor status");
            }
        );

    [Fact]
    public void EditorToolbarLabel_WhenResourceMissing_FallsBackToDefault()
        => WithUiCulture(
            "zz-ZZ", () =>
            {
                EditorResources.EditorToolbarLabel.Should().NotBeNullOrWhiteSpace()
                    .And.Be("Editor toolbar");
            }
        );

    [Fact]
    public void AllLabels_AreNonEmpty_UnderNormalCulture()
        => WithUiCulture(
            "en-US", () =>
            {
                EditorResources.EditorLabel.Should().NotBeNullOrWhiteSpace();
                EditorResources.EditorContentLabel.Should().NotBeNullOrWhiteSpace();
                EditorResources.EditorStatusLabel.Should().NotBeNullOrWhiteSpace();
                EditorResources.EditorToolbarLabel.Should().NotBeNullOrWhiteSpace();
            }
        );
}
