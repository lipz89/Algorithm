using System.Collections.Generic;
using System.Linq;

namespace Eight.Core
{
    public class SolveLogic : Logic
    {
        public Direction? Step { get; private set; }
        public SolveLogic Last { get; private set; }
        public SolveLogic(Logic logic)
        {
            this.Empty = logic.Empty;
            this.State = logic.State;
            this.Value = logic.Value.ToArray();
            this.Last = logic as SolveLogic;
        }

        public SolveLogic TryMove(Direction direction)
        {
            var newMap = new SolveLogic(this) { Step = direction };
            if (newMap.Move(direction))
            {
                return newMap;
            }
            return null;
        }

        public List<Direction> GetSteps()
        {
            var list = new List<Direction>();
            var p = this;
            while (p.Step != null)
            {
                list.Insert(0, p.Step.Value);
                p = p.Last;
            }
            return list;
        }
    }
}
