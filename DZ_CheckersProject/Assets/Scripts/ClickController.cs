using System;
using System.Collections;
using System.Collections.Generic;
using Cell;
using Checker;
using UnityEngine;

namespace DefaultNamespace
{
    public class ClickController : MonoBehaviour
    {
        private ECellsNeighbours _directionRight = ECellsNeighbours.RightTop;
        private ECellsNeighbours _directionLeft = ECellsNeighbours.LeftTop;

        [SerializeField] private CellView[] _cellViews;

        [SerializeField] private CheckerView[] _checkerViews;
        [SerializeField] private List<CheckerView> _checkersInAttackRange;

        [SerializeField] private Transform _startMovePosition;
        [SerializeField] private Transform _endMovePosition;
        [SerializeField] private bool _wantMove;
        [SerializeField] private bool _isCanAttack;

        [SerializeField]
        private Dictionary<ECellsNeighbours, CellView>
            CellsAndNeighbours = new Dictionary<ECellsNeighbours, CellView>();

        [SerializeField] private Dictionary<CheckerView, CellView> _pairCheckerCell;

        [SerializeField] private CellView _startCell;
        [SerializeField] private CellView _finishedCell;
        [SerializeField] private CellView LeftNeighbour;
        [SerializeField] private CellView RightNeighbour;
        [SerializeField] private CheckerView _clickedChecker;
        [SerializeField] private CheckerView _attackedChecker;
        [SerializeField] private CellView _commonNeighbour;
        private Coroutine _moveRoutine;


        private void Start()
        {
            FindAllNeighbours();

            foreach (var cell in _cellViews)
            {
                cell.OnCellClick += PrintDictionary;
                cell.OnCellClick += OnCellClick;
            }

            foreach (var checker in _checkerViews)
            {
                checker.OnCheckerClick += OnChecker;
            }
        }

        private void OnCellClick(CellView clickedCell)
        {
            if (!_wantMove)
            {
                _startMovePosition = null;
                _endMovePosition = null;
                _commonNeighbour = null;
                return;
            }

            foreach (var clickedCellMyNeighbour in clickedCell.MyNeighbours)
            {
                //todo: дергать метод валидная ли клетка для атаки

                if (_startCell != clickedCellMyNeighbour.Value) continue;
                if (!IsFreeCell(clickedCell, out var pofig)) break;
                _endMovePosition = clickedCell.transform;
                Move(_clickedChecker, _startCell, clickedCell);
                return;
            }

            isValideCellForAttack(clickedCell);

            if (_commonNeighbour != null && !IsFreeCell(_commonNeighbour, out CheckerView checker))
            {
                for (int i = 0; i < _checkersInAttackRange.Count; i++)
                {
                    if (checker == _checkersInAttackRange[i])
                    {
                        checker.transform.position += new Vector3(100, 0, 100);
                        _checkersInAttackRange.Remove(_checkersInAttackRange[i]);
                        Move(_clickedChecker, _startCell, clickedCell);
                        //todo: добавить лок тапов во время анимации перемещения
                    }
                }
            }
        }

        private void isValideCellForAttack(CellView clickedCell)
        {
            FindCommonNeighbours(clickedCell);
            GetNeighbourCells(clickedCell);
        }

        private void FindCommonNeighbours(CellView clickedCell)
        {
            if (_startCell.MyNeighbours.ContainsKey(ECellsNeighbours.RightTop) &&
                clickedCell.MyNeighbours.ContainsKey(ECellsNeighbours.LeftBot))
            {
                var _startCellRightTopNei = _startCell.MyNeighbours[ECellsNeighbours.RightTop];
                var _clickedCellLeftBotNei = clickedCell.MyNeighbours[ECellsNeighbours.LeftBot];
                if (_startCellRightTopNei == _clickedCellLeftBotNei)
                {
                    _commonNeighbour = _startCellRightTopNei;
                    return;
                }
            }

            if (_startCell.MyNeighbours.ContainsKey(ECellsNeighbours.LeftTop) &&
                clickedCell.MyNeighbours.ContainsKey(ECellsNeighbours.RightBot))
            {
                var _startCellLeftTopNei = _startCell.MyNeighbours[ECellsNeighbours.LeftTop];
                var _clickedCellRightBotNei = clickedCell.MyNeighbours[ECellsNeighbours.RightBot];
                if (_startCellLeftTopNei == _clickedCellRightBotNei)
                {
                    _commonNeighbour = _startCellLeftTopNei;
                }
            }
            else
            {
                _commonNeighbour = null;
                _wantMove = false;
            }
        }

        private void Move(CheckerView checkerView, CellView startCell, CellView finishCell)
        {
            _moveRoutine = StartCoroutine(MoveRoutine(startCell.transform.position, finishCell.transform.position, 1f));
            Debug.Log($"Moving {checkerView} from {startCell} to {finishCell}");
            _wantMove = false;
        }
        private IEnumerator MoveRoutine(Vector3 startPosition, Vector3 endPosition, float time)
        {
            var currentTime = 0f;
            while (currentTime < time)
            {
                _clickedChecker.transform.position = Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }
            _clickedChecker.transform.position = endPosition;
        }

        private bool IsFreeCell(CellView clickedCell, out CheckerView checkerView)
        {
            var (xCell, zCell) = GetPositionToInt(clickedCell.transform.position);
            foreach (var checker in _checkerViews)
            {
                var (xChecker, zChecker) = GetPositionToInt(checker.transform.position);
                if (xCell == xChecker && zCell == zChecker)
                {
                    Debug.Log($"The cell {clickedCell.name} is occupied by checker {checker.name}");
                    checkerView = checker;
                    return false;
                }
            }

            checkerView = null;
            return true;
        }
        

        private void OnChecker(CheckerView checkerView)
        {
            _checkersInAttackRange.Clear();
            _wantMove = true;
            _clickedChecker = checkerView;
            _commonNeighbour = null;

            var (xChecker, zChecker) = GetPositionToInt(checkerView.transform.position);

            foreach (var cell in _cellViews)
            {
                var (xCell, zCell) = GetPositionToInt(cell.transform.position);

                if (xChecker == xCell && zChecker == zCell)
                {
                    _startMovePosition = cell.transform;
                    _startCell = cell;
                    GetNeighbourCells(_startCell);
                    GetEnemieOnNeighbourCell(LeftNeighbour);
                    GetEnemieOnNeighbourCell(RightNeighbour);
                    _endMovePosition = null;
                }
            }
        }

        private void GetNeighbourCells(CellView startCell)
        {
            LeftNeighbour = null;
            RightNeighbour = null;

            if (startCell.MyNeighbours.ContainsKey(_directionLeft))
            {
                LeftNeighbour = startCell.MyNeighbours[_directionLeft];
            }

            if (startCell.MyNeighbours.ContainsKey(_directionRight))
            {
                RightNeighbour = startCell.MyNeighbours[_directionRight];
            }
        }

        private void GetEnemieOnNeighbourCell(CellView cell)
        {
            if (cell != null)
            {
                if (!IsFreeCell(cell, out CheckerView checkerView))
                {
                    if (checkerView.ECheckerType == ECheckerType.Black)
                    {
                        _checkersInAttackRange.Add(checkerView);
                    }
                }
            }
        }

        private void FindAllNeighbours()
        {
            foreach (var cell in _cellViews)
            {
                cell.transform.position = new Vector3(cell.transform.position.x, 0f, cell.transform.position.z);
                SetNeighborsForCurrentCell(cell);
            }
        }

        private void SetNeighborsForCurrentCell(CellView cellView)
        {
            var positionData = GetNeighboursPositionData(cellView.transform.position);


            foreach (var cell in _cellViews)
            {
                var (x, z) = GetPositionToInt(cell.transform.position);

                if (IsEqualXZ(positionData.RightTopNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(ECellsNeighbours.RightTop, cell);
                }

                if (IsEqualXZ(positionData.LeftTopNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(ECellsNeighbours.LeftTop, cell);
                }

                if (IsEqualXZ(positionData.RightBotNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(ECellsNeighbours.RightBot, cell);
                }

                if (IsEqualXZ(positionData.LeftBotNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(ECellsNeighbours.LeftBot, cell);
                }
            }
        }

        private bool IsEqualXZ(IntPoint point, int x, int z)
        {
            return (x == point.X && z == point.Z);
        }

        private void PrintDictionary(CellView cellView)
        {
            cellView.PrintDictionary();
        }

        private CellPositionsData GetNeighboursPositionData(Vector3 cellViewPos)
        {
            var rightTopNeighbor = cellViewPos;
            var leftTopNeighbor = cellViewPos;
            var rightBotNeighbor = cellViewPos;
            var leftBotNeighbor = cellViewPos;

            rightTopNeighbor += new Vector3(10f, 0, 10f);
            var (xRTCell, zRTCell) = GetPositionToInt(rightTopNeighbor);

            leftTopNeighbor += new Vector3(-10f, 0, 10f);
            var (xLTCell, zLTCell) = GetPositionToInt(leftTopNeighbor);

            rightBotNeighbor += new Vector3(10f, 0, -10f);
            var (xRBCell, zRBCell) = GetPositionToInt(rightBotNeighbor);

            leftBotNeighbor += new Vector3(-10f, 0, -10f);
            var (xLBCell, zLBCell) = GetPositionToInt(leftBotNeighbor);

            return new CellPositionsData(new IntPoint(xRTCell, zRTCell), new IntPoint(xLTCell, zLTCell),
                new IntPoint(xRBCell, zRBCell), new IntPoint(xLBCell, zLBCell));
        }

        private (int, int) GetPositionToInt(Vector3 FloatPosition)
        {
            return ((int) FloatPosition.x, (int) FloatPosition.z);
        }
    }


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

    public class IntPoint
    {
        public int X;
        public int Z;

        public IntPoint(int x, int z)
        {
            X = x;
            Z = z;
        }
    }
}