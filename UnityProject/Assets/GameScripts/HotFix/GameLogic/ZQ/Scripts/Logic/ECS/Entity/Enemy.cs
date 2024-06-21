using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class Enemy : Entity, IAfterBackup
    {
        public CBrain brain = new CBrain();

        protected override void BindRef()
        {
            base.BindRef();
            RegisterComponent(brain);
            moveSpd = 2;
            turnSpd = 150;
        }

        public void OnAfterDeserialize()
        {
        }
    }
}