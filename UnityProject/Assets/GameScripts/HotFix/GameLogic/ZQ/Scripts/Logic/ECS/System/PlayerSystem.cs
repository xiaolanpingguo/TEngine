using Lockstep.Framework;


namespace Lockstep.Game
{
    public class PlayerSystem : IGameSystem
    {
        public PlayerSystem(World world) : base(world) { }

        public override void Update(LFloat deltaTime)
        {
            foreach (var player in World.GetPlayers())
            {
                player.Update(deltaTime);
            }
        }
    }
}