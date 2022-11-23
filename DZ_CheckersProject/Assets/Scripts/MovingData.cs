using System;
using System.Collections.Generic;
using Cell;
using Checker;

namespace DefaultNamespace
{
    [Serializable]
    public class MovingData
    {
        public ECheckerType Turn;
        public CheckerView MovingChecker;
        public CheckerView KillingChecker;
        public CellView Cell;
    }

    [Serializable]
    public class MovingDataList
    {
        public List<MovingData> DatasList = new List<MovingData>(128);
    }
}