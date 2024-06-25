using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Spawner : Entity
    {
        public SpawnerInfo Info = new SpawnerInfo();
        public LFloat Timer;

        public override void Start()
        {
            Timer = Info.spawnTime;
        }

        public override void Update(LFloat deltaTime)
        {
            Timer += deltaTime;
            if (Timer > Info.spawnTime)
            {
                Timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn()
        {
            if (World.Instance.CurEnemyCount >= World.Instance.MaxEnemyCount)
            {
                return;
            }

            World.Instance.CurEnemyCount++;
            World.Instance.CreateEntity<Enemy>(Info.prefabId, Info.spawnPoint);
        }

        public override void WriteBackup(Serializer writer)
        {
            writer.Write(EntityId);
            writer.Write(PrefabId);
            writer.Write(Timer);
            Info.WriteBackup(writer);
            transform.WriteBackup(writer);
        }

        public override void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
            Timer = reader.ReadLFloat();
            Info.ReadBackup(reader);
            transform.ReadBackup(reader);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Timer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Info.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += transform.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + PrefabId.ToString());
            sb.AppendLine(prefix + "Timer" + ":" + Timer.ToString());
            sb.AppendLine(prefix + "Info" + ":"); Info.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); transform.DumpStr(sb, "\t" + prefix);
        }
    }
}