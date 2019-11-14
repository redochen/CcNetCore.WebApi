namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 匹配类型
    /// </summary>
    public enum MatchType {
        Equal,
        NotEqual,
        Greater,
        GreaterOrEqual,
        Less,
        LessOrEqual,
        In,
        NotIn,
        Like,
        NotLike,
        BeginsWith,
        NotBeginsWith,
        EndsWith,
        NotEndsWith,
    }
}