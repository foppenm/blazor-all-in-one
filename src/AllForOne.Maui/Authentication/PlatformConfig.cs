using AllForOne.Maui.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AllForOne.Authentication;

/// <summary>
/// Platform specific configuration.
/// </summary>
public class PlatformConfig
{
    /// <summary>
    /// Platform specific Redirect URI
    /// </summary>
    public string RedirectUri { get; }

    /// <summary>
    /// Platform specific parent window
    /// </summary>
    public object ParentWindow { get; set; }

    // private constructor to ensure singleton
    public PlatformConfig(IConfiguration configuration)
    {
        var b2cSettings = configuration.GetRequiredSection(B2cSettings.SectionName).Get<B2cSettings>();
        RedirectUri = $"msal{b2cSettings.ClientID}://auth";
    }
}
