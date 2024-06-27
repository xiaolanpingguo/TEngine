using Lockstep.Framework;
using UnityEngine;


namespace Lockstep.Game
{
    public class EnemyConfig : ScriptableObject
    {
        public int MaxHealth = 100;
        public int Damage = 10;
        public LFloat MoveSpd = 2;
        public LFloat TurnSpd = 150;
    }
}