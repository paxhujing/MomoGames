using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MomoGames.Utility
{
    public class DoubleLayerTimeWheel : SingleLayerTimeWheel
    {
        #region Constructors

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.DoubleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnSecondLayer">第二层的刻度数量。</param>
        public DoubleLayerTimeWheel(Byte ticksOnSecondLayer)
            : base()
        {
            Init(ticksOnSecondLayer);
        }

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.DoubleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnSecondLayer">第二层的刻度数量。</param>
        /// <param name="ticksOnFirstLayer">第一层的刻度数量。</param>
        public DoubleLayerTimeWheel(Byte ticksOnSecondLayer, Byte ticksOnFirstLayer)
            : base(ticksOnFirstLayer)
        {
            Init(ticksOnSecondLayer);
        }

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.DoubleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnSecondLayer">第二层的刻度数量。</param>
        /// <param name="ticksOnFirstLayer">第一层的刻度数量。</param>
        /// <param name="durationPerTick">刻度，单位毫秒。</param>
        public DoubleLayerTimeWheel(Byte ticksOnSecondLayer, Byte ticksOnFirstLayer, Int32 durationPerTick)
            : base(ticksOnFirstLayer, durationPerTick)
        {
            Init(ticksOnSecondLayer);
        }

        /// <summary>
        /// 初始化类型实例 MomoGames.Utility.DoubleLayerTimeWheel。
        /// </summary>
        /// <param name="ticksOnSecondLayer">第二层的刻度数量。</param>
        /// <param name="ticksOnFirstLayer">第一层的刻度数量。</param>
        /// <param name="durationPerTick">刻度，单位毫秒。</param>
        /// <param name="slotsProvider">创建用于存储计时器元素的列表。</param>
        public DoubleLayerTimeWheel(Byte ticksOnSecondLayer, Byte ticksOnFirstLayer, Int32 durationPerTick, Func<Byte, ICollection<TimerSlotElement>[]> slotsProvider)
            : base(ticksOnFirstLayer, durationPerTick, slotsProvider)
        {
            Init(ticksOnSecondLayer);
        }

        #endregion

        #region Properties

        #region TicksOnSecondLayer

        private Byte _ticksOnSecondLayer;

        public Byte TicksOnSecondLayer
        {
            get { return this._ticksOnSecondLayer; }
        }

        #endregion

        #region CurrentTickOnSecondLayer

        /// <summary>
        /// 第二层的Tick值。
        /// </summary>
        private Byte _currentTickOnSecondLayer;
        /// <summary>
        /// 获取第二层的Tick值。
        /// </summary>
        public Byte CurrentTickOnSecondLayer
        {
            get { return this._currentTickOnSecondLayer; }
        }

        #endregion

        /// <summary>
        /// 获取时间轮层数，从零开始。
        /// </summary>
        public override Byte Layer
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 获取时间轮的当前时间。
        /// </summary>
        public override Int32 CurrentTime
        {
            get
            {
                return (this._currentTickOnSecondLayer * TicksOnFirstLayer ) * DurationPerTick + base.CurrentTime;
            }
        }

        #endregion

        #region Methods

        private void Init(Byte ticksOnSecondLayer)
        {
            if (ticksOnSecondLayer <= 0)
            {
                throw new ArgumentException("ticksOnSecondLayer must bigger than 0.");
            }
            this._ticksOnSecondLayer = ticksOnSecondLayer;
        }

        /// <summary>
        /// 创建头。
        /// </summary>
        /// <param name="totalTicks">总的Tick数。</param>
        /// <param name="element">时间槽元素。</param>
        /// <param name="tick">所属的时间槽。</param>
        protected override Boolean CreateHeaders(Int64 totalTicks, TimerSlotElement element, out Byte tick)
        {
            Byte remainder = (Byte)(totalTicks % TicksOnFirstLayer);
            Byte data = (Byte)(totalTicks / TicksOnFirstLayer + this._currentTickOnSecondLayer) ;
            if (base.CreateHeaders(remainder, element, out tick))
            {
                data += 1;
            }
            if(data < TicksOnSecondLayer)
            {
                element.AddHeader(Layer, data);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取各层的Tick值，从最高层开始。
        /// </summary>
        /// <returns>各层的Tick值。</returns>
        protected override Byte[] GetTickOfEachLayer()
        {
            return new Byte[] { CurrentTickOnFirstLayer, this._currentTickOnSecondLayer};
        }

        /// <summary>
        /// 前进指针。
        /// </summary>
        /// <returns>如果进位返回true，否则返回false。</returns>
        protected override Boolean OnTick()
        {
            if (base.OnTick())
            {
                if(++this._currentTickOnSecondLayer == this._ticksOnSecondLayer)
                {
                    this._currentTickOnSecondLayer = 0;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 注册。
        /// </summary>
        protected override void ResetCore()
        {
            this._currentTickOnSecondLayer = 0;
        }

        #endregion
    }
}
