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
        /// <param name="state">包含方法所用数据的对象。</param>
        internal TimerSlotElement(Action<Object> callback,Object state)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            this._callback = callback;
            this._state = state;
            this._headers = new Stack<TimerSlotElementHeader>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所属时间槽。
        /// </summary>
        internal ICollection<TimerSlotElement> Slot
        {
            get;
            private set;
        }

        /// <summary>
        /// 所属时间轮。
        /// </summary>
        internal TimeWheel Owner
        {
            get;
            private set;
        }

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
            get { return this._headers.Count < 3; }
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

        /// <summary>
        /// 计时器是否启用。
        /// </summary>
        public Boolean IsEnable
        {
            get;
            internal set;
        }

        #region Callback

        private readonly Action<Object> _callback;
        /// <summary>
        /// 回调方法。
        /// </summary>
        internal Action<Object> Callback
        {
            get { return this._callback; }
        }

        #endregion

        #region State

        private readonly Object _state;
        /// <summary>
        /// 包含方法所用数据的对象。
        /// </summary>
        internal Object State
        {
            get { return this._state; }
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
        internal void AddHeader(Byte layer,Byte data)
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
        public void Unload()
        {
            lock (this)
            {
                if (IsEnable)
                {
                    lock(Owner._syncRoot)
                    {
                        Slot.Remove(this);
                        Owner._timerCounter--;
                    }
                    IsEnable = false;
                    Slot = null;
                    Owner = null;
                }
            }
        }

        /// <summary>
        /// 加载计时器。
        /// </summary>
        /// <param name="owner">所属时间轮。</param>
        /// <param name="slot">所属时间槽。</param>
        internal void Load(TimeWheel owner,ICollection<TimerSlotElement> slot)
        {
            Owner = owner;
            Slot = slot;
            IsEnable = true;
            lock(owner._syncRoot)
            {
                slot.Add(this);
                owner._timerCounter++;
            }
        }

        #endregion
    }
}
