using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomoGames.Utility
{
    /// <summary>
    /// 单层时间轮。
    /// </summary>
    public class SingleLayerTimeWheel: TimeWheel
    {
        #region Fields

        /// <summary>
        /// 一轮的时长。
        /// </summary>
        protected Int32 _durationOnFirstLayer;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.SingleLayerTimeWheel。
        /// </summary>
        public SingleLayerTimeWheel()
            :this(20)
        {
        }

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.SingleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnFirstLayer">第一层的刻度数量。</param>
        public SingleLayerTimeWheel(Int16 ticksOnFirstLayer)
            : this(ticksOnFirstLayer, 50)
        {

        }

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.SingleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnFirstLayer">第一层的刻度数量。</param>
        /// <param name="durationPerTick">刻度，单位毫秒。</param>
        public SingleLayerTimeWheel(Int16 ticksOnFirstLayer, Int32 durationPerTick)
            : base(durationPerTick, ticksOnFirstLayer)
        {
            Init(ticksOnFirstLayer);
        }

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.SingleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnFirstLayer">第一层的刻度数量。</param>
        /// <param name="durationPerTick">刻度，单位毫秒。</param>
        /// <param name="slotsProvider">创建用于存储计时器元素的列表。</param>
        public SingleLayerTimeWheel(Int16 ticksOnFirstLayer, Int32 durationPerTick, Func<Int16, ICollection<TimerSlotElement>[]> slotsProvider)
            : base(durationPerTick, ticksOnFirstLayer, slotsProvider)
        {
            Init(ticksOnFirstLayer);
        }

        #endregion

        #region Properties

        #region TicksOnFirstLayer

        private Int16 _ticksOnFirstLayer;

        public Int16 TicksOnFirstLayer
        {
            get { return this._ticksOnFirstLayer; }
        }

        #endregion

        #region CurrentTickOnFirstLayer

        /// <summary>
        /// 第一层的Tick值。
        /// </summary>
        private Int16 _currentTickOnFirstLayer;
        /// <summary>
        /// 获取第一层的Tick值。
        /// </summary>
        public Int16 CurrentTickOnFirstLayer
        {
            get { return this._currentTickOnFirstLayer; }
        }

        #endregion

        /// <summary>
        /// 获取时间轮层数，从零开始。
        /// </summary>
        public override Byte Layer
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取时间轮的当前时间。
        /// </summary>
        public override Int32 CurrentTime
        {
            get
            {
                return this._currentTickOnFirstLayer * DurationPerTick;
            }
        }

        #endregion

        #region Methods

        private void Init(Int16 ticksOnFirstLayer)
        {
            if (ticksOnFirstLayer <= 0)
            {
                throw new ArgumentException("ticksOnFirstLayer must bigger than 0.");
            }
            this._ticksOnFirstLayer = ticksOnFirstLayer;
            this._durationOnFirstLayer = ticksOnFirstLayer * DurationPerTick;
        }

        /// <summary>
        /// 创建头。
        /// </summary>
        /// <param name="totalTicks">总的Tick数。</param>
        /// <param name="element">时间槽元素。</param>
        /// <param name="tick">所属的时间槽。</param>
        protected override void CreateHeaders(Int32 totalTicks, TimerSlotElement element, out Int16 tick)
        {
            tick = (Int16)(totalTicks % this._ticksOnFirstLayer);
            element.AddHeader(Layer, tick);
        }

        /// <summary>
        /// 获取各层的Tick值，从最高层开始。
        /// </summary>
        /// <returns>各层的Tick值。</returns>
        protected override Int16[] GetTickOfEachLayer()
        {
            return new Int16[] { this._currentTickOnFirstLayer };
        }

        /// <summary>
        /// 前进指针。
        /// </summary>
        /// <returns>如果进位返回true，否则返回false。</returns>
        protected override Boolean OnTick()
        {
            if (++this._currentTickOnFirstLayer == this._ticksOnFirstLayer)
            {
                this._currentTickOnFirstLayer = 0;
                return true;
            }
            return false;
        }

        protected override void ResetCore()
        {
            this._currentTickOnFirstLayer = 0;
        }

        #endregion
    }
}
