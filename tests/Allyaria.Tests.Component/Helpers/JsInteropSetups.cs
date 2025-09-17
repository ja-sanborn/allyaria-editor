using Microsoft.JSInterop;

namespace Allyaria.Tests.Component.Helpers;

public static class JsInteropSetups
{
    public static IJSRuntime GetRequiredJsRuntime(this TestContext ctx)
    {
        var js = (IJSRuntime?)ctx.Services.GetService(typeof(IJSRuntime));

        return js ?? throw new InvalidOperationException("IJSRuntime is not registered in the TestContext services.");
    }

    public static void SetupDetectSystemTheme(this BunitJSInterop js, string theme = "light")
        => js.Setup<string>("Allyaria_Editor_detectSystemTheme", _ => true).SetResult(theme);

    public static void SetupGetInnerHtml(this BunitJSInterop js, string html)
        => js.Setup<string>("Allyaria_Editor_getInnerHtml", _ => true).SetResult(html);

    public static void SetupSanitizeLabelledBy(this BunitJSInterop js, string result = "")
        => js.Setup<string>("Allyaria_Editor_sanitizeLabelledBy", _ => true).SetResult(result);
}
