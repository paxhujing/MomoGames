using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;

namespace MomoGames.Utility
{
    public abstract class TimeWheel
    {
        #region Fields

        #region init

        /// <summary>
        /// 系统计时器。
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// 一轮总共有多少Tick。
        /// </summary>
        private Int32 _ticksPerTurn;

        /// <summary>
        /// 时间槽列表。
        /// </summary>
        private readonly HashSet<TimerSlotElement>[] _slots;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化 MomoGames.Utility.TimeWheel 类型实例。
        /// </summary>
        /// <param name="timePerTick">每一刻度表示大于0的时长，单位毫秒。</param>
        /// <param name="ticksPerTurn">一轮总共的刻度数，必须大于1。</param>
        protected TimeWheel(Double timePerTick,Int32 ticksPerTurn)
        {
            if (timePerTick <= 0)
            {
                throw new ArgumentException("timerPerTick");
            }
            this._timePerTick = timePerTick;

            if(ticksPerTurn < 1)
            {
                throw new ArgumentException("ticksPerTurn");
            }
            this._ticksPerTurn = ticksPerTurn;

            this._slots = new HashSet<TimerSlotElement>[ticksPerTurn];
            for (Int32 i = 0; i < ticksPerTurn; i++)
            {
                this._slots[i] = new HashSet<TimerSlotElement>();
            }

            this._timer = new Timer(timePerTick);
            this._timer.Elapsed += _timer_Elapsed;
        }

        #endregion

        #region Properties

        #region TimePerTick

        /// <summary>
        /// 每一刻度表示的时长，单位毫秒。
        /// </summary>
        private readonly Double _timePerTick;
        /// <summary>
        /// 获取每一刻度表示的时长，单位毫秒。
        /// </summary>
        public Double TimePerTick
        {
            get { return this._timePerTick; }
        }

        #endregion

        #region CurrentTick

        /// <summary>
        /// 当前指针指向的Tick。
        /// </summary>
        private Int32 _currentTick;
        /// <summary>
        /// 获取当前的Tick。
        /// </summary>
        public Int32 CurrentTick
        {
            get { return this._currentTick; }
        }

        #endregion

        #endregion

        #region Methods

        #region Register&Unregister

        #endregion

        #region Generate Header

        /// <summary>
        /// 创建头部。
        /// </summary>
        /// <param name="type">头部类型。</param>
        /// <param name="data">头部数据</param>
        /// <returns>头部</returns>
        protected UInt32 NewHeader(TimerSlotElementHeaderType type, UInt16 data)
        {
            return NewHeader((Byte)type, data);
        }

        /// <summary>
        /// 创建头部。
        /// </summary>
        /// <param name="type">头部类型。</param>
        /// <param name="data">头部数据</param>
        /// <returns>头部</returns>
        protected UInt32 NewHeader(Byte type, UInt16 data)
        {
            UInt32 header = 0;
            header |= (UInt32)(type << 16);
            header |= data;
            return header;
        }

        #endregion

        #region Timer Core

        private void _timer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            if ((++this._currentTick) == this._ticksPerTurn)
            {
                this._currentTick = 0;
                ExecuteTurnFinished();
            }
            else
            {
                ExecuteTickForward(this._currentTick);
            }
        }

        /// <summary>
        /// 刻度每前进移步时执行。
        /// </summary>
        /// <param name="tick">当前刻度。</param>
        private void ExecuteTickForward(Int32 tick)
        {
            HashSet<TimerSlotElement> slot = this._slots[tick];
            lock(slot)
            {
                IEnumerable<TimerSlotElement> elements = slot.Where(x => x.Headers.Count == 1);
                foreach(TimerSlotElement e in elements)
                {
                }
            }
        }

        private void ExecuteTurnFinished()
        {
            //OnTickForward();
        }

        /// <summary>
        /// 在指针走完一轮时触发。
        /// </summary>
        protected abstract void OnTurnFinished();

        #endregion

        #region Start&Stop

        public void Start()
        {
            if(!this._timer.Enabled)
            {
                this._timer.Start();
            }
        }

        public void Stop()
        {
            if(this._timer.Enabled)
            {
                this._timer.Stop();
            }
        }

        #endregion

        #endregion
    }
}
