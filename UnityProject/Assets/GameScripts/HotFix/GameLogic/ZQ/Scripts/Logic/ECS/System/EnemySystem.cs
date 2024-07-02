using Lockstep.Framework;


namespace Lockstep.Game
{
    public class EnemySystem : IGameSystem
    {
        public EnemySystem(World world) : base(world) { }

        public override void Update(LFloat deltaTime)
        {
            foreach (var enemy in World.GetEnemies())
            {
                enemy.Update(deltaTime);
            }
        }
    }
}