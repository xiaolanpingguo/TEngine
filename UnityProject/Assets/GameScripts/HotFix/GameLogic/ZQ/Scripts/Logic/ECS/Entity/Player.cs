using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class Player : Entity
    {
        public int localId;
        public PlayerCommands input = new PlayerCommands();
        public CMover mover = new CMover();

        public override void BindRef()
        {
            base.BindRef();
            RegisterComponent(mover);
        }

        public override void Update(LFloat deltaTime)
        {
            base.Update(deltaTime);
            if (input.skillId != 0)
            {
                Fire(input.skillId);
            }
        }
    }
}