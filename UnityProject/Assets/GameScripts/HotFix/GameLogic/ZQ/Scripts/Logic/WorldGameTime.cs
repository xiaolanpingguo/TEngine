using System;
using UnityEngine;


namespace Lockstep.Game
{
    public class WorldGameTime
    {
        private long _stampNow;
        public long ServerMinusClientTime { private get; set; }

        public long StartTime { get; private set; }
        public long Time { get; private set; }

        public void Start()
        {
            StartTime = StampNow();
        }

        public void Update()
        {
            _stampNow = StampNow();
            Time = _stampNow - StartTime;
        }

        public long StampNow()
        {
            DateTime currentTime = DateTime.UtcNow;
            return ((DateTimeOffset)currentTime).ToUnixTimeMilliseconds();
        }

        public long ServerNow()
        {
            return _stampNow + ServerMinusClientTime;
        }
    }
}