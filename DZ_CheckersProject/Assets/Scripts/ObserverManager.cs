using Cell;
using Checker;

namespace DefaultNamespace
{
    public class ObserverManager : IObserverManager
    {
       
        private readonly ObserverView _observerView;
        private readonly GameManager _gameManager;
        private readonly ClickController _clickController;
        private readonly MovingDataList _movingDataList = new MovingDataList();
        private MovingData _movingData;

        public bool isReplayModeOn => _observerView.IsReplayMode;

        public ObserverManager(ObserverView observerView, GameManager gameManager, ClickController clickController)
        {
            _observerView = observerView;
            _gameManager = gameManager;
            _clickController = clickController;
        }
        
        public void Subscribe()
        {
            _gameManager.EndMoveForObserver += EndMoveForObserver;
            _clickController.ClickedCellForMove += ClickedCellForMove;
            _clickController.ClickedCheckerForMove += ClickedCheckerForMove;
            _clickController.KillCheckerForObserver += KillCheckerForObserver;
        }

        private void EndMoveForObserver()
        {
            AddData(_movingData);
        }

        public void Unsubscribe()
        {
            _clickController.ClickedCellForMove -= ClickedCellForMove;
            _clickController.ClickedCheckerForMove -= ClickedCheckerForMove;
            _clickController.KillCheckerForObserver -= KillCheckerForObserver;
        }

        private void KillCheckerForObserver(CheckerView checker)
        {
            _movingData.KillingChecker = checker;
        }


        private void ClickedCheckerForMove(CheckerView checker)
        {
            _movingData = new MovingData();
            
            _movingData.Turn = checker.ECheckerType;
            _movingData.MovingChecker = checker;
        }

        private void ClickedCellForMove(CellView cell)
        {
            _movingData.Cell = cell;
        }

        public void AddData(MovingData data)
        {
            _movingDataList.DatasList.Add(data);
        }
    }
}