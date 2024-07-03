using Lockstep.Framework;
using GameBase;
using TEngine;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Windows;


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

        // game init timestamp
        public long _gameStartTimestampMs = -1;

        private int _tickSinceGameStart;
        public int TargetTick;

        // input presend
        private PlayerCommand _receivedPlayerCommand;
        private bool _receivedInput = false;
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


        public void CreateGame()
        {
            byte localPlayerId = 0;
            _world.Init(localPlayerId);
            _gameStartTimestampMs = LTime.realtimeSinceStartupMS;
            Log.Info($"Game Start");
        }

        public void Update()
        {
            _tickSinceGameStart = (int)((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);

            InputManager.Update();
            UpdateMockNetwork();

            float fDeltaTime = Time.deltaTime;
            _frameBuffer.Update(fDeltaTime);

            TargetTick = _tickSinceGameStart;
            while (_world.Tick < TargetTick)
            {
                PushFrame();
                ProcessInputQueue();
                _world.Update();
                var tick = _world.Tick;
                _frameBuffer.SetClientTick(tick);
            }
        }

        private void UpdateMockNetwork()
        {
            var input = InputManager.CurrentInput;
            input.Tick = _world.Tick;
            input.EntityId = _world.LocalPlayerId;

            _receivedPlayerCommand = input;
            _receivedInput = true;
        }

        private void PushFrame()
        {
            var input = GetPlayerInput();
            var frame = new ServerFrame()
            {
                tick = _world.Tick,
                Inputs = new PlayerCommand[] { input }
            };
            _frameBuffer.PushLocalFrame(frame);
            _frameBuffer.PushServerFrames(new ServerFrame[] { frame });
        }

        private PlayerCommand GetPlayerInput()
        {
            return _receivedPlayerCommand;
        }

        private void ProcessInputQueue()
        {
            ServerFrame frame = _frameBuffer.GetFrame(_world.Tick);
            if (frame == null)
            {
                return;
            }

            var inputs = frame.Inputs;
            foreach (var input in inputs)
            {
                if (input.EntityId >= _world.PlayerInputs.Length)
                {
                    continue;
                }

                if (input.inputUV.x >0 || input.inputUV.y >0)
                {
                   // Debug.Log("dawdawdawd");
                }
                _world.PlayerInputs[input.EntityId] = input;
            }
        }
    }
}