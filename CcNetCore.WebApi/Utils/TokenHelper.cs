using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CcNetCore.Common;
using CcNetCore.Utils.Extensions;
using CcNetCore.Utils.Helpers;
using CcNetCore.Utils.Interfaces;

namespace CcNetCore.WebApi {
    /// <summary>
    /// 令牌服务接口
    /// </summary>
    public interface ITokenService {
        /// 获取令牌
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        string GetToken (string userName, int userId);

        /// <summary>
        /// 验证令牌的有效性
        /// </summary>
        /// <param name="token">要验证的令牌</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        ErrorCode CheckToken (string token, out int userId);
    }

    /// <summary>
    /// Token帮助类
    /// </summary>
    public class TokenHelper : ITokenService, ISingletonInstance {
        //自动装载属性（必须为public，否则自动装载失败）
        public IAppSettings _AppSettings { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TokenHelper () {
            Task.Run (() => LoadCache ());
        }

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public string GetToken (string userName, int userId) {
            if (!userName.IsValid ()) {
                return string.Empty;
            }

            foreach (var ctx in _UserTokens.Values) {
                if (ctx.UserName.Equals (userName) && !IsExpired (ctx)) {
                    return ctx.Token;
                }
            }

            var token = new TokenContext {
                Token = StringExtension.GetRandString (
                Constants.RAND_LEN_TOKEN, Constants.RAND_PREFIX_TOKEN),
                UserName = userName,
                UserID = userId,
                Time = DateTime.Now,
            };

            var hashCode = token.Token.GetHashCode ();
            _UserTokens.AddOrUpdate (hashCode, token, (k, v) => (v = token));

            Task.Run (() => SaveCache ());

            return token.Token;
        }

        /// <summary>
        /// 验证令牌的有效性
        /// </summary>
        /// <param name="token">要验证的令牌</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public ErrorCode CheckToken (string token, out int userId) {
            userId = 0;

            if (!token.IsValid ()) {
                return ErrorCode.InvalidToken;
            }

            var hashCode = token.GetHashCode ();
            if (!_UserTokens.TryGetValue (hashCode, out TokenContext ctx)) {
                return ErrorCode.TokenNotFound;
            }

            if (IsExpired (ctx)) {
                return ErrorCode.ExpiredToken;
            }

            userId = ctx.UserID;

            ctx.Time = DateTime.Now;
            _UserTokens.AddOrUpdate (hashCode, ctx, (k, v) => (v = ctx));

            Task.Run (() => SaveCache ());

            return ErrorCode.Success;
        }

        /// <summary>
        /// 令牌是否已过期
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsExpired (TokenContext token) {
            try {
                if (null == token) {
                    return true;
                }

                var minutes = token.Time.GetAbsDistance (DateTime.Now).TotalMinutes;
                return minutes >= _AppSettings.TokenExpireMinutes;
            } catch {
                return true;
            }
        }

        /// <summary>
        /// 加载缓存
        /// </summary>
        private void LoadCache () {
            var json = IoHelper.ReadFile (Constants.FILE_TOKEN_CACHE);

            var tokens = json.FromJson<List<TokenContext>> ();
            if (tokens.IsEmpty ()) {
                return;
            }

            tokens.ForEach (t => {
                if (t != null) {
                    _UserTokens.AddOrUpdate (t.Token.GetHashCode (), t, (k, v) => (v = t));
                }
            });
        }

        /// <summary>
        /// 保存缓存
        /// </summary>
        private void SaveCache () {
            var json = _UserTokens.Values.ToJson ();
            IoHelper.WriteFile (Constants.FILE_TOKEN_CACHE, json, append : false);
        }

        /// <summary>
        /// 用户令牌集合：Key-令牌Hash值
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <typeparam name="TokenContext"></typeparam>
        /// <returns></returns>
        private readonly ConcurrentDictionary<int, TokenContext> _UserTokens =
            new ConcurrentDictionary<int, TokenContext> ();

        /// <summary>
        /// 令牌上下文类
        /// </summary>
        internal class TokenContext {
            /// <summary>
            /// 令牌
            /// </summary>
            /// <value></value>
            public string Token { get; set; }

            /// <summary>
            /// 用户名
            /// </summary>
            /// <value></value>
            public string UserName { get; set; }

            /// <summary>
            /// 用户ID
            /// </summary>
            /// <value></value>
            public int UserID { get; set; }

            /// <summary>
            /// 刷新时间
            /// </summary>
            /// <value></value>
            public DateTime Time { get; set; }
        }
    }
}