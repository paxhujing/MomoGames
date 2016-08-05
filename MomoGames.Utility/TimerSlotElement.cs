using System;
using System.Collections.Generic;
using System.Threading;

namespace MomoGames.Utility
{
    /// <summary>
    /// 计时器槽元素。
    /// </summary>
    public class TimerSlotElement
    {
        #region Fields

        private readonly Stack<TimerSlotElementHeader> _headers;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化结构 MomoGames.Utility.TimerSlotElement。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        internal TimerSlotElement(Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            this._callback = callback;
            this._headers = new Stack<TimerSlotElementHeader>();
            IsEnable = true;
        }

        #endregion

        #region Properties

        #region Slot

        /// <summary>
        /// 所属时间槽。
        /// </summary>
        internal ICollection<TimerSlotElement> Slot
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 头数量。
        /// </summary>
        internal Int32 HeaderCount
        {
            get { return this._headers.Count; }
        }

        /// <summary>
        /// 是否具有被执行的候选资格。
        /// </summary>
        internal Boolean IsCandidature
        {
            get { return this._headers.Count < 2; }
        }

        /// <summary>
        /// 当前所在的头。
        /// </summary>
        internal TimerSlotElementHeader CurrentHeader
        {
            get
            {
                return this._headers.Peek();
            }
        }

        #region Status
        /// <summary>
        /// 计时器是否启用。
        /// </summary>
        public Boolean IsEnable
        {
            get;
            internal set;
        }

        #endregion

        #region Callback

        private readonly Action _callback;
        /// <summary>
        /// 回调方法。
        /// </summary>
        public Action Callback
        {
            get { return this._callback; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// 获取实例的Hash值。
        /// </summary>
        /// <returns>Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return this._callback.GetHashCode() * 37 + this._headers.GetHashCode();
        }

        /// <summary>
        /// 添加一个头。
        /// </summary>
        /// <param name="layer">层数。</param>
        /// <param name="data">数据。</param>
        internal void AddHeader(Byte layer,Int16 data)
        {
            if (data == 0) return;
            TimerSlotElementHeader header = new TimerSlotElementHeader(layer, data);
            this._headers.Push(header);
        }

        /// <summary>
        /// 移除当前的头。
        /// </summary>
        internal void RemoveCurrentHeader()
        {
            this._headers.Pop();
        }

        /// <summary>
        /// 卸载计时器。
        /// </summary>
        /// <returns>成功返回true；失败返回false。</returns>
        public Boolean Unload()
        {
            lock (this)
            {
                if (IsEnable)
                {
                    IsEnable = false;
                    Slot.Remove(this);
                    Slot = null;
                    return true;
                }
                return false;
            }
        }

        #endregion
    }
}
