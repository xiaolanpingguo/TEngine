using System;
using Lockstep.Framework;


namespace Lockstep.Game
{
    [Serializable]
    public partial class Enemy : Entity
    {
        public CBrain brain = new CBrain();

        public override void BindRef()
        {
            base.BindRef();
            RegisterComponent(brain);
            moveSpd = 2;
            turnSpd = 150;
        }
    }
}