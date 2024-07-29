using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class ServerFrame
    {
        public int tick;
        public PlayerCommand[] Inputs;

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var frame = obj as ServerFrame;
            if (frame == null) 
            {
                return false;
            }

            return Equals(frame);
        }

        public override int GetHashCode()
        {
            return tick;
        }

        public bool Equals(ServerFrame frame)
        {
            if (frame == null)
            {
                return false;
            }
            if (tick != frame.tick)
            {
                return false;
            }

            if (Inputs == null || frame.Inputs == null)
            {
                return true;
            }

            var count = Inputs.Length;
            if (count != frame.Inputs.Length)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                if (Inputs[i] == frame.Inputs[i])
                {
                    return false;
                }
            }

            return true;
        }
    }


    public class RepMissFrame
    {
        public int startTick;
        public ServerFrame[] frames;
    }
}