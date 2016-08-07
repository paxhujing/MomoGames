using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomoGames.Utility
{
    /// <summary>
    /// 计时器槽元素头。
    /// </summary>
    /// --------------------------------------
    /// |类型8bit|保留8bit|  实际数值16bit   |
    /// --------------------------------------
    public struct TimerSlotElementHeader : IEquatable<TimerSlotElementHeader>
    {
        #region Fields

        private readonly Int32 _hash;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化结构 MomoGames.Utility.TimerSlotElementHeader。
        /// </summary>
        /// <param name="layer">层数。</param>
        /// <param name="data">数据。</param>
        public TimerSlotElementHeader(Byte layer,Byte data)
        {
            this._layer = layer;
            this._data = data;
            this._hash = layer.GetHashCode() * 37 + data.GetHashCode();
        }

        #endregion

        #region Properties

        #region Type

        /// <summary>
        /// 层数。
        /// </summary>
        private readonly Byte _layer;
        /// <summary>
        /// 获取层数。
        /// </summary>
        public Byte Layer
        {
            get { return this._layer; }
        }

        #endregion

        #region Data

        /// <summary>
        /// 头部数据。
        /// </summary>
        private readonly Byte _data;
        /// <summary>
        /// 获取头部值。
        /// </summary>
        public Byte Data
        {
            get { return this._data; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// 比较两个实例相等性。
        /// </summary>
        /// <param name="other">要比较的实例。</param>
        /// <returns>相等返回true，否则返回false。</returns>
        public override Boolean Equals(Object obj)
        {
            if (obj is TimerSlotElementHeader)
            {
                return Equals((TimerSlotElementHeader)obj);
            }
            return false;
        }

        /// <summary>
        /// 比较两个实例相等性。
        /// </summary>
        /// <param name="other">要比较的实例。</param>
        /// <returns>相等返回true，否则返回false。</returns>
        public Boolean Equals(TimerSlotElementHeader other)
        {
            return (Layer == other.Layer) && (Data == other.Data);
        }

        /// <summary>
        /// 获取实例的Hash值。
        /// </summary>
        /// <returns>Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return this._hash;
        }

        /// <summary>
        /// 将无符号32位数值转换为计时器槽元素头。
        /// </summary>
        /// <param name="value">无符号16位数值。</param>
        public static implicit operator TimerSlotElementHeader(UInt16 value)
        {
            return new TimerSlotElementHeader((Byte)(value >> 8), (Byte)value);
        }

        /// <summary>
        /// 将计时器槽元素头转换为无符号16位数值。
        /// </summary>
        /// <param name="header">计时器槽元素头。</param>
        public static explicit operator UInt16(TimerSlotElementHeader header)
        {
            UInt16 value = 0;
            value |= (UInt16)header.Data;
            value |= (UInt16)(header.Layer << 8);
            return value;
        }

        #endregion
    }
}
