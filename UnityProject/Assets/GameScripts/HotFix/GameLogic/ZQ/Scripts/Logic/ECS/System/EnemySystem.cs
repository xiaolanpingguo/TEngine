using Lockstep.Framework;


namespace Lockstep.Game
{
    public class EnemySystem : IGameSystem
    {
        private Spawner[] Spawners => World.Instance.GetSpawners();
        private Enemy[] AllEnemy => World.Instance.GetEnemies();


        public override void Init()
        {
            for (int i = 0; i < 3; i++)
            {
                var configId = 100 + i;
                var config = GameConfigSingleton.Instance.GetEntityConfig(configId) as SpawnerConfig;
                World.Instance.CreateEntity<Spawner>(configId, config.entity.Info.spawnPoint);
            }

            foreach (var spawner in Spawners)
            {
                spawner.Start();
            }
        }

        public override void Update(LFloat deltaTime)
        {
            foreach (var spawner in Spawners)
            {
                spawner.Update(deltaTime);
            }

            foreach (var enemy in AllEnemy)
            {
                enemy.Update(deltaTime);
            }
        }
    }
}