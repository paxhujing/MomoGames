using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomoGames.Utility
{
    /// <summary>
    /// 表示由两个实例组成的一对。
    /// </summary>
    /// <typeparam name="T1">类型参数。</typeparam>
    /// <typeparam name="T2">类型参数。</typeparam>
    public class Pair<T1, T2> : IEquatable<Pair<T1, T2>>
    {
        #region Fields

        private static readonly IEqualityComparer<T1> FirstComparer = EqualityComparer<T1>.Default;

        private static readonly IEqualityComparer<T2> SecondComparer = EqualityComparer<T2>.Default;

        #endregion

        #region Constructors

        public Pair(T1 first, T2 second)
        {
            this._first = first;
            this._second = second;
        }

        #endregion

        #region Properties

        #region First

        /// <summary>
        /// 第一个元素。
        /// </summary>
        private readonly T1 _first;
        /// <summary>
        /// 获取第一个元素。
        /// </summary>
        public T1 First
        {
            get { return this._first; }
        }

        #endregion

        #region Second

        /// <summary>
        /// 第二个元素。
        /// </summary>
        private readonly T2 _second;
        /// <summary>
        /// 获取第二个元素。
        /// </summary>
        public T2 Second
        {
            get { return this._second; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// 比较两个实例相等性。
        /// </summary>
        /// <param name="other">要比较的实例。</param>
        /// <returns>相等返回true，否则返回false。</returns>
        public Boolean Equals(Pair<T1, T2> other)
        {
            return other != null
                && FirstComparer.Equals(First, other.First)
                && SecondComparer.Equals(Second, other.Second);
        }

        /// <summary>
        /// 比较两个实例相等性。
        /// </summary>
        /// <param name="other">要比较的实例。</param>
        /// <returns>相等返回true，否则返回false。</returns>
        public override Boolean Equals(Object obj)
        {
            return Equals(obj as Pair<T1, T2>);
        }

        /// <summary>
        /// 获取实例的Hash值。
        /// </summary>
        /// <returns>Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return FirstComparer.GetHashCode(First) * 37 + SecondComparer.GetHashCode(Second);
        }

        #endregion
    }

    public static class Pair
    {
        public static Pair<T1, T2> Of<T1, T2>(T1 first, T2 second)
        {
            return new Pair<T1, T2>(first, second);
        }
    }
}
