using System;
using System.Linq;

namespace Eight.Core
{
    public class Logic
    {
        protected Logic()
        {
        }
        public Logic(GameType gameType = GameType.Random)
        {
            if (!Consts.States.Keys.Contains(gameType))
            {
                var rd = new Random(DateTime.Now.Millisecond);
                var index = rd.Next(Consts.States.Keys.Count);
                gameType = Consts.States.Keys.ToList()[index];
            }
            State = Consts.States[gameType];
            Value = State.ToCharArray();
            this.Shuffle();
            Empty = new string(Value).IndexOf('0');
        }
        public Logic(string state, string current)
        {
            State = state;
            Value = current.ToCharArray();
            Empty = current.IndexOf('0');
        }

        public string State { get; protected set; }
        public char[] Value { get; protected set; }
        public int Empty { get; protected set; }
        private void Shuffle()
        {
            var rd = new Random(DateTime.Now.Millisecond);
            do
            {
                var em = 0;
                for (int i = 0; i < this.Value.Length * 2 || (i - em) % 2 == 1; i++)
                {
                    var s = rd.Next(this.Value.Length);
                    int e;
                    if (s % 2 == 0)
                    {
                        e = s + 3;
                        if (e > 8)
                        {
                            e -= 6;
                        }
                    }
                    else
                    {
                        e = s + 1;
                        if (e % 3 == 0)
                        {
                            e -= 2;
                        }
                    }
                    if ((this.Value[s] - '0') * (this.Value[e] - '0') == 0)
                    {
                        em++;
                    }
                    var t = this.Value[s];
                    this.Value[s] = this.Value[e];
                    this.Value[e] = t;
                }
            } while (this.IsFinished());
        }
        private bool CanMove(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Empty < 6;
                case Direction.Down: return Empty > 2;
                case Direction.Left: return Empty % 3 < 2;
                case Direction.Right: return Empty % 3 > 0;
            }

            return false;
        }
        public bool Move(Direction direction)
        {
            if (!CanMove(direction))
            {
                return false;
            }

            int to;
            switch (direction)
            {
                case Direction.Up:
                    to = Empty + 3;
                    break;
                case Direction.Down:
                    to = Empty - 3;
                    break;
                case Direction.Left:
                    to = Empty + 1;
                    break;
                case Direction.Right:
                    to = Empty - 1;
                    break;
                default: return false;
            }
            char temp = Value[to];
            Value[Empty] = temp;
            Value[to] = '0';
            Empty = to;
            return true;
        }
        public bool IsFinished()
        {
            return this.State == new string(this.Value);
        }
        public int GetState()
        {
            return int.Parse(new string(Value));
        }

        public override string ToString()
        {
            return new string(Value);
        }
    }
}