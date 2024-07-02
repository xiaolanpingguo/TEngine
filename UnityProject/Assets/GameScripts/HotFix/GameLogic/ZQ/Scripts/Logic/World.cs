using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using Lockstep.Framework;
using UnityEngine;
using TEngine;
using System.Text;
using Lockstep.Game;
using System.Net.Http.Headers;


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
        public bool IsPause { get; set; }
        public int Tick { get; set; }
        public static Player MyPlayer;

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

        private int _entityIdCounter = 0;
        private Dictionary<int, int> _tick2Id = new Dictionary<int, int>();
        public LFloat RemainTime
        {
            get => _curGameState.RemainTime;
            set => _curGameState.RemainTime = value;
        }

        public LFloat DeltaTime
        {
            get => _curGameState.DeltaTime;
            set => _curGameState.DeltaTime = value;
        }

        public int CurEnemyCount
        {
            get => _curGameState.CurEnemyCount;
            set => _curGameState.CurEnemyCount = value;
        }

        public int CurEnemyId
        {
            get => _curGameState.CurEnemyId;
            set => _curGameState.CurEnemyId = value;
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
            return GetEntities<Spawner>().ToArray(); //TODO Cache
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
                foreach(var c in components)
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

        public void Clean(int maxVerifiedTick)
        {
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

        public void Init(int localPlayerId)
        {
            Instance = this;
            RegisterSystems();

            foreach (var sys in _systems)
            {
                sys.Init();
            }

            //create Players 
            int playerCount = 1;
            for (int i = 0; i < playerCount; i++)
            {
                var initPos = LVector2.zero;
                var player = CreateEntity<Player>(initPos);
            }

            var allPlayers = GetPlayers();
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

        public void Update()
        {
            if (IsPause)
            {
                return;
            }

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

        public GameObject LoadPrefab(PrefabType type)
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

        public void BindView(Entity entity, Entity oldEntity = null)
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
    }
}