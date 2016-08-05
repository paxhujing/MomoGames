using Microsoft.VisualStudio.TestTools.UnitTesting;
using MomoGames.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MomoGames.Utility.Tests
{
    [TestClass()]
    public class SingleTimeWheelTests
    {
        TimeWheel tw = new DoubleLayerTimeWheel(10);

        [TestMethod()]
        public void RegisterTest()
        {
            TimerSlotElement result = tw.Register(3500, Callback);
            Assert.IsNotNull(result);
        }

        private void Callback()
        {
            Debug.WriteLine("Callback time: " + tw.CurrentTime);
        }

        [TestMethod()]
        public void StartTest()
        {
            tw.Register(5050, Callback);
            tw.Start();
            while(true)
            {
                Debug.WriteLine(tw.CurrentTime);
                Thread.Sleep(50);
            }
            Assert.Fail();
        }
    }
}