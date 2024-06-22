using System.Collections.Generic;
using System.Linq;
using Lockstep.Framework;
using UnityEngine;
using TEngine;


namespace Lockstep.Game
{
    public class World
    {
        public static World Instance { get; private set; }
        public bool IsPause { get; set; }
        public int Tick { get; set; }
        public PlayerCommands[] PlayerInputs => GameStateService.Instance.GetPlayers().Select(a => a.input).ToArray();
        public static Player MyPlayer;
        public static object MyPlayerTrans => MyPlayer?.engineTransform;
        private List<IGameSystem> _systems = new List<IGameSystem>();
        private bool _hasStart = false;

        private Dictionary<int, GameObject> _id2Prefab = new Dictionary<int, GameObject>();
        private InputManager _inputManager = null;

        public void RollbackTo(int tick, int maxContinueServerTick, bool isNeedClear = true)
        {
            if (tick < 0)
            {
                Log.Error("Target Tick invalid!" + tick);
                return;
            }

            Debug.Log($" Rollback diff:{Tick - tick} From{Tick}->{tick}  maxContinueServerTick:{maxContinueServerTick} {isNeedClear}");

            //foreach (var service in _serviceContainer.GetAllServices())
            //{
            //    service.RollbackTo(tick);
            //}

            Tick = tick;
        }

        public void Init()
        {
            Instance = this;
            _inputManager = new InputManager();
            RegisterSystems();

            foreach (var mgr in _systems)
            {
                mgr.Init();
            }
        }

        public void StartGame(int localPlayerId)
        {
            if (_hasStart) return;
            _hasStart = true;
            var playerCount = 1;
            string _traceLogPath = "";
#if UNITY_STANDALONE_OSX
            _traceLogPath = $"/tmp/LPDemo/Dump_{localPlayerId}.txt";
#else
            _traceLogPath = $"c:/tmp/LPDemo/Dump_{localPlayerId}.txt";
#endif

            SimulatorService.Instance.Trace("CreatePlayer " + playerCount);

            //create Players 
            for (int i = 0; i < playerCount; i++)
            {
                var PrefabId = 0; //TODO
                var initPos = LVector2.zero; //TODO
                var player = GameStateService.Instance.CreateEntity<Player>(PrefabId, initPos);
                player.localId = i;
            }

            var allPlayers = GameStateService.Instance.GetPlayers();

            MyPlayer = allPlayers[localPlayerId];
        }

        public void DoDestroy()
        {
            foreach (var mgr in _systems)
            {
                mgr.Destroy();
            }
        }

        public void OnApplicationQuit()
        {
            DoDestroy();
        }

        public void Step(bool isNeedGenSnap = true)
        {
            if (IsPause) return;

            var deltaTime = new LFloat(true, 30);
            foreach (var system in _systems)
            {
                if (system.Enable)
                {
                    system.Update(deltaTime);
                }
            }

            Tick++;
        }

        public void RegisterSystems()
        {
            RegisterSystem(new PhysicSystem());
            RegisterSystem(new HeroSystem());
            RegisterSystem(new EnemySystem());
            if (!GameConfigSingleton.Instance.IsVideoMode)
            {
                RegisterSystem(new TraceLogSystem());
            }
        }

        public void RegisterSystem(IGameSystem mgr)
        {
            _systems.Add(mgr);
        }

        public GameObject LoadPrefab(int id)
        {
            if (_id2Prefab.TryGetValue(id, out var val))
            {
                return val;
            }

            var config = GameConfigSingleton.Instance.GetEntityConfig(id);
            if (string.IsNullOrEmpty(config.prefabPath))
            {
                return null;
            }

            string pathPrefix = "Prefabs/";
            var prefab = (GameObject)Resources.Load(pathPrefix + config.prefabPath);
            _id2Prefab[id] = prefab;
            PhysicSystem.Instance.RigisterPrefab(id, id < 10 ? (int)EColliderLayer.Hero : (int)EColliderLayer.Enemy);
            return prefab;
        }
    }
}