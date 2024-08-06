using System;


namespace Lockstep.Game
{
    public class FixedTimeCounter
    {
        private long _startTime;
        private int _startFrame;
        public int Interval { get; private set; }

        public FixedTimeCounter(long startTime, int startFrame, int interval)
        {
            _startTime = startTime;
            _startFrame = startFrame;
            Interval = interval;
        }

        public void ChangeInterval(int interval, int frame)
        {
            _startTime += (frame - _startFrame) * Interval;
            _startFrame = frame;
            Interval = interval;
        }

        public long FrameTime(int frame)
        {
            return _startTime + (frame - _startFrame) * Interval;
        }

        public void Reset(long time, int frame)
        {
            _startTime = time;
            _startFrame = frame;
        }
    }
}