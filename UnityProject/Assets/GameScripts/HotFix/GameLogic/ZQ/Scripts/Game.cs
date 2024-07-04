using Lockstep.Framework;
using UnityEngine;
using TEngine;


namespace Lockstep.Game
{
    public enum GameType
    {
        Local,
        Video,
        Client,
    }

    [Update]
    public class Game : BehaviourSingleton<Game>
    {
        public int JumpToTick = 10;
        public bool IsRunVideo;
        public bool IsVideoMode = false;

        public GameType GameType = GameType.Client;
        private VideoGameLoop _videoGameLoop;
        private ClientGameLoop _clientGameLoop;
        private LocalGameLoop _localGameLoop;

        public override void Awake()
        {
            LTLog.OnMessage += UnityLogHandler.LockstepLogHandler;
            Screen.SetResolution(1024, 768, false);

            LTime.Init();
        }

        public override void Start()
        {
            if (GameType == GameType.Local) 
            {
                LocalGameLoop.Instance.CreateGame();
            }
            else if (GameType == GameType.Video)
            {
                VideoGameLoop.Instance.SnapshotFrameInterval = 20;
            }
            else if (GameType == GameType.Client)
            {
                ClientGameLoop.Instance.CreateGame();
            }
        }

        public override void Update()
        {
            LTime.Update();

            if (GameType == GameType.Local)
            {
                LocalGameLoop.Instance.Update();
            }
            else if (GameType == GameType.Video)
            {
                if (IsRunVideo)
                {
                    VideoGameLoop.Instance.Update();
                }
                else
                {
                    VideoGameLoop.Instance.JumpTo(JumpToTick);
                }
            }
            else if (GameType == GameType.Client)
            {
                ClientGameLoop.Instance.Update();
            }
        }
    }
}
