using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class SpawnerInfo : INeedBackup
    {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;
    }
}