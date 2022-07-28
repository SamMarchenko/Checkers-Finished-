namespace DefaultNamespace
{
    public class CellPositionsData
    {
        public IntPoint RightTopNeighbor;
        public IntPoint LeftTopNeighbor;
        public IntPoint RightBotNeighbor;
        public IntPoint LeftBotNeighbor;

        public CellPositionsData(IntPoint rightTopNeighbor, IntPoint leftTopNeighbor, IntPoint rightBotNeighbor,
            IntPoint leftBotNeighbor)
        {
            RightTopNeighbor = rightTopNeighbor;
            LeftTopNeighbor = leftTopNeighbor;
            RightBotNeighbor = rightBotNeighbor;
            LeftBotNeighbor = leftBotNeighbor;
        }
    }
}