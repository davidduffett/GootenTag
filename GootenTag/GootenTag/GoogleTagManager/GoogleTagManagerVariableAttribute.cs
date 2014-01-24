using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GootenTag
{
    /// <summary>
    /// Action filter attribute that can be used to specify variables to add to the Google Tag Manager data layer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class GoogleTagManagerVariableAttribute : ActionFilterAttribute
    {
        readonly string _variableName;
        readonly object _value;

        public GoogleTagManagerVariableAttribute(string variableName, object value)
        {
            if (string.IsNullOrWhiteSpace(variableName)) throw new ArgumentNullException("variableName");
            _variableName = variableName;
            _value = value;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ((IDictionary<string, Object>)GoogleTagManager.Current.DataLayer)[_variableName] = _value;
        }
    }
}