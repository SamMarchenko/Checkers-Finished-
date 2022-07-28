using System;
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


        private void Start()
        {
            FillCheckerCounters();
            _clickController.OnKillChecker += ClickControllerOnOnKillChecker;
            _clickController.OnChangeTurn += ClickControllerOnOnChangeTurn;
            _clickController.OnWinnerCell += ClickControllerOnOnWinnerCell;
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
            }
            else
            {
                //todo Может надо поменять местами лево-право
                _clickController.SetDirection(ECellsNeighbours.RightBot,ECellsNeighbours.LeftBot, ECellsNeighbours.RightTop, ECellsNeighbours.LeftTop);
                _clickController.TurnSide = ECheckerType.Black;
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
    }
}