using GameBase;
using UnityEngine;


namespace Lockstep.Game
{
    public class GameConfigSingleton : Singleton<GameConfigSingleton>
    {
        public GameConfig GameConfig {  get; private set; }
        public EnemyConfig EnemyConfig { get; private set; }
        public PlayerConfig PlayerConfig { get; private set; }
        public SpawnerConfig SpawnerConfig { get; private set; }
        public CollisionConfig CollisionConfig { get; private set; }
        public SkillConfig SkillConfig { get; private set; }

        private string _configPath = "Config";
        private string _gameConfigName = "GameConfig";
        private string _enemyConfigName = "EnemyConfig";
        private string _playerConfigName = "PlayerConfig";
        private string _spawnerConfigName = "SpawnerConfig";
        private string _collisionConfigName = "CollisionConfig";
        private string _skillConfigName = "SkillConfig";


        protected override void Init()
        {
            GameConfig = Resources.Load<GameConfig>($"{_configPath}/{_gameConfigName}");
            EnemyConfig = Resources.Load<EnemyConfig>($"{_configPath}/{_enemyConfigName}");
            PlayerConfig = Resources.Load<PlayerConfig>($"{_configPath}/{_playerConfigName}");
            SpawnerConfig = Resources.Load<SpawnerConfig>($"{_configPath}/{_spawnerConfigName}");
            CollisionConfig = Resources.Load<CollisionConfig>($"{_configPath}/{_collisionConfigName}");
            SkillConfig = Resources.Load<SkillConfig>($"{_configPath}/{_skillConfigName}");
        }
    }
}