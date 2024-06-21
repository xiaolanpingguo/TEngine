using Lockstep.Framework;
using UnityEngine;
using TEngine;


namespace Lockstep.Game
{
    [Update]
    public class Game : BehaviourSingleton<Game>
    {
        private RepMissFrame _framesInfo;
        private GameStartInfo _gameStartInfo;

        public string RecordPath;
        public int MaxRunTick = int.MaxValue;

        public int JumpToTick = 10;

        public int MaxEnemyCount = 10;
        public bool IsClientMode = true;
        public bool IsRunVideo;
        public bool IsVideoMode = false;
        public string RecordFilePath;

        public override void Awake()
        {
            LTLog.OnMessage += UnityLogHandler.LockstepLogHandler;
            Screen.SetResolution(1024, 768, false);

            GameConfigSingleton.Instance.IsClientMode = IsClientMode;
            GameConfigSingleton.Instance.IsVideoMode = IsVideoMode;

            GameStateService.Instance.MaxEnemyCount = MaxEnemyCount;
            LTime.DoStart();
        }

        public override void Start()
        {
            if (IsVideoMode)
            {
                GameConfigSingleton.Instance.SnapshotFrameInterval = 20;
                //OpenRecordFile(RecordPath);
            }

            if (IsVideoMode)
            {
                SimulatorService.Instance.OnEvent_BorderVideoFrame(_framesInfo);
                SimulatorService.Instance.OnEvent_OnGameCreate(_gameStartInfo);
            }
            else if (IsClientMode)
            {
                _gameStartInfo = GameConfigService.Instance.ClientModeInfo;
                SimulatorService.Instance.OnEvent_OnGameCreate(_gameStartInfo);
                SimulatorService.Instance.OnEvent_LevelLoadDone(_gameStartInfo);
            }
        }

        public override void Update()
        {
            float fDeltaTime = Time.deltaTime;

            LTime.DoUpdate();

            var deltaTime = fDeltaTime.ToLFloat();
            //_networkService.DoUpdate(deltaTime);

            if (IsVideoMode && IsRunVideo)
            {
                SimulatorService.Instance.RunVideo();
                return;
            }

            if (IsVideoMode && !IsRunVideo)
            {
                SimulatorService.Instance.JumpTo(JumpToTick);
            }

            SimulatorService.Instance.DoUpdate(fDeltaTime);
        }
    }
}
