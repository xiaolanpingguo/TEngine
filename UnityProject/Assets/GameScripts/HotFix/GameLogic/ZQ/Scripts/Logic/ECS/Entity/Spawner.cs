using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class Spawner : Entity
    {
        public SpawnerInfo Info = new SpawnerInfo();
        public LFloat Timer;

        public override void Start()
        {
            Timer = Info.spawnTime;
        }

        public override void Update(LFloat deltaTime)
        {
            Timer += deltaTime;
            if (Timer > Info.spawnTime)
            {
                Timer = LFloat.zero;
                Spawn();
            }
        }

        public void Spawn()
        {
            if (World.Instance.CurEnemyCount >= World.Instance.MaxEnemyCount)
            {
                return;
            }

            World.Instance.CurEnemyCount++;
            World.Instance.CreateEntity<Enemy>(Info.prefabId, Info.spawnPoint);
        }
    }
}