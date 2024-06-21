using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class SpawnerInfo : INeedBackup, IAfterBackup
    {
        public LFloat spawnTime;
        public LVector3 spawnPoint;
        public int prefabId;

        public void OnAfterDeserialize()
        {
        }
    }
}