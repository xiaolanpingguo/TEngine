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
            base.Start();
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
            LTrans2D.WriteBackup(writer);
        }

        public override void ReadBackup(Deserializer reader)
        {
            EntityId = reader.ReadInt32();
            PrefabId = reader.ReadInt32();
            Timer = reader.ReadLFloat();
            Info.ReadBackup(reader);
            LTrans2D.ReadBackup(reader);
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += EntityId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += PrefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Timer.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += Info.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += LTrans2D.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "EntityId" + ":" + EntityId.ToString());
            sb.AppendLine(prefix + "PrefabId" + ":" + PrefabId.ToString());
            sb.AppendLine(prefix + "Timer" + ":" + Timer.ToString());
            sb.AppendLine(prefix + "Info" + ":"); Info.DumpStr(sb, "\t" + prefix);
            sb.AppendLine(prefix + "transform" + ":"); LTrans2D.DumpStr(sb, "\t" + prefix);
        }
    }
}