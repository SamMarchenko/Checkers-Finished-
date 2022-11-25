using System;
using System.Collections;
using System.Collections.Generic;
using Cell;
using Checker;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ClickController _clickController;
        [SerializeField] private int whiteCheckersCounter;
        [SerializeField] private int blackCheckersCounter;
        [SerializeField] private GameObject _camera;
        [SerializeField] private ObserverView _observerView;
        [SerializeField] private Image _inputBlockerImage;
        private Vector3 _whiteTurnCameraPos;
        private Vector3 _blackTurnCameraPos;
        private IObserverManager _observerManager;

        public Action EndMoveForObserver;


        private void Start()
        {
            SetCameraTurnPositions(_camera.transform.position);
            Subscribe();
            FillCheckerCounters();
            
            
            _observerManager = new ObserverManager(_observerView, this, _clickController, _inputBlockerImage);
            _observerManager.Subscribe();
            _observerManager.Start();
        }

        private void Subscribe()
        {
            _clickController.OnKillChecker += ClickControllerOnKillChecker;
            _clickController.OnChangeTurn += ClickControllerOnChangeTurn;
            _clickController.OnWinnerCell += ClickControllerOnWinnerCell;
        }
        private void Unsubscribe()
        {
            _clickController.OnKillChecker -= ClickControllerOnKillChecker;
            _clickController.OnChangeTurn -= ClickControllerOnChangeTurn;
            _clickController.OnWinnerCell -= ClickControllerOnWinnerCell;
        }
        
        private void SetCameraTurnPositions(Vector3 cameraStartPosition)
        {
            _whiteTurnCameraPos = cameraStartPosition;
            _blackTurnCameraPos = new Vector3(_whiteTurnCameraPos.x, _whiteTurnCameraPos.y, -1f *_whiteTurnCameraPos.z);
        }
        private void ClickControllerOnWinnerCell(ECheckerType checkertype)
        {
            _clickController.IsTeamWin = true;
            Debug.Log($"{checkertype} WIN!!!");
        }

        private void ClickControllerOnChangeTurn(ECheckerType checkertype)
        {
            if (checkertype == ECheckerType.Black)
            {
                _clickController.SetDirection(ECellsNeighbours.LeftTop,ECellsNeighbours.RightTop, ECellsNeighbours.LeftBot, ECellsNeighbours.RightBot);
                _clickController.TurnSide = ECheckerType.White;
                if (!_clickController.IsTeamWin)
                {
                    MoveCamera(_blackTurnCameraPos, _whiteTurnCameraPos);
                }
            }
            else
            {
                _clickController.SetDirection(ECellsNeighbours.RightBot,ECellsNeighbours.LeftBot, ECellsNeighbours.RightTop, ECellsNeighbours.LeftTop);
                _clickController.TurnSide = ECheckerType.Black;
                if (!_clickController.IsTeamWin)
                {
                    MoveCamera(_whiteTurnCameraPos, _blackTurnCameraPos);
                }
            }
        }

        private void ClickControllerOnKillChecker(ECheckerType checkertype)
        {
            if (checkertype == ECheckerType.Black)
            {
                blackCheckersCounter--;
                if (blackCheckersCounter == 0)
                {
                    _clickController.IsCanClick = false;
                    _clickController.IsTeamWin = true;
                    Debug.Log("White win!");
                }
            }
            else
            {
                whiteCheckersCounter--;
                if (whiteCheckersCounter == 0)
                {
                    _clickController.IsCanClick = false;
                    _clickController.IsTeamWin = true;
                    Debug.Log("Black win!");
                }
            }
        }

        private void FillCheckerCounters()
        {
            foreach (var clickControllerCheckerView in _clickController.CheckerViews)
            {
                if (clickControllerCheckerView.ECheckerType == ECheckerType.White)
                {
                    whiteCheckersCounter++;
                }
                else
                {
                    blackCheckersCounter++;
                }
            }
        }

        private void MoveCamera(Vector3 startPosition, Vector3 finishPosition)
        {
            StartCoroutine(MoveRoutine(startPosition, finishPosition, 1f));
        }

        private IEnumerator MoveRoutine(Vector3 startPosition, Vector3 finishPosition, float time)
        {
            yield return new WaitForSeconds(1f);
            var currentTime = 0f;
            while (currentTime < time)
            {
                _camera.transform.position =
                    Vector3.Lerp(startPosition, finishPosition, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            _camera.transform.position = finishPosition;
            _camera.transform.Rotate(0,180f,0, Space.World);
           _clickController.IsCanClick = true;
           
           //todo: Для наблюдателя
           EndMoveForObserver?.Invoke();
        }
        
        
        private void OnDisable()
        {
            Unsubscribe();
            _observerManager.Unsubscribe();
        }
    }
}