using System.Text;
using Lockstep.Framework;


namespace Lockstep.Game
{
    public class TraceLogSystem : IGameSystem
    {
        StringBuilder _dumpSb = new StringBuilder();

        public override void Update(LFloat deltaTime)
        {
            _dumpSb.AppendLine("Tick: " + World.Instance.Tick);
            //trace input
            //foreach (var input in World.Instance.PlayerInputs) {
            //    DumpInput(input);
            //}

            foreach (var entity in GameStateService.Instance.GetPlayers())
            {
                DumpEntity(entity);
            }

            foreach (var entity in GameStateService.Instance.GetEnemies())
            {
                //dumpSb.Append(" " + entity.timer);
                DumpEntity(entity);
            }

            //_debugService.Trace(_dumpSb.ToString(), true);
            _dumpSb.Clear();
        }


        private void DumpEntity(BaseEntity entity)
        {
            _dumpSb.Append("    ");
            _dumpSb.Append(" " + entity.EntityId);
            _dumpSb.Append(" " + entity.transform.Pos3);
            _dumpSb.Append(" " + entity.transform.deg);
            _dumpSb.AppendLine();
        }
    }
}