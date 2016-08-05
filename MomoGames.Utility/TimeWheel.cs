using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace MomoGames.Utility
{
    public abstract class TimeWheel
    {
        #region Fields

        #region init

        /// <summary>
        /// 系统计时器。
        /// </summary>
        private readonly System.Timers.Timer _timer;

        /// <summary>
        /// 时间槽列表。
        /// </summary>
        private readonly ICollection<TimerSlotElement>[] _slots;

        /// <summary>
        /// 同步对象。
        /// </summary>
        private readonly Object _syncRoot = new Object();

        /// <summary>
        /// 计数器。
        /// </summary>
        internal Int32 TimerCounter;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化 MomoGames.Utility.TimeWheel 类型实例。
        /// </summary>
        /// <param name="durationPerTick">每一刻度表示大于0的时长，单位毫秒。</param>
        /// <param name="slotCount">刻度数量。</param>
        /// <param name="slotsProvider">创建用于存储计时器元素的列表。</param>
        protected TimeWheel(Int32 durationPerTick, Int16 slotCount, Func<Int16, ICollection<TimerSlotElement>[]> slotsProvider)
        {
            if (durationPerTick < 0)
            {
                throw new ArgumentException("durationPerTick >= 0ms.");
            }
            this._durationPerTick = durationPerTick;
            if (slotsProvider == null)
            {
                throw new ArgumentNullException("createSlots.");

            }
            this._slots = slotsProvider(slotCount);
            this._timer = new System.Timers.Timer(durationPerTick);
            this._timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// 初始化 MomoGames.Utility.TimeWheel 类型实例。
        /// </summary>
        /// <param name="durationPerTick">每一刻度表示大于0的时长，单位毫秒。</param>
        /// <param name="slotCount">刻度数量。</param>
        protected TimeWheel(Int32 durationPerTick, Int16 slotCount)
            : this(durationPerTick, slotCount, CreateTimersSlots)
        {
            
        }

        #endregion

        #region Properties

        #region DurationPerTick

        /// <summary>
        /// 每一刻度表示的时长，单位毫秒。
        /// </summary>
        private readonly Int32 _durationPerTick;
        /// <summary>
        /// 获取每一刻度表示的时长，单位毫秒。
        /// </summary>
        public Int32 DurationPerTick
        {
            get { return this._durationPerTick; }
        }

        #endregion


        /// <summary>
        /// 获取时间轮的当前时间。
        /// </summary>
        public abstract Int32 CurrentTime
        {
            get;
        }

        /// <summary>
        /// 获取时间轮层数，从零开始。
        /// </summary>
        public abstract Byte Layer
        {
            get;
        }

        #endregion

        #region Methods

        #region Init&Register&Unregister

        /// <summary>
        /// 创建时间槽。
        /// </summary>
        /// <param name="slotCount">时间槽数量。</param>
        /// <returns>时间槽列表。</returns>
        protected static ICollection<TimerSlotElement>[] CreateTimersSlots(Int16 slotCount)
        {
            HashSet<TimerSlotElement>[] slots = new HashSet<TimerSlotElement>[slotCount];
            for (Int32 i = 0; i < slots.Length; i++)
            {
                slots[i] = new HashSet<TimerSlotElement>();
            }
            return slots;
        }

        /// <summary>
        /// 创建头。
        /// </summary>
        /// <param name="totalTicks">总的Tick数。</param>
        /// <param name="element">时间槽元素。</param>
        /// <param name="tick">所属的时间槽。</param>
        protected abstract void CreateHeaders(Int32 totalTicks, TimerSlotElement element, out Int16 tick);

        /// <summary>
        /// 注册一个计时器。
        /// </summary>
        /// <param name="elapsed">时间间隔，单位毫秒。param>
        /// <param name="callback">回到方法。<param>
        /// <returns>成功返回 TimerSlotElement实例，否则返回 null。<returns>
        public TimerSlotElement Register(Int32 elapsed, Action callback)
        {
            if (elapsed < this._durationPerTick) return null;
            Int32 totalTicks = (CurrentTime + elapsed)/this._durationPerTick;
            Int16 tick;
            TimerSlotElement element = new TimerSlotElement(callback);
            CreateHeaders(totalTicks, element, out tick);
            ICollection<TimerSlotElement> slot = this._slots[tick];
            element.Slot = slot;
            slot.Add(element);
            return element;
        }

        #endregion

        #region Timer Core

        private void _timer_Elapsed(Object sender, ElapsedEventArgs e)
        {
            Int16[] tickOfEachLayer = null;
            lock (this._syncRoot)
            {
                OnTick();
                tickOfEachLayer = GetTickOfEachLayer();
            }
            TimerSlotElement[] elements = null;
            ICollection<TimerSlotElement> slot = this._slots[tickOfEachLayer[tickOfEachLayer.Length - 1]];
            lock (slot)
            {
                if (slot.Count == 0) return;
                elements = slot.ToArray();
            }
            ExecuteCore(elements, tickOfEachLayer);
        }

        private void ExecuteCore(TimerSlotElement[] elements, Int16[] tickOfEachLayer)
        {
            Collection<TimerSlotElement> candidature = new Collection<TimerSlotElement>();
            Collection<TimerSlotElement> others = new Collection<TimerSlotElement>();
            for(Int32 i = 0; i<elements.Length;i++)
            {
                if(elements[i].IsCandidature)
                {
                    candidature.Add(elements[i]);
                }
                else
                {
                    others.Add(elements[i]);
                }
            }
            if (others.Count != 0)
            {
                Task.Factory.StartNew(UpdateHeader, new Object[] { others, tickOfEachLayer });
            }
            if (candidature.Count != 0)
            {
                Task.Factory.StartNew(ExecuteCandidature, new Object[] { candidature, tickOfEachLayer });
            }
        }

        private void UpdateHeader(Object state)
        {
            Object[] args = (Object[])state;
            Collection<TimerSlotElement> items = (Collection<TimerSlotElement>)args[0];
            Int16[] tickOfEachLayer = (Int16[])args[1];

            for (Int32 i = 0; i < items.Count; i++)
            {
                if (!items[i].IsEnable) continue;
                if (items[i].CurrentHeader.Data <= tickOfEachLayer[items[i].CurrentHeader.Layer])
                {
                    items[i].RemoveCurrentHeader();
                }
            }
        }

        private void ExecuteCandidature(Object state)
        {
            Object[] args = (Object[])state;
            Collection<TimerSlotElement> items = (Collection<TimerSlotElement>)args[0];
            Int16[] tickOfEachLayer = (Int16[])args[1];

            Collection<TimerSlotElement> executions = new Collection<TimerSlotElement>();
            for (Int32 i = 0; i < items.Count; i++)
            {
                if (!items[i].IsEnable) continue;
                if (items[i].HeaderCount == 1)
                {
                    if (items[i].CurrentHeader.Data <= tickOfEachLayer[items[i].CurrentHeader.Layer])
                    {
                        items[i].RemoveCurrentHeader();
                        items[i].Unload();
                        executions.Add(items[i]);
                    }
                }
                else
                {
                    items[i].Unload();
                    executions.Add(items[i]);
                }
            }
            if (executions.Count == 0) return;
            for (Int32 i = 0; i < executions.Count; i++)
            {
                executions[i].Callback();
            }
        }

        /// <summary>
        /// 前进指针。
        /// </summary>
        /// <returns>如果进位返回true，否则返回false。</returns>
        protected abstract Boolean OnTick();

        /// <summary>
        /// 获取各层的Tick值，从最高层开始。
        /// </summary>
        /// <returns>各层的Tick值。</returns>
        protected abstract Int16[] GetTickOfEachLayer();

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
            if (this._timer.Enabled)
            {
                this._timer.Stop();
                for (Int32 i = 0; i < this._slots.Length; i++)
                {
                    if (this._slots[i].Count == 0) continue;
                    this._slots[i].Clear();
                }
            }
        }

        public void Reset()
        {
            Stop();
            ResetCore();
        }

        protected abstract void ResetCore();

        #endregion

        #endregion
    }
}
