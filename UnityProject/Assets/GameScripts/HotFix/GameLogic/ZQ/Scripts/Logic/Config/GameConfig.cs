using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;


namespace Lockstep.Game 
{
    public enum PrefabType
    {
        None,
        Player,
        Enemy,
        Spawner,
    }

    public class GameConfig : ScriptableObject 
    {
        public int MaxEnemyCount= 10;
        public string PlayerPrefabPath = "Prefabs/Player";
        public string EnemyPrefabPath = "Prefabs/Enemy";
        public string SpawnerPrefabPath = "Prefabs/Spawner";
        public string RecorderFilePath;
        public string DumpStrPath;

        public string GetPrefabPath(PrefabType type)
        {
            switch(type)
            {
                case PrefabType.Player: return PlayerPrefabPath;
                case PrefabType.Enemy: return EnemyPrefabPath;
                case PrefabType.Spawner: return SpawnerPrefabPath;
                default: return string.Empty;
            }
        }
    }
}