using Lockstep.Framework;


namespace Lockstep.Game
{
    public class EnemySystem : IGameSystem
    {
        public override void Update(LFloat deltaTime)
        {
            foreach (var enemy in World.Instance.GetEnemies())
            {
                enemy.Update(deltaTime);
            }
        }
    }
}