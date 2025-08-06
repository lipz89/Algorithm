using System.Collections.Generic;

namespace Eight.Core
{
    internal static class Consts
    {
        public static readonly Dictionary<GameType, string> States = new Dictionary<GameType, string>
        {
            [GameType.WarpAsc] = "123456780",//8
            [GameType.WarpAscWithSpace] = "012345678",//0
            [GameType.WarpDesc] = "876543210",//8+7+6+5+4+3+2+1=36
            [GameType.WarpDescWithSpace] = "087654321",//7+6+5+4+3+2+1=28
            [GameType.LoopAsc] = "123804765",//1+1+1+5+2+1=11
            [GameType.LoopAscSkipOne] = "812703654",//8+1+1+5+2+1=18
            [GameType.LoopDesc] = "876105234",//8+7+6+1+3=25
            [GameType.LoopDescSkipOne] = "187206345",//1+7+6+1+3=18
        };

        public static List<Direction> Directions = new List<Direction>
        {
            Direction.Down,
            Direction.Right,
            Direction.Left,
            Direction.Up,
        };
    }
}
