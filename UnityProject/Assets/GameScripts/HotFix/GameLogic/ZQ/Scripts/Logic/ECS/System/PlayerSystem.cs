using Lockstep.Framework;


namespace Lockstep.Game
{
    public class PlayerSystem : IGameSystem
    {
        public override void Update(LFloat deltaTime)
        {
            foreach (var player in World.Instance.GetPlayers())
            {
                player.Update(deltaTime);
            }
        }
    }
}