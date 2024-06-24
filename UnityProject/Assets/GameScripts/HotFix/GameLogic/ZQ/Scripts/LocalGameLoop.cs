using Lockstep.Framework;
using GameBase;
using TEngine;
using UnityEngine;
using System.Linq;


namespace Lockstep.Game
{
    public class LocalGameLoop : Singleton<LocalGameLoop>
    {
        public const int MaxPredictFrameCount = 30;

        public int PingVal => _frameBuffer?.PingVal ?? 0;
        public int DelayVal => _frameBuffer?.DelayVal ?? 0;

        // components
        private World _world;
        private FrameBuffer _frameBuffer;

        // game status
        public byte LocalActorId { get; set; }
        private byte[] _allActors;
        public bool IsRunning { get; set; }

        // frame count that need predict(TODO should change according current network's delay)
        public int FramePredictCount = 0;

        // game init timestamp
        public long _gameStartTimestampMs = -1;

        private int _tickSinceGameStart;
        public int TargetTick;

        // input presend
        public int PreSendInputCount = 1;
        public int inputTick = 0;
        public int inputTargetTick => _tickSinceGameStart + PreSendInputCount;

        private int _snapshotFrameInterval = 1;

        protected override void Init()
        {
            _frameBuffer = new FrameBuffer(2000, _snapshotFrameInterval, MaxPredictFrameCount);
            _world = new World();
            InputManager.Init();
        }

        public void DoDestroy()
        {
            IsRunning = false;
        }

        public void CreateGame(byte localActorId, byte actorCount)
        {
            if (IsRunning)
            {
                Log.Error("Already started!");
                return;
            }
            IsRunning = true;

            _allActors = new byte[actorCount];
            for (byte i = 0; i < actorCount; i++)
            {
                _allActors[i] = i;
            }

            _world.Init(LocalActorId);
            _gameStartTimestampMs = LTime.realtimeSinceStartupMS;
            Log.Info($"Game Start");
        }

        public void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            _tickSinceGameStart = (int)((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);

            InputManager.Update();

            float fDeltaTime = Time.deltaTime;
            _frameBuffer.Update(fDeltaTime);

            TargetTick = _tickSinceGameStart + FramePredictCount;
            while (_world.Tick < TargetTick)
            {
                FramePredictCount = 0;
                var input = InputManager.CurrentInput;
                input.Tick = _world.Tick;
                input.ActorId = LocalActorId;
                var frame = new ServerFrame()
                {
                    tick = _world.Tick,
                    Inputs = new PlayerCommands[] { input }
                };
                _frameBuffer.PushLocalFrame(frame);
                _frameBuffer.PushServerFrames(new ServerFrame[] { frame });
                Simulate(_frameBuffer.GetFrame(_world.Tick));
            }
        }

        private void Simulate(ServerFrame frame)
        {
            ProcessInputQueue(frame);
            _world.Update();
            var tick = _world.Tick;
            _frameBuffer.SetClientTick(tick);
        }

        private void ProcessInputQueue(ServerFrame frame)
        {
            var playerInputs = World.Instance.GetPlayers().Select(a => a.input).ToArray();
            foreach (var playerInput in playerInputs)
            {
                playerInput.Reset();
            }

            Player[] players = World.Instance.GetPlayers();
            var inputs = frame.Inputs;
            foreach (var input in inputs)
            {
                if (input.ActorId >= playerInputs.Length) continue;
                //_playerInputs[input.ActorId] = input;
                players[input.ActorId].input = input;
            }
        }
    }
}