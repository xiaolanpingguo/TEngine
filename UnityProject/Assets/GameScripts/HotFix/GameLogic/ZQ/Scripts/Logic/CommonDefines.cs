using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [System.Serializable]
    public enum ECampType : byte
    {
        Player,
        Enemy,
        Other,
    }
    public enum EDir
    {
        Up,
        Left,
        Down,
        Right,
        EnumCount,
    }

    public class PlayerInput1
    {
        public PlayerInput1()
        {

        }

        public static PlayerInput1 Empty = new PlayerInput1();

        public LVector2 mousePos;
        public LVector2 inputUV;
        public bool isInputFire;
        public int skillId;
        public bool isSpeedUp;

        public byte[] InputDatas;
        public bool IsMiss;
        public byte ActorId;
        public int Tick;
#if DEBUG_FRAME_DELAY
        public float timeSinceStartUp;
#endif

        public PlayerInput1(int tick, byte actorID)
        {
            this.Tick = tick;
            this.ActorId = actorID;
        }

        public void Reset()
        {
            mousePos = LVector2.zero;
            inputUV = LVector2.zero;
            isInputFire = false;
            skillId = 0;
            isSpeedUp = false;
        }
    }

    public class ServerFrame1
    {
        public byte[] inputDatas;
        public int tick;
        public PlayerInput1[] Inputs;
        public byte[] ServerInputs;
    }

    public class MutilFrames1
    {
        public int startTick;
        public ServerFrame1[] frames;
    }

    public class RepMissFrame : MutilFrames1
    {
    }

    public class ServerFrames : MutilFrames1
    {
    }

    [Serializable]
    public class GameStartInfo
    {
        public byte LocalId;
        public byte UserCount;
        public int MapId;
        public int RoomId;
        public int Seed;
        public string Ip;
        public ushort Port;
        public int SimulationSpeed;
    }
}