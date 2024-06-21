using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class Spawner : BaseEntity, IAfterBackup
    {
        public SpawnerInfo Info = new SpawnerInfo();
        public LFloat Timer;

        public override void DoStart()
        {
            Timer = Info.spawnTime;
        }

        public override void DoUpdate(LFloat deltaTime)
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
            if (GameStateService.Instance.CurEnemyCount >= GameStateService.Instance.MaxEnemyCount)
            {
                return;
            }

            GameStateService.Instance.CurEnemyCount++;
            GameStateService.Instance.CreateEntity<Enemy>(Info.prefabId, Info.spawnPoint);
        }

        public void OnAfterDeserialize()
        {
        }
    }
}