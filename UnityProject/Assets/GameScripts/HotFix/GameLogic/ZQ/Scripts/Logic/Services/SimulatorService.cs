using Lockstep.Framework;
using GameBase;
using TEngine;


namespace Lockstep.Game 
{
    public class SimulatorService : Singleton<SimulatorService>
    {
        public int __debugRockbackToTick;

        public const long MinMissFrameReqTickDiff = 10;
        public const long MaxSimulationMsPerFrame = 20;
        public const int MaxPredictFrameCount = 30;

        public int PingVal => _cmdBuffer?.PingVal ?? 0;
        public int DelayVal => _cmdBuffer?.DelayVal ?? 0;

        // components
        public World World => _world;
        private World _world;
        private FrameBuffer _cmdBuffer;
        private HashHelper _hashHelper;
        private DumpHelper _dumpHelper;

        // game status
        public byte LocalActorId { get; set; }
        private byte[] _allActors;
        private int _actorCount => _allActors.Length;
        private PlayerCommands[] _playerInputs => _world.PlayerInputs;
        public bool IsRunning { get; set; }

        /// frame count that need predict(TODO should change according current network's delay)
        public int FramePredictCount = 0; //~~~

        /// game init timestamp
        public long _gameStartTimestampMs = -1;

        private int _tickSinceGameStart;
        public int TargetTick => _tickSinceGameStart + FramePredictCount;

        // input presend
        public int PreSendInputCount = 1; //~~~
        public int inputTick = 0;
        public int inputTargetTick => _tickSinceGameStart + PreSendInputCount;


        //video mode
        private RepMissFrame _videoFrames;
        private bool _isInitVideo = false;
        private int _tickOnLastJumpTo;
        private long _timestampOnLastJumpToMs;

        private bool _isDebugRollback = false;

        public int snapshotFrameInterval = 1;
        private bool _hasRecvInputMsg;

        public GameConfigSingleton GameConfigSingleton;

        protected override void Init()
        {
            GameConfigSingleton = GameConfigSingleton.Instance;
            snapshotFrameInterval = 1;
            if (GameConfigSingleton.IsVideoMode) 
            {
                snapshotFrameInterval = GameConfigSingleton.SnapshotFrameInterval;
            }

            _cmdBuffer = new FrameBuffer(this, 2000, snapshotFrameInterval, MaxPredictFrameCount);
            _world = new World();
            _hashHelper = new HashHelper( _world, _cmdBuffer);
            _dumpHelper = new DumpHelper();

            InputManager.Init();
        }

        public void DoDestroy()
        {
            IsRunning = false;
            _dumpHelper.DumpAll();
        }


        public void CreateGame(byte localActorId, byte actorCount)
        {
            FrameBuffer.__debugMainActorID = localActorId;
            var allActors = new byte[actorCount];
            for (byte i = 0; i < actorCount; i++) 
            {
                allActors[i] = i;
            }

            Log.Info($"GameCreate " + LocalActorId);

            //Init game status
            //_localActorId = localActorId;
            _allActors = allActors;
            _world.Init();
            StartSimulate();
        }

        public void StartSimulate()
        {
            if (IsRunning) 
            {
                Log.Error("Already started!");
                return;
            }

            IsRunning = true;
            if (GameConfigSingleton.IsClientMode) 
            {
                _gameStartTimestampMs = LTime.realtimeSinceStartupMS;
            }

            _world.StartGame(LocalActorId);
            Log.Info($"Game Start");

            while (inputTick < PreSendInputCount)
            {
                SendInputs(inputTick++);
            }
        }

        public void Trace(string msg, bool isNewLine = false, bool isNeedLogTrace = false)
        {
            _dumpHelper.Trace(msg, isNewLine, isNeedLogTrace);
        }

        public void JumpTo(int tick)
        {
            if (tick + 1 == _world.Tick || tick == _world.Tick) return;

            tick = LMath.Min(tick, _videoFrames.frames.Length - 1);
            var time = LTime.realtimeSinceStartupMS + 0.05f;
            if (!_isInitVideo) 
            {
                GameConfigSingleton.IsVideoLoading = true;
                while (_world.Tick < _videoFrames.frames.Length) 
                {
                    var sFrame = _videoFrames.frames[_world.Tick];
                    Simulate(sFrame, true);
                }

                GameConfigSingleton.IsVideoLoading = false;
                _isInitVideo = true;
            }

            if (_world.Tick > tick) 
            {
                RollbackTo(tick, _videoFrames.frames.Length, false);
            }

            while (_world.Tick <= tick)
            {
                var sFrame = _videoFrames.frames[_world.Tick];
                Simulate(sFrame, false);
            }

            //_viewService.RebindAllEntities();
            _timestampOnLastJumpToMs = LTime.realtimeSinceStartupMS;
            _tickOnLastJumpTo = tick;
        }

        public void RunVideo()
        {
            if (_tickOnLastJumpTo == _world.Tick) 
            {
                _timestampOnLastJumpToMs = LTime.realtimeSinceStartupMS;
                _tickOnLastJumpTo = _world.Tick;
            }

            var frameDeltaTime = (LTime.timeSinceLevelLoad - _timestampOnLastJumpToMs) * 1000;
            var targetTick = System.Math.Ceiling(frameDeltaTime / NetworkDefine.UPDATE_DELTATIME) + _tickOnLastJumpTo;
            while (_world.Tick <= targetTick) 
            {
                if (_world.Tick < _videoFrames.frames.Length)
                {
                    var sFrame = _videoFrames.frames[_world.Tick];
                    Simulate(sFrame, false);
                }
                else 
                {
                    break;
                }
            }
        }

        public void DoUpdate(float deltaTime)
        {
            if (!IsRunning)
            {
                return;
            }

            if (_hasRecvInputMsg)
            {
                if (_gameStartTimestampMs == -1) 
                {
                    _gameStartTimestampMs = LTime.realtimeSinceStartupMS;
                }
            }

            if (_gameStartTimestampMs <= 0) 
            {
                return;
            }

            _tickSinceGameStart = (int)((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);
            if (GameConfigSingleton.IsVideoMode) 
            {
                return;
            }

            if (__debugRockbackToTick > 0)
            {
                _world.IsPause = true;
                RollbackTo(__debugRockbackToTick, 0, false);
                __debugRockbackToTick = -1;
            }

            if (_world.IsPause) 
            {
                return;
            }

            InputManager.Update();
            _cmdBuffer.DoUpdate(deltaTime);

            //client mode no network
            if (GameConfigSingleton.IsClientMode) 
            {
                DoClientUpdate();
            }
            else 
            {
                while (inputTick <= inputTargetTick)
                {
                    SendInputs(inputTick++);
                }

                DoNormalUpdate();
            }
        }


        private void DoClientUpdate()
        {
            int maxRollbackCount = 5;
            if (_isDebugRollback && _world.Tick > maxRollbackCount && _world.Tick % maxRollbackCount == 0) 
            {
                var rawTick = _world.Tick;
                //var revertCount = LRandom.Range(1, maxRollbackCount);
                var revertCount = 2;
                for (int i = 0; i < revertCount; i++) 
                {
                    var input = InputManager.CurrentInput;
                    input.Tick = _world.Tick;
                    input.ActorId = LocalActorId;
                    var frame = new ServerFrame() 
                    {
                        tick = rawTick - i,
                        Inputs = new PlayerCommands[] {input}
                    };

                    _cmdBuffer.ForcePushDebugFrame(frame);
                }

                Trace("RollbackTo " + (_world.Tick - revertCount));
                if (!RollbackTo(_world.Tick - revertCount, _world.Tick)) 
                {
                    _world.IsPause = true;
                    return;
                }

                while (_world.Tick < rawTick)
                {
                    var sFrame = _cmdBuffer.GetServerFrame(_world.Tick);
                    UnityEngine.Debug.Assert(sFrame != null && sFrame.tick == _world.Tick,$"logic error: server Frame  must exist tick {_world.Tick}");
                    _cmdBuffer.PushLocalFrame(sFrame);
                    Simulate(sFrame);
                    if (_world.IsPause) 
                    {
                        return;
                    }
                }
            }

            while (_world.Tick < TargetTick) 
            {
                FramePredictCount = 0;
                var input = InputManager.CurrentInput;
                input.Tick = _world.Tick;
                input.ActorId = LocalActorId;
                var frame = new ServerFrame() 
                {
                    tick = _world.Tick,
                    Inputs = new PlayerCommands[] {input}
                };
                _cmdBuffer.PushLocalFrame(frame);
                _cmdBuffer.PushServerFrames(new ServerFrame[] {frame});
                Simulate(_cmdBuffer.GetFrame(_world.Tick));
                if (_world.IsPause) 
                {
                    return;
                }
            }
        }

        private void DoNormalUpdate()
        {
            //make sure client is not move ahead too much than server
            var maxContinueServerTick = _cmdBuffer.MaxContinueServerTick;
            if ((_world.Tick - maxContinueServerTick) > MaxPredictFrameCount) 
            {
                return;
            }

            var minTickToBackup = (maxContinueServerTick - (maxContinueServerTick % snapshotFrameInterval));

            // Pursue Server frames
            var deadline = LTime.realtimeSinceStartupMS + MaxSimulationMsPerFrame;
            while (_world.Tick < _cmdBuffer.CurTickInServer) 
            {
                var tick = _world.Tick;
                var sFrame = _cmdBuffer.GetServerFrame(tick);
                if (sFrame == null)
                {
                    OnPursuingFrame();
                    return;
                }

                _cmdBuffer.PushLocalFrame(sFrame);
                Simulate(sFrame, tick == minTickToBackup);
                if (LTime.realtimeSinceStartupMS > deadline)
                {
                    OnPursuingFrame();
                    return;
                }
            }

            if (GameConfigSingleton.IsPursueFrame) 
            {
                GameConfigSingleton.IsPursueFrame = false;
                EventHelper.Trigger(EEvent.PursueFrameDone);
            }

            // Roll back
            if (_cmdBuffer.IsNeedRollback) 
            {
                RollbackTo(_cmdBuffer.NextTickToCheck, maxContinueServerTick);
                CleanUselessSnapshot(System.Math.Min(_cmdBuffer.NextTickToCheck - 1, _world.Tick));

                minTickToBackup = System.Math.Max(minTickToBackup, _world.Tick + 1);
                while (_world.Tick <= maxContinueServerTick) 
                {
                    var sFrame = _cmdBuffer.GetServerFrame(_world.Tick);
                    UnityEngine.Debug.Assert(sFrame != null && sFrame.tick == _world.Tick,$"logic error: server Frame  must exist tick {_world.Tick}");
                    _cmdBuffer.PushLocalFrame(sFrame);
                    Simulate(sFrame, _world.Tick == minTickToBackup);
                }
            }

            //Run frames
            while (_world.Tick <= TargetTick) 
            {
                var curTick = _world.Tick;
                ServerFrame frame = null;
                var sFrame = _cmdBuffer.GetServerFrame(curTick);
                if (sFrame != null) 
                {
                    frame = sFrame;
                }
                else 
                {
                    var cFrame = _cmdBuffer.GetLocalFrame(curTick);
                    FillInputWithLastFrame(cFrame);
                    frame = cFrame;
                }

                _cmdBuffer.PushLocalFrame(frame);
                Predict(frame, true);
            }

            _hashHelper.CheckAndSendHashCodes();
        }

        void SendInputs(int curTick)
        {
            var input = InputManager.CurrentInput;
            input.Tick = curTick;
            input.ActorId = LocalActorId;
            var cFrame = new ServerFrame();
            var inputs = new PlayerCommands[_actorCount];
            inputs[LocalActorId] = input;
            cFrame.Inputs = inputs;
            cFrame.tick = curTick;
            //FillInputWithLastFrame(cFrame);
            _cmdBuffer.PushLocalFrame(cFrame);
            //if (input.Commands != null) {
            //    var playerInput = new Deserializer(input.Commands[0].content).Parse<Lockstep.Game.PlayerInput>();
            //    Debug.Log($"SendInput curTick{curTick} maxSvrTick{_cmdBuffer.MaxServerTickInBuffer} _tickSinceGameStart {_tickSinceGameStart} uv {playerInput.inputUV}");
            //}
            if (curTick > _cmdBuffer.MaxServerTickInBuffer) 
            {
                //TODO combine all history inputs into one Msg 
                //Debug.Log("SendInput " + curTick +" _tickSinceGameStart " + _tickSinceGameStart);
                _cmdBuffer.SendInput(input);
            }
        }


        private void Simulate(ServerFrame frame, bool isNeedGenSnap = true)
        {
            Step(frame, isNeedGenSnap);
        }

        private void Predict(ServerFrame frame, bool isNeedGenSnap = true)
        {
            Step(frame, isNeedGenSnap);
        }

        private bool RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true)
        {
            _world.RollbackTo(tick, maxContinueServerTick, isNeedClear);
            var hash = GameStateService.Instance.Hash;
            var curHash = _hashHelper.CalcHash();
            if (hash != curHash) 
            {
                Log.Error($"tick:{tick} Rollback error: Hash isDiff oldHash ={hash}  curHash{curHash}");
#if UNITY_EDITOR
                _dumpHelper.DumpToFile(true);
                return false;
#endif
            }
            return true;
        }


        void Step(ServerFrame frame, bool isNeedGenSnap = true)
        {
            //Debug.Log("Step: " + _world.Tick + " TargetTick: " + TargetTick);
            var hash = _hashHelper.CalcHash();
            GameStateService.Instance.Hash = hash;
            DumpFrame(hash);
            hash = _hashHelper.CalcHash(true);
            _hashHelper.SetHash(_world.Tick, hash);
            ProcessInputQueue(frame);
            _world.Step(isNeedGenSnap);
            _dumpHelper.OnFrameEnd();
            var tick = _world.Tick;
            _cmdBuffer.SetClientTick(tick);
            //clean useless snapshot
            if (isNeedGenSnap && tick % snapshotFrameInterval == 0) 
            {
                CleanUselessSnapshot(System.Math.Min(_cmdBuffer.NextTickToCheck - 1, _world.Tick));
            }
        }

        private void CleanUselessSnapshot(int tick)
        {
            //TODO
        }

        private void DumpFrame(int hash)
        {
            if (GameConfigSingleton.IsClientMode) 
            {
                _dumpHelper.DumpFrame(!_hashHelper.TryGetValue(_world.Tick, out var val));
            }
            else 
            {
                _dumpHelper.DumpFrame(true);
            }
        }

        private void FillInputWithLastFrame(ServerFrame frame)
        {
            int tick = frame.tick;
            var inputs = frame.Inputs;
            var lastServerInputs = tick == 0 ? null : _cmdBuffer.GetFrame(tick - 1)?.Inputs;
            var myInput = inputs[LocalActorId];

            //fill inputs with last frame's input (Input predict)
            for (int i = 0; i < _actorCount; i++) 
            {
                inputs[i] = lastServerInputs[i];
            }

            inputs[LocalActorId] = myInput;
        }

        private void ProcessInputQueue(ServerFrame frame)
        {
            var inputs = frame.Inputs;
            foreach (var playerInput in _playerInputs) 
            {
                playerInput.Reset();
            }

            Player[] players = GameStateService.Instance.GetPlayers();
            foreach (var input in inputs)
            {
                if (input.ActorId >= _playerInputs.Length) continue;
                //_playerInputs[input.ActorId] = input;
                players[input.ActorId].input = input;
            }
        }

        private void OnPursuingFrame()
        {
            GameConfigSingleton.IsPursueFrame = true;
            Log.Info($"PurchaseServering curTick:" + _world.Tick);
            var progress = _world.Tick * 1.0f / _cmdBuffer.CurTickInServer;
            EventHelper.Trigger(EEvent.PursueFrameProcess, progress);
        }

        private void GetLocalInput()
        {

        }


        #region NetEvents

        public void OnEvent_BorderVideoFrame(object param)
        {
            _videoFrames = param as RepMissFrame;
        }

        public void OnEvent_OnServerFrame(object param)
        {
            var msg = param as ServerFrames;
            _hasRecvInputMsg = true;

            _cmdBuffer.PushServerFrames(msg.frames);
        }

        public void OnEvent_OnServerMissFrame(object param)
        {
            Log.Info($"OnEvent_OnServerMissFrame");
            var msg = param as RepMissFrame;
            _cmdBuffer.PushMissServerFrames(msg.frames, false);
        }

        public void OnEvent_LevelLoadDone(object param)
        {
            Log.Info($"OnEvent_LevelLoadDone " + GameConfigSingleton.IsReconnecting);
            if (GameConfigSingleton.IsReconnecting || GameConfigSingleton.IsVideoMode || GameConfigSingleton.IsClientMode)
            {
                StartSimulate();
            }
        }

        #endregion
    }
}