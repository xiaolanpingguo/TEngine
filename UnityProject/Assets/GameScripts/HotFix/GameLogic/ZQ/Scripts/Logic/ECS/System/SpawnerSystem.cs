using Lockstep.Framework;
using System.Collections.Generic;


namespace Lockstep.Game
{
    public class SpawnerSystem : IGameSystem
    {
        private Spawner[] _spawners => World.GetSpawners();

        public SpawnerSystem(World world) : base(world) { }

        public override void Init()
        {
            var config = GameConfigSingleton.Instance.SpawnerConfig;
            foreach(var data in config.Spawners) 
            {
                CEnemySpawner enemySpawner = new CEnemySpawner();
                enemySpawner.SpawnPoint = data.spawnPoint;
                enemySpawner.SpawnTime = data.spawnTime;
                List<IComponent> components = new List<IComponent>() { enemySpawner };
                World.CreateEntity<Spawner>(data.spawnPoint, components);
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