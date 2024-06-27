using GameBase;
using UnityEngine;


namespace Lockstep.Game
{
    public class GameConfigSingleton : Singleton<GameConfigSingleton>
    {
        public CollisionConfig CollisionConfig => _config.CollisionConfig;
        public string RecorderFilePath => _config.RecorderFilePath;
        public string DumpStrPath => _config.DumpStrPath;

        private GameConfig _config;
        public string configPath = "GameConfig";

        protected override void Init()
        {
            _config = Resources.Load<GameConfig>(configPath);
        }

        public EntityConfig GetEntityConfig(int id)
        {
            if (id >= 100)
            {
                return _config.GetSpawnerConfig(id - 100);
            }
            if (id >= 10)
            {
                return _config.GetEnemyConfig(id - 10);
            }

            return _config.GetPlayerConfig(id);
        }

        public SkillConfig GetSkillConfig()
        {
            return _config.GetSkillConfig();
        }
    }
}