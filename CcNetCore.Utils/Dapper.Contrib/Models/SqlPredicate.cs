using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CcNetCore.Utils.Extensions;

namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 获取匹配类型代理方法
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public delegate MatchType GetMatchTypeDelegate (PropertyInfo property);

    /// <summary>
    /// 过滤谓词类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlPredicate<T> {
        /// <summary>
        /// 查询对象
        /// </summary>
        /// <value></value>
        public T Condition { get; set; }

        /// <summary>
        /// SQL模板
        /// </summary>
        /// <value></value>
        public string SqlTemplate { get; set; }

        /// <summary>
        /// 逻辑操作类型
        /// </summary>
        /// <value></value>
        public string LogicType { get; set; }

        /// <summary>
        /// 匹配字段集合
        /// </summary>
        /// <value></value>
        public string[] MatchFields { get; set; }

        /// <summary>
        /// 如果MatchFields为空，是否自动识别匹配字段
        /// </summary>
        /// <value></value>
        public bool AutoMatch { get; set; } = true;

        /// <summary>
        /// 参数列表
        /// </summary>
        /// <value></value>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// 获取匹配类型的方法
        /// </summary>
        /// <value></value>
        public GetMatchTypeDelegate GetMatchType { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlTemplate">SQL模板</param>
        /// <param name="parameters">参数列表</param>
        public SqlPredicate (string sqlTemplate, Dictionary<string, object> parameters) {
            LogicType = string.Empty;
            SqlTemplate = sqlTemplate;
            Parameters = parameters;
            MatchFields = parameters.Keys.ToArray ();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="condition">查询条件实体</param>
        /// <param name="sqlTemplate">SQL模板</param>
        /// <param name="parameters">参数列表</param>
        public SqlPredicate (T condition, string sqlTemplate, Dictionary<string, object> parameters) {
            Condition = condition;
            LogicType = string.Empty;
            SqlTemplate = sqlTemplate;
            Parameters = parameters;
            MatchFields = parameters.Keys?.ToArray ();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="condition">查询条件实体</param>
        /// <param name="matchFields">匹配字段集合</param>
        public SqlPredicate (T condition, params string[] matchFields) {
            Condition = condition;
            LogicType = LogicTypes.AND;
            MatchFields = matchFields;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sqlTemplateOrLogicType">SQL模板或逻辑操作类型</param>
        /// <param name="matchFields">匹配字段集合</param>
        public SqlPredicate (string sqlTemplateOrLogicType, params string[] matchFields) {
            MatchFields = matchFields;

            SetSqlTemplateOrLogicType (sqlTemplateOrLogicType);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="condition">查询条件实体</param>
        /// <param name="sqlTemplateOrLogicType">SQL模板或逻辑操作类型</param>
        /// <param name="matchFields">匹配字段集合</param>
        public SqlPredicate (T condition, string sqlTemplateOrLogicType, params string[] matchFields) {
            Condition = condition;
            MatchFields = matchFields;

            SetSqlTemplateOrLogicType (sqlTemplateOrLogicType);
        }

        /// <summary>
        /// 设置SQL模板或逻辑操作类型
        /// </summary>
        /// <param name="sqlTemplateOrLogicType">SQL模板或逻辑操作类型</param>
        private void SetSqlTemplateOrLogicType (string sqlTemplateOrLogicType) {
            LogicType = GetLogicType (sqlTemplateOrLogicType);
            if (!LogicType.IsValid ()) {
                SqlTemplate = sqlTemplateOrLogicType;
            }
        }

        /// <summary>
        /// 获取逻辑操作类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string GetLogicType (string expression) {
            var opCode = _LogicTypes.FirstOrDefault (
                x => x.EqualsEx (expression, ignoreCase : true));
            return opCode;
        }

        /// <summary>
        /// 逻辑类型集合
        /// </summary>
        /// <value></value>
        private static readonly string[] _LogicTypes = new string[] { LogicTypes.AND, LogicTypes.OR };
    }
}