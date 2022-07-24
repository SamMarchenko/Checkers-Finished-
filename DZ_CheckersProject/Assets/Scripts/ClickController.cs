using System;
using System.Collections.Generic;
using Cell;
using Checker;
using UnityEngine;

namespace DefaultNamespace
{
    public class ClickController : MonoBehaviour
    {
        [SerializeField] private CellView[] _cellViews;
        [SerializeField] private CheckerView[] _checkerViews;
        [SerializeField] private List<CellView> _cellNeighbors;

        [SerializeField] private Transform _startMovePosition;
        [SerializeField] private Transform _endMovePosition;
        [SerializeField] private bool _wantMove;

        [SerializeField]
        private Dictionary<CellsNeighbours, CellView> CellsAndNeighbours = new Dictionary<CellsNeighbours, CellView>();

        [SerializeField] private Dictionary<CheckerView, CellView> _pairCheckerCell;


        private void Start()
        {
            FindAllNeighbours();
            foreach (var cell in _cellViews)
            {
                cell.OnCellClick += PrintDictionary;
                cell.OnCellClick += FindMovingСoordinates;
            }

            foreach (var checker in _checkerViews)
            {
                checker.OnCheckerClick += OnChecker;
            }
        }

        //todo: пока не актуален
        private void GetNeighborsOld(CellView cellView)
        {
            _cellNeighbors.Clear();
            Debug.Log($"Clicked cell = {cellView.name}");
            var clickedCell = cellView.gameObject.transform;

            var rightTopNeighbor = clickedCell.position;
            var leftTopNeighbor = clickedCell.position;
            var rightBotNeighbor = clickedCell.position;
            var leftBotNeighbor = clickedCell.position;

            rightTopNeighbor += new Vector3(10f, 0, 10f);
            leftTopNeighbor += new Vector3(-10f, 0, 10f);
            rightBotNeighbor += new Vector3(10f, 0, -10f);
            leftBotNeighbor += new Vector3(-10f, 0, -10f);


            foreach (var cell in _cellViews)
            {
                if (cell.transform.position == rightTopNeighbor)
                {
                    _cellNeighbors.Add(cell);
                    Debug.Log($"Right top neighbor = {cell.name}");
                }

                if (cell.transform.position == leftTopNeighbor)
                {
                    _cellNeighbors.Add(cell);
                    Debug.Log($"Left top neighbor = {cell.name}");
                }

                if (cell.transform.position == rightBotNeighbor)
                {
                    _cellNeighbors.Add(cell);
                    Debug.Log($"Right bot neighbor = {cell.name}");
                }

                if (cell.transform.position == leftBotNeighbor)
                {
                    _cellNeighbors.Add(cell);
                    Debug.Log($"Left bot neighbor = {cell.name}");
                }
            }
        }

        private void FindMovingСoordinates(CellView cellView)
        {
            if (!_wantMove)
            {
                _startMovePosition = null;
                _endMovePosition = null;
                return;
            }

            _wantMove = false;
            foreach (var cell in _cellNeighbors)
            {
                if (cellView == cell)
                {
                    _endMovePosition = cell.transform;

                    return;
                }
            }

            _startMovePosition = null;
        }

        private void OnChecker(CheckerView checkerView)
        {
            _wantMove = true;
            Debug.Log("Test");
            var xChecker = (int) checkerView.transform.position.x;
            var zChecker = (int) checkerView.transform.position.z;

            foreach (var cell in _cellViews)
            {
                var xCell = (int) cell.transform.position.x;
                var zCell = (int) cell.transform.position.z;

                if (xChecker == xCell && zChecker == zCell)
                {
                    _startMovePosition = cell.transform;
                    _endMovePosition = null;
                    Debug.Log("In if");
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
                    cellView.MyNeighbours.Add(CellsNeighbours.RightTop, cell);
                }

                if (IsEqualXZ(positionData.LeftTopNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(CellsNeighbours.LeftTop, cell);
                }

                if (IsEqualXZ(positionData.RightBotNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(CellsNeighbours.RightBot, cell);
                }

                if (IsEqualXZ(positionData.LeftBotNeighbor, x, z))
                {
                    cellView.MyNeighbours.Add(CellsNeighbours.LeftBot, cell);
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