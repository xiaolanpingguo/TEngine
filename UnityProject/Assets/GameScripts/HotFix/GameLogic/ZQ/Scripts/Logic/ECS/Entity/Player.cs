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

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(mover);
        }

        public override void DoUpdate(LFloat deltaTime)
        {
            base.DoUpdate(deltaTime);
            if (input.skillId != 0)
            {
                Fire(input.skillId);
            }
        }
    }
}