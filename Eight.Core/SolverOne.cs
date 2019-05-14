using System.Collections.Generic;

namespace Eight.Core
{
    public class SolverOne : Solver
    {
        public override bool Run()
        {
            var games = new List<SolveLogic> { first };
            var states = new HashSet<int> { first.GetState() };
            while (true)
            {
                if (games.Count == 0)
                {
                    stateCount = states.Count;
                    return false;
                }
                queueHistory.Add(games.Count);
                var newGames = new List<SolveLogic>();
                foreach (var game in games)
                {
                    foreach (var direction in Consts.Directions)
                    {
                        var g = game.TryMove(direction);
                        if (g == null)
                        {
                            continue;
                        }

                        var state = g.GetState();
                        if (states.Contains(state))
                        {
                            continue;
                        }
                        if (g.IsFinished())
                        {
                            queueHistory.Add(newGames.Count + 1);
                            steps = g.GetSteps();
                            stateCount = states.Count;
                            return true;
                        }

                        states.Add(state);
                        newGames.Add(g);
                    }
                }

                games = newGames;
            }
        }

        public SolverOne(Logic logic) : base(logic)
        {
        }
    }
}