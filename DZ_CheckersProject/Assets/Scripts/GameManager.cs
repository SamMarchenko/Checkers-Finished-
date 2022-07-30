using System;
using System.Collections;
using System.Collections.Generic;
using Cell;
using Checker;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ClickController _clickController;
        [SerializeField] private int whiteCheckersCounter;
        [SerializeField] private int blackCheckersCounter;
        [SerializeField] private GameObject _camera;
        private Vector3 _whiteTurnCameraPos;
        private Vector3 _blackTurnCameraPos;


        private void Start()
        {
            SetCameraTurnPositions(_camera.transform.position);
            FillCheckerCounters();
            _clickController.OnKillChecker += ClickControllerOnOnKillChecker;
            _clickController.OnChangeTurn += ClickControllerOnOnChangeTurn;
            _clickController.OnWinnerCell += ClickControllerOnOnWinnerCell;
        }

        private void SetCameraTurnPositions(Vector3 cameraStartPosition)
        {
            _whiteTurnCameraPos = cameraStartPosition;
            _blackTurnCameraPos = new Vector3(_whiteTurnCameraPos.x, _whiteTurnCameraPos.y, -1f *_whiteTurnCameraPos.z);
        }
        private void ClickControllerOnOnWinnerCell(ECheckerType checkertype)
        {
            _clickController.IsTeamWin = true;
            Debug.Log($"{checkertype} WIN!!!");
        }

        private void ClickControllerOnOnChangeTurn(ECheckerType checkertype)
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

        private void ClickControllerOnOnKillChecker(ECheckerType checkertype)
        {
            if (checkertype == ECheckerType.Black)
            {
                blackCheckersCounter--;
                if (blackCheckersCounter == 0)
                {
                    Debug.Log("White win!");
                }
            }
            else
            {
                whiteCheckersCounter--;
                if (whiteCheckersCounter == 0)
                {
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
            yield return new WaitForSeconds(1.5f);
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
        }
    }
}