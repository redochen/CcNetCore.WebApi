using System.Collections.Concurrent;
using System.Diagnostics;
using CcNetCore.Common;
using CcNetCore.Utils.Extensions;
using log4net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CcNetCore.WebApi.Filters {
    /// <summary>
    /// 日志过滤器
    /// </summary>
    public class LogFilter : IActionFilter {
        private ILog _Logger = null;
        private static ConcurrentDictionary<object, Stopwatch> _Timers =
            new ConcurrentDictionary<object, Stopwatch> ();

        /// <summary>
        /// 构造函数
        /// </summary>
        public LogFilter () {
            _Logger = LogManager.GetLogger (Startup.LoggerRepository.Name, this.GetType ());
        }

        /// <summary>
        /// Called before the action executes, after model binding is complete.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext.</param>
        public void OnActionExecuting (ActionExecutingContext context) {
            var sw = new Stopwatch ();
            sw.Start ();

            var id = StringExtension.GetRandString (8);
            var uri = context.HttpContext?.Request?.Path.Value;
            var args = context.ActionArguments?.ToJson ();

            _Logger.Debug ($"[{id}]开始请求:\"{uri}\",参数:{args}");

            _Timers.AddOrUpdate (id, sw, (k, v) => {
                v = sw;
                return v;
            });

            context.RouteData.Values.Add (Constants.ROUTE_DATA_KEY_REQ_ID, id);
        }

        /// <summary>
        /// Called after the action executes, before the action result.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext.</param>
        public void OnActionExecuted (ActionExecutedContext context) {
            var uri = context.HttpContext?.Request?.Path.Value;

            context.RouteData.Values.TryGetValue (
                Constants.ROUTE_DATA_KEY_REQ_ID, out object id);

            _Timers.TryRemove (id, out Stopwatch sw);

            if (sw != null) {
                sw.Stop ();
                _Logger.Debug ($"[{id}]结束请求:\"{uri}\",总耗时:{sw.Elapsed.TotalSeconds}秒");
            } else {
                _Logger.Debug ($"[{id}]结束请求:\"{uri}\"");
            }
        }
    }
}