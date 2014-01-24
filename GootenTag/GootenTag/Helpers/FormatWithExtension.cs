﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace GootenTag.Helpers
{
    /// <summary>
    /// Enables format strings to contain property names, and Javascript encodes strings.
    /// See http://james.newtonking.com/archive/2008/03/29/formatwith-2-0-string-formatting-with-named-variables.aspx
    /// </summary>
    /// <example>
    /// var nameString = "My name is {Name}".FormatWithForJavascript(new { Name = "David" });
    /// </example>
    public static class FormatWithExtension
    {
        public static string FormatWithForJavaScript(this string format, object source, IFormatProvider provider = null)
        {
            if (string.IsNullOrWhiteSpace(format)) throw new ArgumentNullException("format");

            var r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
                              RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            var values = new List<object>();
            var rewrittenFormat = r.Replace(format, delegate(Match m)
            {
                var startGroup = m.Groups["start"];
                var propertyGroup = m.Groups["property"];
                var formatGroup = m.Groups["format"];
                var endGroup = m.Groups["end"];

                values.Add((propertyGroup.Value == "0")
                  ? source
                  : DataBinder.Eval(source, propertyGroup.Value));

                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value
                  + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, encodeValues(values));
        }

        static object[] encodeValues(IEnumerable<object> values)
        {
            return values.Select(value => value is string ? HttpUtility.JavaScriptStringEncode((string)value) : value).ToArray();
        }
    }
}