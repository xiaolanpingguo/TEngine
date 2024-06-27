using UnityEngine;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class SpawnerConfig : ScriptableObject
    {
        public LFloat spawnTime = 0;
        public LVector3 spawnPoint;
        public int prefabId = 0;
    }
}