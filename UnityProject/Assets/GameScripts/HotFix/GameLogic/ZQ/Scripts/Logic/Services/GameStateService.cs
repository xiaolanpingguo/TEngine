using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameBase;
using Lockstep.Framework;
using TEngine;


namespace Lockstep.Game
{
    public class GameStateService : Singleton<GameStateService>
    {
        private Dictionary<int, GameState> _tick2State = new Dictionary<int, GameState>();
        private GameState _curGameState;
        private Dictionary<Type, IList> _type2Entities = new Dictionary<Type, IList>();
        private Dictionary<int, BaseEntity> _id2Entities = new Dictionary<int, BaseEntity>();
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

        public int MaxEnemyCount
        {
            get => _curGameState.MaxEnemyCount;
            set => _curGameState.MaxEnemyCount = value;
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

        private void AddEntity<T>(T e) where T : BaseEntity
        {
            if (typeof(T) == typeof(Player))
            {
                int i = 0;
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

        private void RemoveEntity<T>(T e) where T : BaseEntity
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

            hash += _entityIdCounter * PrimerLUT.GetPrimer(idx++);
            foreach (var entity in GetPlayers())
            {
                hash += entity.curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += entity.skillBox.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            }

            //_debugService.Trace($"GetPlayers hash {hash}",true);
            foreach (var entity in GetEnemies())
            {
                hash += entity.curHealth.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += entity.transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            }
            //_debugService.Trace($"GetEnemies hash {hash}",true);

            foreach (var entity in GetSpawners())
            {
                hash += entity.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            }
            //_debugService.Trace($"GetSpawners hash {hash}",true);

            hash += _curGameState.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            //_debugService.Trace($"GetHash hash {hash}",true);
            return hash;
        }

        public void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine("Hash ------: " + Hash);
            sb.AppendLine(prefix + "EntityIdCounter" + ":" + _entityIdCounter.ToString());
            BackUpUtil.DumpList("GetPlayers", GetPlayers(), sb, prefix);
            BackUpUtil.DumpList("GetEnemies", GetEnemies(), sb, prefix);
            BackUpUtil.DumpList("GetSpawners", GetSpawners(), sb, prefix);
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

        public T CreateEntity<T>(int prefabId, LVector3 position) where T : BaseEntity, new()
        {
            var baseEntity = new T();
            GameConfigSingleton.Instance.GetEntityConfig(prefabId)?.CopyTo(baseEntity);
            baseEntity.EntityId = GenId();
            baseEntity.PrefabId = prefabId;
            baseEntity.transform.Pos3 = position;
            SimulatorService.Instance.Trace($"CreateEntity {prefabId} pos {prefabId} entityId:{baseEntity.EntityId}");
            baseEntity.DoBindRef();
            if (baseEntity is Entity entity)
            {
                PhysicSystem.Instance.RegisterEntity(prefabId, entity);
            }

            baseEntity.DoAwake();
            baseEntity.DoStart();
            UnityGameViewService.Instance.BindView(baseEntity);
            AddEntity(baseEntity);
            return baseEntity;
        }

        public void DestroyEntity(BaseEntity entity)
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
                _id2Entities = new Dictionary<int, BaseEntity>();
                _type2Entities.Clear();

                //. Recover Entities
                RecoverEntities(new List<Player>(), reader);
                RecoverEntities(new List<Enemy>(), reader);
                RecoverEntities(new List<Spawner>(), reader);


                //. Rebind Views 
                foreach (var pair in _id2Entities)
                {
                    BaseEntity oldEntity = null;
                    if (oldId2Entity.TryGetValue(pair.Key, out var poldEntity))
                    {
                        oldEntity = poldEntity;
                        oldId2Entity.Remove(pair.Key);
                    }
                    UnityGameViewService.Instance.BindView(pair.Value, oldEntity);
                }

                //. Unbind Entity views
                foreach (var pair in oldId2Entity)
                {
                    UnityGameViewService.Instance.UnbindView(pair.Value);
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

        private void BackUpEntities<T>(T[] lst, Serializer writer) where T : BaseEntity, IBackup, new()
        {
            writer.Write(lst.Length);
            foreach (var item in lst)
            {
                item.WriteBackup(writer);
            }
        }

        private List<T> RecoverEntities<T>(List<T> lst, Deserializer reader) where T : BaseEntity, IBackup, new()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var t = new T();
                lst.Add(t);
                t.ReadBackup(reader);
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

        public struct GameState
        {
            public LFloat RemainTime;
            public LFloat DeltaTime;
            public int MaxEnemyCount;
            public int CurEnemyCount;
            public int CurEnemyId;

            public int GetHash(ref int idx)
            {
                int hash = 1;
                hash += CurEnemyCount.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += MaxEnemyCount.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                hash += CurEnemyId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
                return hash;
            }
        }
    }
}