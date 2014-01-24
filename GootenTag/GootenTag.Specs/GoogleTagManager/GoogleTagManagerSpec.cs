using System.Linq;
using GootenTag.State;
using Machine.Fakes;
using Machine.Specifications;

namespace GootenTag.Specs
{
    [Subject(typeof(GoogleTagManager))]
    public class When_rendering_google_tag_manager_and_it_is_disabled : WithSubject<GoogleTagManager>
    {
        It should_render_a_blank_html_string = () =>
            GoogleTagManager.Render().ToString().ShouldEqual(string.Empty);

        Establish context = () =>
            GoogleTagManager.Enabled = false;

        Cleanup after = () =>
            GoogleTagManager.Enabled = true;
    }

    [Subject(typeof(GoogleTagManager))]
    public class When_rendering_google_tag_manager_with_no_data_layer_variables : WithSubject<GoogleTagManager>
    {
        It should_only_render_the_container_snippet_with_container_id = () =>
            GoogleTagManager.Render().ToString().ShouldEqual(
@"<!-- Google Tag Manager -->
<noscript><iframe src=""//www.googletagmanager.com/ns.html?id=GTM-1234""
height=""0"" width=""0"" style=""display:none;visibility:hidden""></iframe></noscript>
<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window,document,'script','dataLayer','GTM-1234');</script>
<!-- End Google Tag Manager -->");

        Establish context = () =>
        {
            GoogleTagManager.StateStorage = new InMemoryStateStorage();
            GoogleTagManager.ContainerId = "GTM-1234";
        };
    }

    [Subject(typeof(GoogleTagManager))]
    public class When_rendering_google_tag_manager_with_data_layer_variables : WithSubject<GoogleTagManager>
    {
        It should_first_render_data_layer = () =>
            Result.ShouldStartWith(
@"<script>
dataLayer = [{""page_type"":""home"",""prodid"":""123456""}];
</script>
");

        It should_second_render_the_container_snippet = () =>
            Result.ShouldEndWith(
@"<!-- Google Tag Manager -->
<noscript><iframe src=""//www.googletagmanager.com/ns.html?id=GTM-1234""
height=""0"" width=""0"" style=""display:none;visibility:hidden""></iframe></noscript>
<script>(function(w,d,s,l,i){w[l]=w[l]||[];w[l].push({'gtm.start':
new Date().getTime(),event:'gtm.js'});var f=d.getElementsByTagName(s)[0],
j=d.createElement(s),dl=l!='dataLayer'?'&l='+l:'';j.async=true;j.src=
'//www.googletagmanager.com/gtm.js?id='+i+dl;f.parentNode.insertBefore(j,f);
})(window,document,'script','dataLayer','GTM-1234');</script>
<!-- End Google Tag Manager -->");

        Establish context = () =>
        {
            GoogleTagManager.StateStorage = new InMemoryStateStorage();
            GoogleTagManager.ContainerId = "GTM-1234";
            GoogleTagManager.Current.DataLayer.page_type = "home";
            GoogleTagManager.Current.DataLayer.prodid = "123456";
        };

        Because of = () =>
            Result = GoogleTagManager.Render().ToString();

        Cleanup after = () =>
            GoogleTagManager.Reset();

        static string Result;
    }

    [Subject(typeof(GoogleTagManager))]
    public class When_rendering_google_tag_manager_and_data_layer_variable_values_are_enumerable : WithSubject<GoogleTagManager>
    {
        It should_render_array_as_json_array = () =>
            Result.ShouldContain("\"array_of_numbers\":[1,2,3,4,5]");

        It should_render_linq_expression_as_json_array = () =>
            Result.ShouldContain("\"linq\":[\"first value\",\"second value\"]");

        Establish context = () =>
        {
            GoogleTagManager.StateStorage = new InMemoryStateStorage();
            GoogleTagManager.ContainerId = "GTM-1234";
            GoogleTagManager.Current.DataLayer.array_of_numbers = new[] { 1, 2, 3, 4, 5 };
            GoogleTagManager.Current.DataLayer.linq = new[] { "second value", "first value" }.Select(x => x).OrderBy(x => x);
        };

        Because of = () =>
            Result = GoogleTagManager.Render().ToString();

        Cleanup after = () =>
            GoogleTagManager.Reset();

        static string Result;
    }
}