using Lockstep.Framework;


namespace Lockstep.Game
{
    public class SpawnerSystem : IGameSystem
    {
        private Spawner[] _spawners => World.Instance.GetSpawners();

        public override void Init()
        {
            for (int i = 0; i < 3; i++)
            {
                var configId = 100 + i;
                var config = GameConfigSingleton.Instance.GetEntityConfig(configId) as SpawnerConfig;
                World.Instance.CreateEntity<Spawner>(configId, config.entity.Info.spawnPoint);
            }

            foreach (var spawner in _spawners)
            {
                spawner.Start();
            }
        }

        public override void Update(LFloat deltaTime)
        {
            foreach (var spawner in _spawners)
            {
                spawner.Update(deltaTime);
            }
        }
    }
}