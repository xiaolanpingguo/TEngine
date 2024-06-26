using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class CSpawnerInfo : IComponent
    {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;


        public override void WriteBackup(Serializer writer)
        {
            writer.Write(prefabId);
            writer.Write(spawnPoint);
            writer.Write(spawnTime);
        }

        public override void ReadBackup(Deserializer reader)
        {
            prefabId = reader.ReadInt32();
            spawnPoint = reader.ReadLVector3();
            spawnTime = reader.ReadLFloat();
        }

        public override int GetHash(ref int idx)
        {
            int hash = 1;
            hash += prefabId.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += spawnPoint.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            hash += spawnTime.GetHash(ref idx) * PrimerLUT.GetPrimer(idx++);
            return hash;
        }

        public override void DumpStr(StringBuilder sb, string prefix)
        {
            sb.AppendLine(prefix + "prefabId" + ":" + prefabId.ToString());
            sb.AppendLine(prefix + "spawnPoint" + ":" + spawnPoint.ToString());
            sb.AppendLine(prefix + "spawnTime" + ":" + spawnTime.ToString());
        }
    }
}