using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class Player : Entity, IAfterBackup
    {
        public int localId;
        public PlayerInput1 input = new PlayerInput1();
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

        public void OnAfterDeserialize()
        {
        }
    }
}