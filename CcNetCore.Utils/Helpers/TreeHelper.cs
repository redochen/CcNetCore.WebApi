using System;
using System.Collections.Generic;
using System.Linq;

namespace CcNetCore.Utils.Helpers {
    /// <summary>
    /// 树节点接口
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public interface ITreeNode<TKey, TNode> where TNode : ITreeNode<TKey, TNode> {
        /// <summary>
        /// 键值
        /// </summary>
        /// <value></value>
        TKey Key { get; }

        /// <summary>
        /// 父节点键值
        /// </summary>
        /// <value></value>
        TKey ParentKey { get; }

        /// <summary>
        /// 子节点
        /// </summary>
        /// <value></value>
        List<TNode> Children { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public static class TreeItemExtension {
        /// <summary>
        /// 构建树
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="selectedKey"></param>
        /// <param name="selectNode"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <returns></returns>
        public static List<TNode> BuildTree<TKey, TNode> (this IEnumerable<TNode> nodes,
            TKey selectedKey, Func<TNode, TNode> selectNode)
        where TNode : ITreeNode<TKey, TNode> {
            var lookup = nodes.ToLookup (n => n.ParentKey);
            Func<TKey, List<TNode>> build = null;

            build = (key) => lookup[key].Select (n => {
                var item = selectNode (n);
                item.Children = build (n.Key);
                return item;
            }).ToList ();

            return build (selectedKey);
        }
    }
}