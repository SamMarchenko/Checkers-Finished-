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
    public class DataForJson
    {
        public string TurnName;
        public string MovingCheckerName;
        public string KillingCheckerName;
        public string MovingCellName;
    }

    [Serializable]
    public class MovingDataList
    {
        public List<MovingData> DatasList = new List<MovingData>(128);
        public List<DataForJson> DatasListForJSON = new List<DataForJson>(128);
    }
}