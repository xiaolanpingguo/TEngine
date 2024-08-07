using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using Lockstep.Framework;
using UnityEngine;
using TEngine;
using System.Text;
using C2DS;
using Google.Protobuf;
using System.Management.Instrumentation;
using UnityEngine.InputSystem;
using GameLogic;


namespace Lockstep.Game
{
    public enum EntityType
    {
        Spawner,
        Enemy,
        Player,
    }

    public class World
    {
        public enum State
        {
            Stop,
            Running,
            Pause,
        }

        internal struct GameState
        {
            public LFloat RemainTime;
            public LFloat DeltaTime;
            public int CurEnemyCount;
            public int CurEnemyId;

            public int GetHash(ref int idx)
            {
                int hash = 1;
                hash += CurEnemyCount.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += CurEnemyId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                return hash;
            }
        }

        public static World Instance { get; private set; }

        public int FramePredictCount = 0;

        public int Ping { get; private set; }
        public int Tick { get; private set; }
        public Player LocalPlayer;
        public PlayerCommand[] PlayerInputs;

        public int CurEnemyCount
        {
            get => _curGameState.CurEnemyCount;
            set => _curGameState.CurEnemyCount = value;
        }

        private readonly NetworkModule _networkModule;

        private State _state;
        private List<IGameSystem> _systems = new List<IGameSystem>();
        private Dictionary<Type, IGameSystem> _systemMap = new();

        private Dictionary<PrefabType, GameObject> _id2Prefab = new();

        private Dictionary<int, GameState> _tick2State = new Dictionary<int, GameState>();
        private GameState _curGameState;
        private Dictionary<Type, IList> _type2Entities = new Dictionary<Type, IList>();
        private Dictionary<int, Entity> _id2Entities = new Dictionary<int, Entity>();
        private Dictionary<int, Serializer> _tick2Backup = new Dictionary<int, Serializer>();

        private Dictionary<int, int> _tick2StateHash = new Dictionary<int, int>();
        public int Hash { get; set; }

        private int _playerCount = 1;
        private PlayerInfo _myPlayerInfo = null;
        private int _entityIdCounter = 0;
        private Dictionary<string, int> _profile2Index = new();
        private Dictionary<int, int> _tick2Id = new Dictionary<int, int>();

        private int _inputTick = 0;
        private FrameBuffer _frameBuffer;

        private const int k_frameRate = 20;
        private const int k_updateIntervalMs = 1000 / k_frameRate;
        //private LFloat WorldUpdateTick = new LFloat(true, k_updateIntervalMs);
        private LFloat WorldUpdateTick = new LFloat(true, 30);

        private WorldGameTime _worldGameTime;
        private FixedTimeCounter _fixedTimeCounter;
        private const int k_maxPredictionTick = 5;
        private const int k_maxPursueTime= 5; // ms
        private int _predictionTick = -1;
        private int _authorityTick = -1;
        private long _lastWorldTickTime = 0;

        public World(NetworkModule networkModule)
        {
            _networkModule = networkModule;
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdDs2CAdjustUpdateTimeReq, typeof(DS2CAdjustUpdateTimeReq), OnMsgAdjustUpdateTimeReq);
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdC2DsPingRes, typeof(C2DSPingRes), OnMsgPingRes);
            _networkModule.RegisterMessage((ushort)C2DS_MSG_ID.IdDs2CServerFrameReq, typeof(DS2CServerFrameReq), OnMsgServerFrameReq);
            GameModule.Timer.AddTimer(UpdatePing, 1f, true);
            _worldGameTime = new WorldGameTime();
        }

        public void Init()
        {
            Instance = this;
            _state = State.Stop;
            _frameBuffer = new FrameBuffer();
            RegisterSystems();

            foreach (var sys in _systems)
            {
                sys.Init();
            }
        }

        public void Destroy()
        {
            _state = State.Stop;
            foreach (var sys in _systems)
            {
                sys.Destroy();
            }
        }

        public void OnApplicationQuit()
        {
            Destroy();
        }
        
        public void Update()
        {
            if (_state != State.Running)
            {
                return;
            }

            _worldGameTime.Update();
            UpdatePredict();
            UpdateUI();
        }

        private void UpdatePredict()
        {
            long timeNow = _worldGameTime.ServerNow();
            while (true)
            {
                if (timeNow < _fixedTimeCounter.FrameTime(_predictionTick + 1))
                {
                    return;
                }

                // predict up to 5 frames
                if (_predictionTick - _authorityTick > k_maxPredictionTick)
                {
                    return;
                }

                ++_predictionTick;
                ServerFrame frame = GetOneFrameMessage(_predictionTick);
                RunOneFrame(frame);
                //SendHash(room.PredictionFrame);

                SendInput(frame);

                long timeNow2 = _worldGameTime.ServerNow();
                if (timeNow2 - timeNow > k_maxPursueTime)
                {
                    break;
                }
            }
        }

        public void RegisterSystems()
        {
            RegisterSystem(new PhysicSystem(this));
            RegisterSystem(new SpawnerSystem(this));
            RegisterSystem(new PlayerSystem(this));
            RegisterSystem(new EnemySystem(this));
        }

        public void RegisterSystem(IGameSystem sys)
        {
            Type type = sys.GetType();
            if (_systemMap.ContainsKey(type))
            {
                return;
            }

            _systemMap.Add(type, sys);
            _systems.Add(sys);
        }

        public T GetSystem<T>() where T : IGameSystem
        {
            Type type = typeof(T);
            if (_systemMap.TryGetValue(type, out var v))
            {
                return v as T;
            }

            return null;
        }

        public void OnGameStart(PlayerInfo[] players, PlayerInfo myPlayer)
        {
            _playerCount = players.Length;
            _myPlayerInfo = myPlayer;
            for (int i = 0; i < _playerCount; i++)
            {
                var initPos = LVector2.zero;
                var player = CreateEntity<Player>(initPos);
                if (_myPlayerInfo.PlayerIndex == i)
                {
                    LocalPlayer = player;
                    player.EntityId = i;
                }

                PlayerInfo playerInfo = players[i];
                _profile2Index[playerInfo.ProfileId] = playerInfo.PlayerIndex;
            }

            _state = State.Running;
            PlayerInputs = new PlayerCommand[_playerCount];
            _worldGameTime.Start();
            _fixedTimeCounter = new FixedTimeCounter(_worldGameTime.StartTime, 0, k_updateIntervalMs);
        }

        private void UpdateUI()
        {
            GameInfoUI.Instance?.SetPing(Ping);
            GameInfoUI.Instance?.SetAuthorityTick(_authorityTick);
            GameInfoUI.Instance?.SetPredictionTick(_predictionTick);
        }

        private GameObject LoadPrefab(PrefabType type)
        {
            if (_id2Prefab.TryGetValue(type, out var val))
            {
                return val;
            }

            GameConfig gameConfig = GameConfigSingleton.Instance.GameConfig;
            string prefabPath = gameConfig.GetPrefabPath(type);
            if (string.IsNullOrEmpty(prefabPath))
            {
                return null;
            }

            var prefab = (GameObject)Resources.Load(prefabPath);
            if (prefab == null)
            {
                return null;
            }

            _id2Prefab[type] = prefab;
            return prefab;
        }

        private void Rollback(int tick)
        {
            ServerFrame authorityFrame = _frameBuffer.GetFrame(tick);
            RunOneFrame(authorityFrame);
            //SendHash(tick);

            // re-run prediction tick
            for (int i = _authorityTick + 1; i <= _predictionTick; ++i)
            {
                ServerFrame frame = _frameBuffer.GetFrame(i);
 
                // copy others inputs
                foreach (var input in authorityFrame.Inputs)
                {
                    if (input.EntityId == _myPlayerInfo.PlayerIndex)
                    {
                        continue;
                    }

                    frame.Inputs[input.EntityId] = input;
                }

                RunOneFrame(frame);
            }

            //RunLSRollbackSystem(room);
        }

        private void RunOneFrame(ServerFrame frame)
        {
            long now = _worldGameTime.StampNow();
            LFloat deltaTime = new LFloat((int)Time.deltaTime);
            // set inputs
            PlayerInputs = frame.Inputs;
            foreach (var sys in _systems)
            {
                if (sys.Enable)
                {
                    sys.Update(WorldUpdateTick);
                    //sys.Update(deltaTime);
                }
            }

            Tick++;
        }

        private void BindView(Entity entity, Entity oldEntity = null)
        {
            if (oldEntity != null)
            {
                if (oldEntity.EntityId == entity.EntityId)
                {
                    entity.UserData = oldEntity.UserData;
                    var obj = (oldEntity.UserData as GameObject).gameObject;
                    var views = obj.GetComponents<EntityView>();
                    foreach (var view in views)
                    {
                        view.BindEntity(entity, oldEntity);
                    }
                }
                else
                {
                    UnbindView(oldEntity);
                }
            }
            else
            {
                PrefabType prefabType = GetPrefabTypeByEntity(entity);
                var prefab = LoadPrefab(prefabType);
                if (prefab == null)
                {
                    return;
                }
                var obj = GameObject.Instantiate(prefab, entity.LTrans2D.Pos3.ToVector3(), Quaternion.Euler(new Vector3(0, entity.LTrans2D.deg, 0)));
                entity.UserData = obj;
                var views = obj.GetComponents<EntityView>();
                if (views.Length <= 0)
                {
                    var view = obj.AddComponent<EntityView>();
                    view.BindEntity(entity);
                }
                else
                {
                    foreach (var view in views)
                    {
                        view.BindEntity(entity);
                    }
                }
            }
        }

        private void AddEntity<T>(T e) where T : Entity
        {
            if (typeof(T) == typeof(Player))
            {
                Log.Info("Add Player");
            }

            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj))
            {
                var lst = lstObj as List<T>;
                lst.Add(e);
            }
            else
            {
                var lst = new List<T>();
                _type2Entities.Add(t, lst);
                lst.Add(e);
            }

            _id2Entities[e.EntityId] = e;
        }

        private void RemoveEntity<T>(T e) where T : Entity
        {
            var t = e.GetType();
            if (_type2Entities.TryGetValue(t, out var lstObj))
            {
                lstObj.Remove(e);
                _id2Entities.Remove(e.EntityId);
            }
            else
            {
                Log.Error("Try remove a deleted Entity" + e);
            }
        }

        private List<T> GetEntities<T>()
        {
            var t = typeof(T);
            if (_type2Entities.TryGetValue(t, out var lstObj))
            {
                return lstObj as List<T>;
            }
            else
            {
                var lst = new List<T>();
                _type2Entities.Add(t, lst);
                return lst;
            }
        }

        public int GetHash(ref int idx)
        {
            int hash = 1;

            //hash += _entityIdCounter * PrimerLUT.GetPrimer(idx++);
            //foreach (var entity in GetPlayers())
            //{
            //    hash += entity.curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //    hash += entity.LTrans2D.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //    hash += entity.skillBox.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //}

            //foreach (var entity in GetEnemies())
            //{
            //    hash += entity.curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //    hash += entity.LTrans2D.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //}

            //foreach (var entity in GetSpawners())
            //{
            //    hash += entity.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //}

            //hash += _curGameState.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine("Hash ------: " + Hash);
            sb.AppendLine(prefix + "EntityIdCounter" + ":" + _entityIdCounter.ToString());
            SerializeUtil.DumpList("GetPlayers", GetPlayers(), sb, prefix);
            SerializeUtil.DumpList("GetEnemies", GetEnemies(), sb, prefix);
            SerializeUtil.DumpList("GetSpawners", GetSpawners(), sb, prefix);
            sb.AppendLine(prefix + "EntityId" + ":" + _curGameState.ToString());
        }

        public Enemy[] GetEnemies()
        {
            return GetEntities<Enemy>().ToArray();
        }

        public Player[] GetPlayers()
        {
            return GetEntities<Player>().ToArray();
        }

        public Spawner[] GetSpawners()
        {
            return GetEntities<Spawner>().ToArray();
        }

        public object GetEntity(int id)
        {
            if (_id2Entities.TryGetValue(id, out var val))
            {
                return val;
            }

            return null;
        }

        public T CreateEntity<T>(LVector3 position, List<IComponent> components = null) where T : Entity, new()
        {
            var entity = new T();
            entity.EntityId = GenId();
            entity.LTrans2D.Pos3 = position;

            if (components != null)
            {
                foreach (var c in components)
                {
                    entity.AddComponent(c);
                }
            }
            entity.Start();
            BindView(entity);
            AddEntity(entity);

            EColliderLayer layer = entityType2Layer(typeof(T));
            GetSystem<PhysicSystem>().RegisterEntity(entity.EntityId, entity, (int)layer);
            return entity;
        }

        public void DestroyEntity(Entity entity)
        {
            RemoveEntity(entity);
        }

        public void Backup(int tick)
        {
            _tick2Id[tick] = _entityIdCounter;
            _tick2State[tick] = _curGameState;
            Serializer writer = new Serializer();
            writer.Write(Hash); //hash
            _tick2StateHash[tick] = Hash;
            BackUpEntities(GetPlayers(), writer);
            BackUpEntities(GetEnemies(), writer);
            BackUpEntities(GetSpawners(), writer);
            _tick2Backup[tick] = writer;
        }

        public void RollbackTo(int tick)
        {
            _entityIdCounter = _tick2Id[tick];
            _curGameState = _tick2State[tick];
            if (_tick2Backup.TryGetValue(tick, out var backupData))
            {
                //.TODO reduce the unnecessary create and destroy 
                var reader = new Deserializer(backupData.Data);
                var hash = reader.ReadInt32();
                _tick2StateHash[tick] = hash;

                var oldId2Entity = _id2Entities;
                _id2Entities = new Dictionary<int, Entity>();
                _type2Entities.Clear();

                //. Recover Entities
                RecoverEntities(new List<Player>(), reader);
                RecoverEntities(new List<Enemy>(), reader);
                RecoverEntities(new List<Spawner>(), reader);

                //. Rebind Views 
                foreach (var pair in _id2Entities)
                {
                    Entity oldEntity = null;
                    if (oldId2Entity.TryGetValue(pair.Key, out var poldEntity))
                    {
                        oldEntity = poldEntity;
                        oldId2Entity.Remove(pair.Key);
                    }
                    BindView(pair.Value, oldEntity);
                }

                //. Unbind Entity views
                foreach (var pair in oldId2Entity)
                {
                    UnbindView(pair.Value);
                }
            }
            else
            {
                Log.Error($"Miss backup data  cannot rollback! {tick}");
            }
        }

        private void BackUpEntities<T>(T[] lst, Serializer writer) where T : Entity, new()
        {
            writer.Write(lst.Length);
            foreach (var item in lst)
            {
                item.Serialize(writer);
            }
        }

        private List<T> RecoverEntities<T>(List<T> lst, Deserializer reader) where T : Entity, new()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var t = new T();
                lst.Add(t);
                t.Deserialize(reader);
            }

            _type2Entities[typeof(T)] = lst;
            foreach (var e in lst)
            {
                _id2Entities[e.EntityId] = e;
            }

            return lst;
        }

        private int GenId()
        {
            return _entityIdCounter++;
        }

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

        public void UnbindView(Entity entity)
        {
            entity.OnRollbackDestroy();
        }

        EColliderLayer entityType2Layer(Type type)
        {
            if (type == typeof(Player))
            {
                return EColliderLayer.Player;
            }
            else if (type == typeof(Enemy)) 
            {
                return EColliderLayer.Enemy;
            }
            else
            {
                return EColliderLayer.Static;
            }
        }

        PrefabType GetPrefabTypeByEntity(Entity entity)
        {
            Type type = entity.GetType();
            if (type == typeof(Player))
            {
                return PrefabType.Player;
            }
            else if (type == typeof(Enemy))
            {
                return PrefabType.Enemy;
            }
            else if (type == typeof(Spawner))
            {
                return PrefabType.Spawner;
            }
            else
            {
                return PrefabType.None;
            }
        }

        public PlayerCommand GetPlayerInput(int playerIndex)
        {
            if (playerIndex >= _playerCount)
            {
                return PlayerCommand.Empty;
            }

            return PlayerInputs[playerIndex];
        }

        private ServerFrame GetOneFrameMessage(int tick)
        {
            if (tick <= _authorityTick)
            {
                return _frameBuffer.GetFrame(tick);
            }

            // predict
            ServerFrame predictionFrame = _frameBuffer.GetFrame(tick);
            predictionFrame.Inputs = new PlayerCommand[_playerCount];
            _frameBuffer.MoveForward(tick);

            if (_frameBuffer.CheckFrame(_authorityTick))
            {
                ServerFrame authorityFrame = _frameBuffer.GetFrame(_authorityTick);
                authorityFrame.CopyTo(predictionFrame);
            }

            int index = _myPlayerInfo.PlayerIndex;
            predictionFrame.Inputs[index] = InputManager.CurrentInput;
            predictionFrame.Inputs[index].EntityId = index;
            return predictionFrame;
        }

        private void SendInput(ServerFrame frame)
        {
            PlayerCommand myInput = frame.Inputs[_myPlayerInfo.PlayerIndex];
            C2DSClientInputReq req = new C2DSClientInputReq();
            req.PlayerInput = new C2DS.PlayerInput();
            req.PlayerInput.ProfileId = _myPlayerInfo.ProfileId;
            req.PlayerInput.Tick = _inputTick;
            req.PlayerInput.Button = myInput.ButtonField;
            req.PlayerInput.Horizontal = myInput.inputUV._x;
            req.PlayerInput.Vertical = myInput.inputUV._y;
            _networkModule.Send(req, (ushort)C2DS_MSG_ID.IdC2DsClientInputReq);
        }

        private void UpdatePing(object[] args)
        {
            if (!_networkModule.IsConnected)
            {
                return;
            }

            C2DS.C2DSPingReq req = new C2DS.C2DSPingReq();
            req.ClientTime = TimeHelper.TimeStampNowMs();
            _networkModule.Send(req, (ushort)C2DS.C2DS_MSG_ID.IdC2DsPingReq);
        }

        private int GetPlayerIndexByProfileId(string profileId)
        {
            if (_profile2Index.TryGetValue(profileId, out var index))
            {
                return index;
            }

            return -1;
        }

        private void OnMsgServerFrameReq(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not DS2CServerFrameReq req)
            {
                Log.Error($"OnServerFrameReq error: cannot convert message to DS2CServerFrameReq");
                return;
            }

            _authorityTick++;

            ServerFrame serverFrame = MsgServerFrame2GameFrame(req.ServrFrame);

            // the client tick is behind the server, just use server data for this tick
            if (_authorityTick > _predictionTick)
            {
                ServerFrame authorityFrame = _frameBuffer.GetFrame(_authorityTick);
                serverFrame.CopyTo(authorityFrame);
            }
            // The client tick runs in front of the server tick, this is the case most of the time
            else
            {
                // comparing the predicted frame with the authoritative frame
                // there are two cases of prediction failure
                // 1: failure to predict other player's input
                // 2: failure to predict my input, In this case, the message you sent arrives later than the server,
                // the server will use the input of the previous frame to predict by default
                // when rolling back and re-predicting, my input does not need to change
                ServerFrame predictionFrame = _frameBuffer.GetFrame(_authorityTick);
                if (serverFrame != predictionFrame)
                {
                    serverFrame.CopyTo(predictionFrame);
                    Rollback(_authorityTick);
                }
                // predict success
                else
                {
                    //Record(_authorityTick);
                    //SendHash(_authorityTick);
                }
            }
        }

        private PlayerCommand MsgInput2GameInput(C2DS.PlayerInput msgInpt)
        {
            PlayerCommand playerCommand = default;
            playerCommand.Tick = msgInpt.Tick;
            playerCommand.inputUV._x = msgInpt.Horizontal;
            playerCommand.inputUV._y = msgInpt.Vertical;
            playerCommand.ButtonField = msgInpt.Button;
            playerCommand.EntityId = GetPlayerIndexByProfileId(msgInpt.ProfileId);
            return playerCommand;
        }

        private ServerFrame MsgServerFrame2GameFrame(C2DS.ServerFrame msgFrame)
        {
            ServerFrame serverFrame = new ServerFrame();
            serverFrame.tick = msgFrame.Tick;
            serverFrame.Inputs = new PlayerCommand[_playerCount];

            for (int i = 0; i < serverFrame.Inputs.Length; ++i)
            {
                serverFrame.Inputs[i] = PlayerCommand.Empty;
            }

            foreach (var msgInput in msgFrame.PlayerInputs)
            {
                PlayerCommand playerCommand = MsgInput2GameInput(msgInput);
                if (playerCommand.EntityId < 0 || playerCommand.EntityId >= _playerCount)
                {
                    continue;
                }

                serverFrame.Inputs[playerCommand.EntityId] = playerCommand;
            }

            return serverFrame;
        }

        public void OnMsgServerFrameRecieved(ServerFrame serverFrame)
        {
            _authorityTick++;

            // the client tick is behind the server, just use server data for this tick
            if (_authorityTick > _predictionTick)
            {
                ServerFrame authorityFrame = _frameBuffer.GetFrame(_authorityTick);
                serverFrame.CopyTo(authorityFrame);
            }
            // The client tick runs in front of the server tick, this is the case most of the time
            else
            {
                ServerFrame predictionFrame = _frameBuffer.GetFrame(_authorityTick);

                // comparing the predicted frame with the authoritative frame
                // there are two cases of prediction failure
                // 1: failure to predict other player's input
                // 2: failure to predict my input, In this case, the message you sent arrives later than the server,
                // the server will use the input of the previous frame to predict by default
                // when rolling back and re-predicting, my input does not need to change
                if (serverFrame != predictionFrame)
                {
                    serverFrame.CopyTo(predictionFrame);
                    Rollback(_authorityTick);
                }
                // predict success
                else
                {
                    //Record(_authorityTick);
                    //SendHash(_authorityTick);
                }
            }
        }

        private void OnMsgAdjustUpdateTimeReq(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not DS2CAdjustUpdateTimeReq req)
            {
                Log.Error($"OnMsgAdjustUpdateTimeReq error: cannot convert message to DS2CAdjustUpdateTimeReq");
                return;
            }

            int diffTime = req.DiffTime;
            int newInterval = (1000 + (diffTime - k_updateIntervalMs)) * k_updateIntervalMs / 1000;

            if (newInterval < 40)
            {
                newInterval = 40;
            }

            if (newInterval > 66)
            {
                newInterval = 66;
            }

            _fixedTimeCounter.ChangeInterval(newInterval, _predictionTick);
        }

        private void OnMsgPingRes(ushort messageId, int rpcId, IMessage message)
        {
            if (message == null || message is not C2DSPingRes res)
            {
                Log.Error($"OnMsgPingRes error: cannot convert message to C2DSPingRes");
                return;
            }

            long clientRequestTime = res.ClientTime;
            long serverTime = res.ServerTime;
            long now = TimeHelper.TimeStampNowMs();
            Ping = (int)(now - clientRequestTime);
            _worldGameTime.ServerMinusClientTime = serverTime + (now - clientRequestTime) / 2 - now;
        }
    }
}