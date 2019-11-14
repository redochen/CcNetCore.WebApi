namespace Dapper.Contrib.Extensions {
    /// <summary>
    /// 计数表
    /// </summary>
    public class CountTable {
        [Column ("count")]
        public long Count { get; set; }
    }
}