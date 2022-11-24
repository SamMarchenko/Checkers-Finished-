using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cell;
using Checker;
using Unity.IO;
using UnityEngine;

namespace DefaultNamespace
{
    public class ObserverManager : IObserverManager
    {
       
        private readonly ObserverView _observerView;
        private readonly GameManager _gameManager;
        private readonly ClickController _clickController;
        private readonly MovingDataList _movingDataList = new MovingDataList();
        private MovingData _movingData;

        private int _moveCounter = 0;

        public bool isReplayModeOn => _observerView.IsReplayMode;

        public ObserverManager(ObserverView observerView, GameManager gameManager, ClickController clickController)
        {
            _observerView = observerView;
            _gameManager = gameManager;
            _clickController = clickController;
        }

        public void Start()
        {
            if (isReplayModeOn)
            {
                Debug.Log("REPLAY mode ON!!!");
                ReadFromJson();
                ReplayGame();
            }
        }


        private void ReplayGame()
        {
            
            var checker = _movingDataList.DatasList[_moveCounter].MovingChecker;
            var cell = _movingDataList.DatasList[_moveCounter].Cell;
            
            MEC.Timing.CallDelayed(1f, () => _clickController.OnCheckerСlick(checker));

            MEC.Timing.CallDelayed(1f, () => _clickController.OnCellClick(cell));
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
            if (isReplayModeOn)
            {
                _moveCounter++;
                ReplayGame();
            }
            AddData(_movingData);
           // WriteToJSON(_movingData);
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
            WriteToJSON(_movingData);
        }

        public void AddData(MovingData data)
        {
            _movingDataList.DatasList.Add(data);
        }

        public void WriteToJSON(MovingData movingData)
        {
            if (isReplayModeOn)
            {
                return;
            }
            var data = ConvertDataForJson(movingData);
            
            _movingDataList.DatasListForJSON.Add(data);
            
           
            string moveToJson = JsonHelper.ToJson(_movingDataList.DatasListForJSON.ToArray(), true);
            
            Debug.Log(moveToJson);
            File.WriteAllText(Application.streamingAssetsPath + "/TestJSON.json", moveToJson);
        }

        public void ReadFromJson()
        {
           var data = File.ReadAllText(Application.streamingAssetsPath + "/TestJSON.json");
            DataForJson[] datasFromJson = JsonHelper.FromJson<DataForJson>(data);
            
            _movingDataList.DatasList.Clear();
            foreach (var item in datasFromJson)
            {
              _movingDataList.DatasList.Add(ConvertDataFromJson(item));
            }
        }
        

        private DataForJson ConvertDataForJson(MovingData movingData)
        {
            DataForJson dataForJson = new DataForJson();
            dataForJson.TurnName = movingData.Turn.ToString();
            dataForJson.MovingCellName = movingData.Cell.name;
            dataForJson.MovingCheckerName = movingData.MovingChecker.name;
            dataForJson.KillingCheckerName = movingData.KillingChecker?.name;

            return dataForJson;
        }
        

        private MovingData ConvertDataFromJson(DataForJson dataForJson)
        {

            MovingData movingData = new MovingData();
            
            movingData.MovingChecker = GetObject(_clickController.CheckerViews, dataForJson.MovingCheckerName);
            movingData.Cell = GetObject(_clickController.CellViews, dataForJson.MovingCellName);
            
            return movingData;
        }
        
        private T GetObject<T>(T[] array, string name) where T : MonoBehaviour
        {
            foreach (var element in array)
            {
                Debug.Log($" jsonName {name}; elementName {element.name}");
                if (element.name.Contains(name))
                {
                    return element;
                }
            }

            return null;
        }
    }
}