using System;
using System.Collections.Generic;
using Cell;
using Checker;
using UnityEngine;

namespace DefaultNamespace
{
    public class ClickController : MonoBehaviour
    {
        private ECellsNeighbours _directionRight = ECellsNeighbours.RightTop;
        private ECellsNeighbours _directionLeft = ECellsNeighbours.RightBot;

        [SerializeField] private CellView[] _cellViews;

        [SerializeField] private CheckerView[] _checkerViews;
        //[SerializeField] private List<CellView> _cellNeighbors;

        [SerializeField] private Transform _startMovePosition;
        [SerializeField] private Transform _endMovePosition;
        [SerializeField] private bool _wantMove;
        [SerializeField] private bool _isCanAttack;

        [SerializeField]
        private Dictionary<ECellsNeighbours, CellView> CellsAndNeighbours = new Dictionary<ECellsNeighbours, CellView>();

        [SerializeField] private Dictionary<CheckerView, CellView> _pairCheckerCell;

        private CellView _startCell;
        private CellView _finishedCell;

        private CheckerView _clickedChecker;


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
                return;
            }
            foreach (var clickedCellMyNeighbour in clickedCell.MyNeighbours)
            {
                if (_startCell != clickedCellMyNeighbour.Value) continue;
                if (!IsFreeCell(clickedCell, out var pofig)) break;
                _endMovePosition = clickedCell.transform;
                Move(_clickedChecker);
            }
            
            
            
            
        }

        private void isValideCellForAttack(CellView clickedCell)
        {
            //todo: нужно написать метод определения валидной для атаки клетки
        }

        private void Move(CheckerView checkerView)
        {
            //todo: нужна корутина движения

            _wantMove = false;
            // if (!IsFreeCell(clickedCell, out var pofig))
            // {
            //     _startMovePosition = null;
            //     return;
            // }
            
            
            _startMovePosition = null;
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

        // private bool IsTryToEatEnemy(CellView clickedCell)
        // {
        //     foreach (var neighbourForStartCell in _startCell.MyNeighbours)
        //     {
        //         if (clickedCell == neighbourForStartCell.Value)
        //         {
        //             return false;
        //         }
        //     }
        //
        //     // if (expr)
        //     // {
        //     //     
        //     // }
        // }

        private void OnChecker(CheckerView checkerView)
        {
            _wantMove = true;
            _clickedChecker = checkerView;
            
            var (xChecker, zChecker) = GetPositionToInt(checkerView.transform.position);

            foreach (var cell in _cellViews)
            {
                var (xCell, zCell) = GetPositionToInt(cell.transform.position);

                if (xChecker == xCell && zChecker == zCell)
                {
                    _startMovePosition = cell.transform;
                    _startCell = cell;
                    GetEnemiesInNeighbourCells(_startCell);
                    _endMovePosition = null;
                }
            }
        }

        private void GetEnemiesInNeighbourCells(CellView startCell)
        {
            CellView LeftNeighbour = null;
            CellView RightNeighbour = null;
            
                if (startCell.MyNeighbours.ContainsKey(_directionLeft))
                {
                    LeftNeighbour = startCell.MyNeighbours[_directionLeft];
                }

                if (startCell.MyNeighbours.ContainsKey(_directionRight))
                {
                    RightNeighbour = startCell.MyNeighbours[_directionRight];
                }
                
            if (LeftNeighbour != null)
            {
                if (!IsFreeCell(LeftNeighbour, out CheckerView checkerView))
                {
                    if (checkerView.ECheckerType == ECheckerType.Black)
                    {
                        Debug.Log("Is enemy on left top neighbour");
                        if (IsFreeCell(LeftNeighbour.MyNeighbours[_directionLeft], out var pofig))
                        {
                            _isCanAttack = true;
                        }
                    }
                }
            }
            
            if (RightNeighbour != null)
            {
                if (!IsFreeCell(RightNeighbour, out CheckerView checkerView))
                {
                    if (checkerView.ECheckerType == ECheckerType.Black)
                    {
                        Debug.Log("Is enemy on left top neighbour");
                        if (IsFreeCell(LeftNeighbour.MyNeighbours[ECellsNeighbours.LeftTop], out var pofig))
                        {
                            _isCanAttack = true;
                        }
                    }
                }
            }

            if (RightNeighbour != null)
            {
                if (!IsFreeCell(RightNeighbour, out CheckerView checkerView))
                {
                    if (checkerView.ECheckerType == ECheckerType.Black)
                    {
                        Debug.Log("Is enemy on right top neighbour");
                    }
                }
            }
        }

        private void FindAllNeighbours()
        {
            foreach (var cell in _cellViews)
            {
                cell.transform.position = new Vector3(cell.transform.position.x, 0f, cell.transform.position.z);
                GetNeighborsForCurrentCell(cell);
            }
        }

        private void GetNeighborsForCurrentCell(CellView cellView)
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