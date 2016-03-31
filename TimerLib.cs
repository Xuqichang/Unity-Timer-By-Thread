using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
public delegate void TimerCall(XQC.Lib.Timer.TimerData data);
///Made By Xqc 转载请注明出处!
namespace Xqc.Lib.Timer
{
    /// <summary>
    /// Script Should AddComponent GameObject
    /// And The Script Is By Thread
    /// </summary>
    public class TimerLib : MonoBehaviour
    {

        public static TimerLib instance = null;

        private List<Thread> _cache = new List<Thread>();

        private List<TimerData> timerdata = new List<TimerData>();


        // Use this for initialization
        void Awake()
        {

            instance = this;
        }

        /// <summary>
        /// Add Timer And Start
        /// </summary>
        /// <param name="timer">每隔多少秒执行一次.</param>
        /// <param name="count">执行多少次,0为无限循环</param>
        /// <param name="call">回调函数</param>
        public void AddTimer(float time,int count,TimerCall call)
        {
            if (count < 0)
            {
                Debug.Log("the count must be > 0");
                return;
            }
            Hashtable hash = new Hashtable();
            hash["timer"] = time;
            hash["count"] = count;
            hash["call"] = call;
            ParameterizedThreadStart pa = new ParameterizedThreadStart(StartTimer);
            Thread t = new Thread(pa);
            
            _cache.Add(t);
            t.Start(hash);
        }

        /// <summary>
        /// Remove ALL Timer
        /// </summary>
        /// <returns></returns>
        public bool RemoveTimer()
        {
            foreach (var key in _cache)
            {
                if (key != null)
                {
                    if (key.ThreadState == ThreadState.Running)
                    {
                        key.Abort();
                    }
                }
            }
            _cache.Clear();
            return true;

        }

        public bool RemoveTimer(TimerData data)
        {
            if (data != null)
            {
                data.setEnd = true;
            }

            return true;
        }
        /// <summary>
        /// 销毁所有定时器
        /// </summary>
        void OnDestroy()
        {
            foreach (var key in _cache)
            {
                if (key != null)
                {
                    if (key.ThreadState == ThreadState.Running)
                    {
                        key.Abort();
                    }
                }
            }
            _cache.Clear();
        }

        private void StartTimer(object o)
        {
            Hashtable hash = o as Hashtable;
            float timer = System.Convert.ToSingle(hash["timer"]);
            int count = System.Convert.ToInt32(hash["count"]);
            TimerCall call = (TimerCall)hash["call"];
            if (count != 0)
            {
                ThreadCount(timer, count, call);
            }
            else
            {
                ThreadTrue(timer, call);
            }
        }

        private void ThreadTrue(float time, TimerCall call)
        {
            TimerData data = new TimerData();
            data.setEnd = false;
            data.call = call;
            int cou = 0;
            while(true)
            {
             
                Thread.Sleep(System.Convert.ToInt32(time * 1000));
                if (data.setEnd == true)
                {
                    return;
                }
                cou++;
                data.fream = (cou);
                data.last = false;
                timerdata.Add(data);
            }

        }


        private void ThreadCount(float time,int count,TimerCall call)
        {
            TimerData data = new TimerData();
            data.setEnd = false;
            data.call = call;
            for (int i = 0; i < count; i++)
            {
                Thread.Sleep(System.Convert.ToInt32( time*1000));
                if (data.setEnd == true)
                {
                    return;
                }
                data.fream = (i + 1);
                data.last = false;
                if (i == count - 1)
                {
                    data.last = true;
                }
                timerdata.Add(data);
            }

        }

        // Update is called once per frame
        void Update()
        {
            lock (timerdata)
            {
                if (timerdata.Count > 0) {

                    var _data = timerdata[0];
                    _data.call(_data);
                    timerdata.RemoveAt(0);
                
                }
            }
           
        }
    }

    public class TimerData
    {
        /// <summary>
        /// The Current Fream
        /// </summary>
        public int fream { get; set; }

        /// <summary>
        /// Can End The Timer
        /// </summary>
        public bool setEnd { get; set; }

        /// <summary>
        /// The Last Fream
        /// </summary>
        public bool last { get; set; }

        /// <summary>
        /// 系统用
        /// </summary>
        public TimerCall call { get; set; }
    }
}
