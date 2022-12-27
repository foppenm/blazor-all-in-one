using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace AllForOne.Maui.Platforms.Android;

[Activity(Exported = true)]

// Todo: See if this hardcoded client id can be removed/configured
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
    DataHost = "auth",
    DataScheme = "msalcd01c71b-2101-41fa-9528-1695420fbbe1")]
public class MsalActivity : BrowserTabActivity
{
}