using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MomoGames.Utility
{
    /// <summary>
    /// 计时器槽元素。
    /// </summary>
    public struct TimerSlotElement
    {
        #region Fields

        private readonly Stack<UInt32> _headers;

        private readonly Action _callback;

        #endregion

        #region Constructors

        public TimerSlotElement(IEnumerable<UInt32> headers,Action callback)
        {
            this._headers = new Stack<UInt32>(headers);
            this._callback = callback;
        }

        #endregion

        /// <summary>
        /// 头部列表。头列表至少有且只有一个Tick类型的头部。
        /// --------------------------------------
        /// |类型8bit|保留8bit|  实际数值16bit   |
        /// --------------------------------------
        /// </summary>
        public UInt32 FirstHeader
        {
            get { return this._headers.Peek(); }
        }

        /// <summary>
        /// 回调方法。
        /// </summary>
        public Action Callback
        {
            get { return this._callback; }
        }
    }
}
