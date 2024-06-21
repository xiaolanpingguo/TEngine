using Lockstep.Framework;


namespace Lockstep.Game
{
    public class EnemySystem : IGameSystem
    {
        private Spawner[] Spawners => GameStateService.Instance.GetSpawners();
        private Enemy[] AllEnemy => GameStateService.Instance.GetEnemies();


        public override void Init()
        {
            for (int i = 0; i < 3; i++)
            {
                var configId = 100 + i;
                var config = GameConfigService.Instance.GetEntityConfig(configId) as SpawnerConfig;
                GameStateService.Instance.CreateEntity<Spawner>(configId, config.entity.Info.spawnPoint);
            }

            foreach (var spawner in Spawners)
            {
                spawner.DoStart();
            }
        }

        public override void Update(LFloat deltaTime)
        {
            foreach (var spawner in Spawners)
            {
                spawner.DoUpdate(deltaTime);
            }

            foreach (var enemy in AllEnemy)
            {
                enemy.DoUpdate(deltaTime);
            }
        }
    }
}