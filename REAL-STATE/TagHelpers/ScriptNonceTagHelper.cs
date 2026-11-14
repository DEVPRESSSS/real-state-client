using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using REAL_STATE.Utilities;

namespace REAL_STATE.TagHelpers;

[HtmlTargetElement("script")]
public sealed class ScriptNonceTagHelper : TagHelper
{
    [ViewContext, HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = default!;
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext.HttpContext.Items.TryGetValue(SecurityHeadersMiddleware.NonceKey, out var nonce))
            output.Attributes.SetAttribute("nonce", nonce);
    }
}
