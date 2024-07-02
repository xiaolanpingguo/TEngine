using UnityEngine;
using Lockstep.Framework;
using System;
using System.Collections.Generic;


namespace Lockstep.Game
{
    public class SpawnerConfig : ScriptableObject
    {
        [Serializable]
        public class SpawnerData
        {
            public LFloat spawnTime = 0;
            public LVector3 spawnPoint;
        }

        public List<SpawnerData> Spawners = new List<SpawnerData>();
    }
}