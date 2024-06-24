using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class ServerFrame
    {
        public int tick;
        public PlayerCommands[] Inputs;
    }

    public class MutilFrames1
    {
        public int startTick;
        public ServerFrame[] frames;
    }

    public class RepMissFrame : MutilFrames1
    {
    }

    public class ServerFrames : MutilFrames1
    {
    }
}