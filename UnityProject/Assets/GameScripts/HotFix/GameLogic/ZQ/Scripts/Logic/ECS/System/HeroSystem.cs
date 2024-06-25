using Lockstep.Framework;


namespace Lockstep.Game
{
    public enum EColliderLayer
    {
        Static,
        Enemy,
        Hero,
        EnumCount
    }
    public class HeroSystem : IGameSystem
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