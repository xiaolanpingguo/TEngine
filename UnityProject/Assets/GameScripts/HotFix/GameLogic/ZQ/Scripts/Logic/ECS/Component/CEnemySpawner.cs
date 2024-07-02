using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class CEnemySpawner : IComponent
    {
        private int _maxEnemyCount = 0;
        private LFloat _timer = LFloat.zero;
        public LFloat SpawnTime;
        public LVector3 SpawnPoint;

        public override void Start()
        {
            _maxEnemyCount = GameConfigSingleton.Instance.GameConfig.MaxEnemyCount;
        }

        public override void Update(LFloat deltaTime)
        {
            _timer += deltaTime;
            if (_timer <= SpawnTime)
            {
                return;
            }

            _timer = LFloat.zero;
            if (World.Instance.CurEnemyCount >= _maxEnemyCount)
            {
                return;
            }

            World.Instance.CurEnemyCount++;
            World.Instance.CreateEntity<Enemy>(SpawnPoint);
        }

        public override void Serialize(Serializer writer)
        {
            writer.Write(SpawnPoint);
            writer.Write(SpawnTime);
        }

        public override void Deserialize(Deserializer reader)
        {
            SpawnPoint = reader.ReadLVector3();
            SpawnTime = reader.ReadLFloat();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += SpawnPoint.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += SpawnTime.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "spawnPoint" + ":" + SpawnPoint.ToString());
            sb.AppendLine(prefix + "spawnTime" + ":" + SpawnTime.ToString());
        }
    }
}