using Lockstep.Framework;
using GameBase;
using TEngine;
using UnityEngine;
using System.Linq;


namespace Lockstep.Game
{
    public class VideoGameLoop : Singleton<VideoGameLoop>
    {
        public const int MaxPredictFrameCount = 30;

        private World _world;
        private FrameBuffer _frameBuffer;

        // game status
        public byte LocalActorId { get; set; }
        private byte[] _allActors;
        public bool IsRunning { get; set; }

        /// game init timestamp
        public long _gameStartTimestampMs = -1;

        private int _tickSinceGameStart;
        public int TargetTick;

        // input presend
        public int PreSendInputCount = 1;
        public int inputTick = 0;
        public int inputTargetTick => _tickSinceGameStart + PreSendInputCount;

        //video mode
        private RepMissFrame _videoFrames;
        private bool _isInitVideo = false;
        private int _tickOnLastJumpTo;
        private long _timestampOnLastJumpToMs;

        public int SnapshotFrameInterval = 1;
        private bool _isVideoLoading = false;

        protected override void Init()
        {
            //_frameBuffer = new FrameBuffer(2000, SnapshotFrameInterval, MaxPredictFrameCount);
            //_world = new World(null);
            //InputManager.Init();
        }

        public void DoDestroy()
        {
            IsRunning = false;
        }

        public void CreateGame(byte localActorId, byte actorCount)
        {
            //if (IsRunning)
            //{
            //    Log.Error("Already started!");
            //    return;
            //}
            //IsRunning = true;

            //_allActors = new byte[actorCount];
            //for (byte i = 0; i < actorCount; i++)
            //{
            //    _allActors[i] = i;
            //}

            //_world.Init(LocalActorId);
            //Log.Info($"Game Start");
        }

        public void JumpTo(int tick)
        {
            if (tick + 1 == _world.Tick || tick == _world.Tick) return;

            tick = LMath.Min(tick, _videoFrames.frames.Length - 1);
            var time = LTime.realtimeSinceStartupMS + 0.05f;
            if (!_isInitVideo)
            {
                _isVideoLoading = true;
                while (_world.Tick < _videoFrames.frames.Length)
                {
                    var sFrame = _videoFrames.frames[_world.Tick];
                    Simulate(sFrame);
                }

                _isVideoLoading = false;
                _isInitVideo = true;
            }

            if (_world.Tick > tick)
            {
                RollbackTo(tick, _videoFrames.frames.Length, false);
            }

            while (_world.Tick <= tick)
            {
                var sFrame = _videoFrames.frames[_world.Tick];
                Simulate(sFrame);
            }

            //_viewService.RebindAllEntities();
            _timestampOnLastJumpToMs = LTime.realtimeSinceStartupMS;
            _tickOnLastJumpTo = tick;
        }

        public void Update()
        {
            //if (_tickOnLastJumpTo == _world.Tick)
            //{
            //    _timestampOnLastJumpToMs = LTime.realtimeSinceStartupMS;
            //    _tickOnLastJumpTo = _world.Tick;
            //}

            //var frameDeltaTime = (LTime.timeSinceLevelLoad - _timestampOnLastJumpToMs) * 1000;
            //var targetTick = System.Math.Ceiling(frameDeltaTime / NetworkDefine.UPDATE_DELTATIME) + _tickOnLastJumpTo;
            //while (_world.Tick <= targetTick)
            //{
            //    if (_world.Tick < _videoFrames.frames.Length)
            //    {
            //        var sFrame = _videoFrames.frames[_world.Tick];
            //        Simulate(sFrame);
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }

        private bool RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true)
        {
            _world.RollbackTo(tick, maxContinueServerTick, isNeedClear);
            return true;
        }

        private void Simulate(ServerFrame frame)
        {
            //ProcessInputQueue(frame);
            //_world.Update();
            //var tick = _world.Tick;
            //_frameBuffer.SetClientTick(tick);
        }

        private void ProcessInputQueue(ServerFrame frame)
        {
            //var playerInputs = _world.GetPlayers().Select(a => a.input).ToArray();
            //foreach (var playerInput in playerInputs)
            //{
            //    playerInput.Reset();
            //}

            //Player[] players = _world.GetPlayers();
            //var inputs = frame.Inputs;
            //foreach (var input in inputs)
            //{
            //    if (input.EntityId >= playerInputs.Length) continue;
            //    //_playerInputs[input.ActorId] = input;
            //    players[input.EntityId].input = input;
            //}
        }
    }
}