using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using GootenTag.Helpers;
using GootenTag.State;
using Newtonsoft.Json;

namespace GootenTag
{
    public interface IGoogleTagManager
    {
        /// <summary>
        /// Dynamic dictionary of Google Tag Manager variables and values.
        /// <example>
        /// GoogleTagManager.Current.DataLayer.page_type = "home";
        /// </example>
        /// </summary>
        dynamic DataLayer { get; }
        /// <summary>
        /// In case you want to add data layer variables using a dictionary interface.
        /// </summary>
        IDictionary<string, object> DataLayerDictionary { get; } 
    }

    /// <summary>
    /// Google Tag Manager Developer Guide: https://developers.google.com/tag-manager/devguide
    /// </summary>
    public class GoogleTagManager : IGoogleTagManager
    {
        const string ContainerSnippet =
@"<!-- Google Tag Manager -->
<noscript><iframe src=""//www.googletagmanager.com/ns.html?id={ContainerId}""
height=""0"" width=""0"" style=""display:none;visibility:hidden""></iframe></noscript>
<script>(function(w,d,s,l,i){{w[l]=w[l]||[];w[l].push({{'gtm.start':
new Date().getTime(),event:'gtm.js'}});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
}})(window,document,'script','dataLayer','{ContainerId}');</script>
<!-- End Google Tag Manager -->";

        const string DataLayerSnippet =
@"<script>
dataLayer = [{0}];
</script>
";

        const string StateKey = "GoogenTag.GoogleTagManager";

        /// <summary>
        /// Storage mechanism for state.  If unit testing, you may want to set this to <see cref="InMemoryStateStorage"/>.
        /// </summary>
        public static IStateStorage StateStorage = new HttpContextStateStorage();

        /// <summary>
        /// Your Google Tag Manager container ID.
        /// </summary>
        public static string ContainerId = "GTM-XXXX";

        /// <summary>
        /// Easy way to stop Google Tag Manager from rendering in your application.
        /// </summary>
        public static bool Enabled = true;

        /// <summary>
        /// Google Tag Manager state for the current request.
        /// </summary>
        public static IGoogleTagManager Current
        {
            get
            {
                var current = StateStorage.Get<IGoogleTagManager>(StateKey);
                if (current == null)
                {
                    current = new GoogleTagManager();
                    StateStorage.Set(StateKey, current);
                }
                return current;
            }
        }

        /// <summary>
        /// Renders the required HTML & JavaScript onto the page for Google Tag Manager.
        /// Call this method immediately after the opening <body> tag in your page.
        /// </summary>
        /// <returns></returns>
        public static IHtmlString Render()
        {
            if (!Enabled) 
                return new HtmlString(string.Empty);

            var containerSnippet = ContainerSnippet.FormatWithForJavaScript(new { ContainerId });

            if (!Current.DataLayerDictionary.Any())
                return new HtmlString(containerSnippet);

            var dataLayerSnippet = string.Format(DataLayerSnippet, JsonConvert.SerializeObject(Current.DataLayerDictionary,
                new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include
                    }));

            return new HtmlString(dataLayerSnippet + "\r\n" + containerSnippet);
        }

        /// <summary>
        /// Clears the current Google Tag Manager state.
        /// </summary>
        public static void Reset()
        {
            StateStorage.Remove(StateKey);
        }

        public GoogleTagManager()
        {
            DataLayer = new ExpandoObject();
        }

        /// <summary>
        /// Dynamic dictionary of Google Tag Manager variables and values.
        /// <example>
        /// GoogleTagManager.Current.DataLayer.page_type = "home";
        /// </example>
        /// </summary>
        public dynamic DataLayer { get; private set; }

        public IDictionary<string, object> DataLayerDictionary
        {
            get { return (IDictionary<string, object>) DataLayer; }
        }
    }
}