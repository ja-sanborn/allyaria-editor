using Allyaria.Editor.Helpers;

namespace Allyaria.Tests.Unit.Helpers;

public sealed class EditorUtilsTests
{
    [Fact]
    public void BuildAriaAttributes_WhenLabelledByProvided_EmitsAriaLabelledBy()
    {
        // Arrange
        const string labelledBy = "heading1 heading2";

        // Act
        var attrs = EditorUtils.BuildAriaAttributes(labelledBy, overrideLabel: null, fallback: "ignored");

        // Assert (avoid WhichValue API)
        attrs.Should().ContainKey("aria-labelledby");
        attrs["aria-labelledby"].Should().Be(labelledBy);
        attrs.Should().NotContainKey("aria-label");
    }

    [Fact]
    public void BuildAriaAttributes_WhenNoLabelledByAndOverrideProvided_UsesTrimmedOverride()
    {
        // Arrange
        const string overrideLabel = "  Custom Label  ";
        const string expected = "Custom Label";

        // Act
        var attrs = EditorUtils.BuildAriaAttributes(labelledByResolved: "", overrideLabel, fallback: "ignored");

        // Assert (avoid WhichValue API)
        attrs.Should().ContainKey("aria-label");
        attrs["aria-label"].Should().Be(expected);
        attrs.Should().NotContainKey("aria-labelledby");
    }

    [Fact]
    public void BuildAriaAttributes_WhenNoLabelledByAndNoOverride_UsesFallback()
    {
        // Arrange
        const string fallback = "Editor content";

        // Act
        var attrs = EditorUtils.BuildAriaAttributes(labelledByResolved: "  ", overrideLabel: "   ", fallback);

        // Assert (avoid WhichValue API)
        attrs.Should().ContainKey("aria-label");
        attrs["aria-label"].Should().Be(fallback);
        attrs.Should().NotContainKey("aria-labelledby");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DefaultOrOverride_WhenNullOrWhitespace_ReturnsFallback(string? candidate)
    {
        // Arrange
        const string fallback = "fallback";

        // Act
        var result = EditorUtils.DefaultOrOverride(candidate, fallback);

        // Assert
        result.Should().Be(fallback);
    }

    [Fact]
    public void DefaultOrOverride_WhenNonWhitespace_TrimsAndReturnsValue()
    {
        // Arrange
        const string input = "  Hello  ";

        // Act
        var result = EditorUtils.DefaultOrOverride(input, fallback: "ignored");

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void Style_WhenValueProvided_BuildsCssDeclaration()
    {
        // Act
        var result = EditorUtils.Style("background-color", "#fff");

        // Assert
        result.Should().Be("background-color: #fff;");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Style_WhenNullOrWhitespace_ReturnsEmpty(string? value)
    {
        // Act
        var result = EditorUtils.Style("color", value);

        // Assert
        result.Should().BeEmpty();
    }
}
