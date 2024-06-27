using System;
using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public class Spawner : Entity
    {
        public CSpawnerInfo Info = new CSpawnerInfo();
        public LFloat Timer;

        public override void Start()
        {
            Timer = Info.spawnTime;
            base.Start();
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