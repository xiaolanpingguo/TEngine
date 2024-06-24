using Lockstep.Framework;
using GameBase;
using TEngine;
using UnityEngine;
using System.Linq;


namespace Lockstep.Game
{
    public class ClientGameLoop : Singleton<ClientGameLoop>
    {
        public const long MinMissFrameReqTickDiff = 10;
        public const long MaxSimulationMsPerFrame = 20;
        public const int MaxPredictFrameCount = 30;

        public int PingVal => _frameBuffer?.PingVal ?? 0;
        public int DelayVal => _frameBuffer?.DelayVal ?? 0;

        // components
        public World World => _world;
        private World _world;
        private FrameBuffer _frameBuffer;
        private HashHelper _hashHelper;
        private DumpHelper _dumpHelper;

        // game status
        public byte LocalActorId { get; set; }
        private byte[] _allActors;
        public bool IsRunning { get; set; }

        /// frame count that need predict(TODO should change according current network's delay)
        public int FramePredictCount = 0; //~~~

        /// game init timestamp
        public long _gameStartTimestampMs = -1;

        private int _tickSinceGameStart;
        public int TargetTick;

        // input presend
        public int PreSendInputCount = 1; //~~~
        public int inputTick = 0;
        public int inputTargetTick => _tickSinceGameStart + PreSendInputCount;

        private int _snapshotFrameInterval = 1;

        public GameConfigSingleton GameConfigSingleton;

        private bool _isPursueFrame = false;

        protected override void Init()
        {
            GameConfigSingleton = GameConfigSingleton.Instance;
            _frameBuffer = new FrameBuffer(2000, _snapshotFrameInterval, MaxPredictFrameCount);
            _world = new World();
            _hashHelper = new HashHelper(_world, _frameBuffer);
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
            while (inputTick < PreSendInputCount)
            {
                SendInputs(inputTick++);
            }

            Log.Info($"Game Start");
        }

        public void Update()
        {
            if (!IsRunning)
            {
                return;
            }

            if (_gameStartTimestampMs <= 0)
            {
                return;
            }

            _tickSinceGameStart = (int)((LTime.realtimeSinceStartupMS - _gameStartTimestampMs) / NetworkDefine.UPDATE_DELTATIME);

            InputManager.Update();

            float fDeltaTime = Time.deltaTime;
            _frameBuffer.Update(fDeltaTime);

            while (inputTick <= inputTargetTick)
            {
                SendInputs(inputTick++);
            }

            //make sure client is not move ahead too much than server
            var maxContinueServerTick = _frameBuffer.MaxContinueServerTick;
            if ((_world.Tick - maxContinueServerTick) > MaxPredictFrameCount)
            {
                return;
            }

            var minTickToBackup = (maxContinueServerTick - (maxContinueServerTick % _snapshotFrameInterval));

            // Pursue Server frames
            var deadline = LTime.realtimeSinceStartupMS + MaxSimulationMsPerFrame;
            while (_world.Tick < _frameBuffer.CurTickInServer)
            {
                var tick = _world.Tick;
                var sFrame = _frameBuffer.GetServerFrame(tick);
                if (sFrame == null)
                {
                    OnPursuingFrame();
                    return;
                }

                _frameBuffer.PushLocalFrame(sFrame);
                Simulate(sFrame);
                if (LTime.realtimeSinceStartupMS > deadline)
                {
                    OnPursuingFrame();
                    return;
                }
            }

            if (_isPursueFrame)
            {
                _isPursueFrame = false;
                EventHelper.Trigger(EEvent.PursueFrameDone);
            }

            // Roll back
            if (_frameBuffer.IsNeedRollback)
            {
                RollbackTo(_frameBuffer.NextTickToCheck, maxContinueServerTick);

                minTickToBackup = System.Math.Max(minTickToBackup, _world.Tick + 1);
                while (_world.Tick <= maxContinueServerTick)
                {
                    var sFrame = _frameBuffer.GetServerFrame(_world.Tick);
                    UnityEngine.Debug.Assert(sFrame != null && sFrame.tick == _world.Tick, $"logic error: server Frame  must exist tick {_world.Tick}");
                    _frameBuffer.PushLocalFrame(sFrame);
                    Simulate(sFrame);
                }
            }

            //Run frames
            while (_world.Tick <= TargetTick)
            {
                var curTick = _world.Tick;
                ServerFrame frame = null;
                var sFrame = _frameBuffer.GetServerFrame(curTick);
                if (sFrame != null)
                {
                    frame = sFrame;
                }
                else
                {
                    var cFrame = _frameBuffer.GetLocalFrame(curTick);
                    FillInputWithLastFrame(cFrame);
                    frame = cFrame;
                }

                _frameBuffer.PushLocalFrame(frame);
                Predict(frame);
            }

            _hashHelper.CheckAndSendHashCodes();
        }

        void SendInputs(int curTick)
        {
            var input = InputManager.CurrentInput;
            input.Tick = curTick;
            input.ActorId = LocalActorId;
            var cFrame = new ServerFrame();
            var inputs = new PlayerCommands[_allActors.Length];
            inputs[LocalActorId] = input;
            cFrame.Inputs = inputs;
            cFrame.tick = curTick;
            //FillInputWithLastFrame(cFrame);
            _frameBuffer.PushLocalFrame(cFrame);
            //if (input.Commands != null) {
            //    var playerInput = new Deserializer(input.Commands[0].content).Parse<Lockstep.Game.PlayerInput>();
            //    Debug.Log($"SendInput curTick{curTick} maxSvrTick{_frameBuffer.MaxServerTickInBuffer} _tickSinceGameStart {_tickSinceGameStart} uv {playerInput.inputUV}");
            //}
            if (curTick > _frameBuffer.MaxServerTickInBuffer)
            {
                //TODO combine all history inputs into one Msg 
                //Debug.Log("SendInput " + curTick +" _tickSinceGameStart " + _tickSinceGameStart);
                _frameBuffer.SendInput(input);
            }
        }

        private void Simulate(ServerFrame frame)
        {
            var hash = _hashHelper.CalcHash();
            World.Instance.Hash = hash;
            DumpFrame(hash);
            hash = _hashHelper.CalcHash(true);
            _hashHelper.SetHash(_world.Tick, hash);
            ProcessInputQueue(frame);
            _world.Update();
            _dumpHelper.OnFrameEnd();
            var tick = _world.Tick;
            _frameBuffer.SetClientTick(tick);
        }

        private void Predict(ServerFrame frame)
        {
            var hash = _hashHelper.CalcHash();
            World.Instance.Hash = hash;
            DumpFrame(hash);
            hash = _hashHelper.CalcHash(true);
            _hashHelper.SetHash(_world.Tick, hash);
            ProcessInputQueue(frame);
            _world.Update();
            _dumpHelper.OnFrameEnd();
            var tick = _world.Tick;
            _frameBuffer.SetClientTick(tick);
        }

        private bool RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true)
        {
            _world.RollbackTo(tick, maxContinueServerTick, isNeedClear);
            var hash = World.Instance.Hash;
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

        private void DumpFrame(int hash)
        {
            _dumpHelper.DumpFrame(true);
        }

        private void FillInputWithLastFrame(ServerFrame frame)
        {
            int tick = frame.tick;
            var inputs = frame.Inputs;
            var lastServerInputs = tick == 0 ? null : _frameBuffer.GetFrame(tick - 1)?.Inputs;
            var myInput = inputs[LocalActorId];

            //fill inputs with last frame's input (Input predict)
            for (int i = 0; i < _allActors.Length; i++)
            {
                inputs[i] = lastServerInputs[i];
            }

            inputs[LocalActorId] = myInput;
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

        private void OnPursuingFrame()
        {
            _isPursueFrame = true;
            Log.Info($"PurchaseServering curTick:" + _world.Tick);
            var progress = _world.Tick * 1.0f / _frameBuffer.CurTickInServer;
            EventHelper.Trigger(EEvent.PursueFrameProcess, progress);
        }

        private void GetLocalInput()
        {

        }


        #region NetEvents


        public void OnEvent_OnServerFrame(object param)
        {
            var msg = param as ServerFrames;
            _gameStartTimestampMs = LTime.realtimeSinceStartupMS;

            _frameBuffer.PushServerFrames(msg.frames);
        }

        public void OnEvent_OnServerMissFrame(object param)
        {
            Log.Info($"OnEvent_OnServerMissFrame");
            var msg = param as RepMissFrame;
            _frameBuffer.PushMissServerFrames(msg.frames, false);
        }

        #endregion
    }
}