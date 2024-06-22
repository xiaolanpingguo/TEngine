using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class ServerFrame
    {
        public byte[] inputDatas;
        public int tick;
        public PlayerCommands[] Inputs;
        public byte[] ServerInputs;
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