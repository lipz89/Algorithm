using System.Collections.Generic;
using System.Linq;

namespace Eight.Core
{
    public abstract class Solver
    {
        protected readonly SolveLogic first;
        protected List<Direction> steps;
        protected List<int> queueHistory = new List<int>();
        protected int stateCount = 0;

        protected Solver(Logic logic)
        {
            first = new SolveLogic(logic);
        }

        public List<Direction> GetSteps()
        {
            return steps?.ToList();
        }

        public List<int> GetQueueHistory()
        {
            return queueHistory.ToList();
        }
        public int GetStateTotal()
        {
            return stateCount;
        }

        public abstract bool Run();
    }
}