namespace Eight.Core
{
    /// <summary> 棋局状态 </summary>
    public enum GameType
    {
        /// <summary> 顺序换行 </summary>
        WarpAsc,
        /// <summary> 顺序换行空位前置 </summary>
        WarpAscWithSpace,
        /// <summary> 逆序换行 </summary>
        WarpDesc,
        /// <summary> 逆序换行空位前置 </summary>
        WarpDescWithSpace,
        /// <summary> 顺序环形 </summary>
        LoopAsc,
        /// <summary> 顺序环形中位 </summary>
        LoopAscSkipOne,
        /// <summary> 逆序环形 </summary>
        LoopDesc,
        /// <summary> 逆序环形中位 </summary>
        LoopDescSkipOne,
        /// <summary> 随机 </summary>
        Random,
    }
}