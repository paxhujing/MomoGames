using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MomoGames.Utility
{
    public class TimeWheel
    {
        #region Fields

        #region init

        /// <summary>
        /// 每走一刻执行的方法。
        /// </summary>
        private readonly Action _onTick;

        /// <summary>
        /// 系统计时器。
        /// </summary>
        private readonly Timer _timer;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化 MomoGames.Utility.TimeWheel 类型实例。
        /// </summary>
        /// <param name="timePerTick">每一刻度表示的时长，单位毫秒。</param>
        /// <param name="onTick">每走一刻执行的方法。</param>
        protected TimeWheel(Double timePerTick, Action onTick)
        {
            if (timePerTick <= 0)
            {
                throw new ArgumentException("timerPerTick");
            }
            this._timePerTick = timePerTick;
            if(onTick == null)
            {
                throw new ArgumentNullException("onTick");
            }
            this._onTick = onTick;
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

        #endregion

        #region Methods

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this._onTick();
        }

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
    }
}
