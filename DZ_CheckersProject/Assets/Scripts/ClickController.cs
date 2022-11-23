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
        private ECellsNeighbours _directionRightTop = ECellsNeighbours.RightTop;
        private ECellsNeighbours _directionLeftTop = ECellsNeighbours.LeftTop;
        private ECellsNeighbours _directionRightBot = ECellsNeighbours.RightBot;
        private ECellsNeighbours _directionLeftBot = ECellsNeighbours.LeftBot;

        [SerializeField] private CellView[] _cellViews;

        [SerializeField] private CheckerView[] _checkerViews;
        public CheckerView[] CheckerViews => _checkerViews;
        [SerializeField] private List<CheckerView> _checkersInAttackRange;

        [SerializeField] private Transform _startMovePosition;
        [SerializeField] private Transform _endMovePosition;
        [SerializeField] private bool _wantMove;
        [SerializeField] private bool _isCanClick = true;
        public bool IsCanClick
        {
            get { return _isCanClick;}
            set {_isCanClick = value;}
        }
        [SerializeField] private bool _isTeamWin;
        public bool IsTeamWin
        {
            get { return _isTeamWin;}
            set {_isTeamWin = value;}
        }

        [SerializeField] private Dictionary<ECellsNeighbours, CellView>
            CellsAndNeighbours = new Dictionary<ECellsNeighbours, CellView>();

        [SerializeField] private Dictionary<CheckerView, CellView> _pairCheckerCell;

        [SerializeField] private CellView _startCell;
        [SerializeField] private CellView _finishedCell;
        [SerializeField] private CellView _leftNeighbour;
        [SerializeField] private CellView _rightNeighbour;
        [SerializeField] private CheckerView _clickedChecker;
        [SerializeField] private CellView _commonNeighbour;
        [SerializeField] private ECheckerType _turnSide;
        public ECheckerType TurnSide
        {
            get { return _turnSide;}
            set {_turnSide = value ; }
        }
        private Coroutine _moveRoutine;
        
        public delegate void ClickEventHandler(ECheckerType checkerType);

        public event ClickEventHandler OnKillChecker;
        public event ClickEventHandler OnChangeTurn;
        public event ClickEventHandler OnWinnerCell;
        public Action<CheckerView> ClickedCheckerForMove;
        public Action<CheckerView> KillCheckerForObserver;
        public Action<CellView> ClickedCellForMove;


        private void Start()
        {
            FindAllNeighbours();

            foreach (var cell in _cellViews)
            {
                cell.OnCellClick += OnCellClick;
            }

            foreach (var checker in _checkerViews)
            {
                checker.OnCheckerClick += OnCheckerСlick;
            }
        }

        private void OnCellClick(CellView clickedCell)
        {
            if (!_wantMove)
            {
                _startMovePosition = null;
                _endMovePosition = null;
                _commonNeighbour = null;
                _startCell = null;
                _finishedCell = null;
                return;
            }

            foreach (var clickedCellMyNeighbour in clickedCell.MyNeighbours)
            {
                if (_startCell != clickedCellMyNeighbour.Value) continue;
                if (!IsFreeCell(clickedCell, out var pofig)) break;
                _endMovePosition = clickedCell.transform;
                _finishedCell = clickedCell;
                Move(_clickedChecker, _startCell, _finishedCell);
                return;
            }

            isValideCellForAttack(clickedCell);

            if (_commonNeighbour != null && !IsFreeCell(_commonNeighbour, out CheckerView checker))
            {
                for (int i = 0; i < _checkersInAttackRange.Count; i++)
                {
                    if (checker == _checkersInAttackRange[i])
                    {
                        KillChecker(checker, i);
                        Move(_clickedChecker, _startCell, clickedCell);
                    }
                }
            }
            
        }
        
        private void KillChecker(CheckerView checker, int index)
        {
            checker.transform.position += new Vector3(1000, 0, 1000);
            _checkersInAttackRange.Remove(_checkersInAttackRange[index]);
            
            OnKillChecker?.Invoke(checker.ECheckerType);
            
            //todo: Использует наблюдатель
            KillCheckerForObserver?.Invoke(checker);
        }

        private bool IsFreeCell(CellView clickedCell, out CheckerView checkerView)
        {
            var (xCell, zCell) = GetPositionToInt(clickedCell.transform.position);
            foreach (var checker in _checkerViews)
            {
                var (xChecker, zChecker) = GetPositionToInt(checker.transform.position);
                if (xCell == xChecker && zCell == zChecker)
                {
                    checkerView = checker;
                    return false;
                }
            }

            checkerView = null;
            return true;
        }

        private (int, int) GetPositionToInt(Vector3 FloatPosition)
        {
            return ((int) FloatPosition.x, (int) FloatPosition.z);
        }

        private void Move(CheckerView checkerView, CellView startCell, CellView finishCell)
        {
            if (_isTeamWin)
            {
                return;
            }
            _isCanClick = false;
            _moveRoutine = StartCoroutine(MoveRoutine(startCell.transform.position, finishCell.transform.position, 1f));
            _wantMove = false;
            SetNeighbourCells(finishCell);
            if (_leftNeighbour == null && _rightNeighbour == null)
            {
                OnWinnerCell?.Invoke(checkerView.ECheckerType);
            }
            OnChangeTurn?.Invoke(checkerView.ECheckerType);
            
            //todo: Для наблюдателя
            ClickedCellForMove?.Invoke(finishCell);
        }

        private IEnumerator MoveRoutine(Vector3 startPosition, Vector3 endPosition, float time)
        {
            //todo: тут костыль
            startPosition += new Vector3(0, 1, 0);
            endPosition += new Vector3(0, 1, 0);
            var currentTime = 0f;
            while (currentTime < time)
            {
                _clickedChecker.transform.position =
                    Vector3.Lerp(startPosition, endPosition, 1 - (time - currentTime) / time);
                currentTime += Time.deltaTime;
                yield return null;
            }

            _clickedChecker.transform.position = endPosition;
            ChangeCheckerMaterial(_clickedChecker, 0);
            
        }

        private void isValideCellForAttack(CellView clickedCell)
        {
            FindCommonNeighbours(clickedCell);
            SetNeighbourCells(clickedCell);
        }

        private void FindCommonNeighbours(CellView clickedCell)
        {
            if (_startCell.MyNeighbours.ContainsKey(_directionRightTop) &&
                clickedCell.MyNeighbours.ContainsKey(_directionLeftBot))
            {
                var _startCellRightTopNei = _startCell.MyNeighbours[_directionRightTop];
                var _clickedCellLeftBotNei = clickedCell.MyNeighbours[_directionLeftBot];
                if (_startCellRightTopNei == _clickedCellLeftBotNei)
                {
                    _commonNeighbour = _startCellRightTopNei;
                    return;
                }
            }

            if (_startCell.MyNeighbours.ContainsKey(_directionLeftTop) &&
                clickedCell.MyNeighbours.ContainsKey(_directionRightBot))
            {
                var _startCellLeftTopNei = _startCell.MyNeighbours[_directionLeftTop];
                var _clickedCellRightBotNei = clickedCell.MyNeighbours[_directionRightBot];
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


        private void OnCheckerСlick(CheckerView checkerView)
        {
            if (_turnSide != checkerView.ECheckerType)
            {
                Debug.Log($"Now is turn of another team: {_turnSide}");
                return;
            }   
            if (!_isCanClick) return;
            _checkersInAttackRange.Clear();
            _wantMove = true;
            if (_clickedChecker != null && _clickedChecker != checkerView)
            {
                ChangeCheckerMaterial(_clickedChecker,0);
            }
            _clickedChecker = checkerView;
            ChangeCheckerMaterial(_clickedChecker, 1);
            
           
            
            
            _commonNeighbour = null;

            var (xChecker, zChecker) = GetPositionToInt(checkerView.transform.position);

            foreach (var cell in _cellViews)
            {
                var (xCell, zCell) = GetPositionToInt(cell.transform.position);

                if (xChecker == xCell && zChecker == zCell)
                {
                    _startMovePosition = cell.transform;
                    _startCell = cell;
                    SetNeighbourCells(_startCell);
                    GetEnemieOnNeighbourCell(_leftNeighbour);
                    GetEnemieOnNeighbourCell(_rightNeighbour);
                    _endMovePosition = null;
                }
            }
            //todo: Для наблюдателя
            ClickedCheckerForMove?.Invoke(checkerView);
        }

        private void ChangeCheckerMaterial(CheckerView checker, int index)
        {
           var material = checker.GetComponent<MeshRenderer>();
           material.material = _clickedChecker.Materials[index];
        }
        public void SetDirection(ECellsNeighbours leftTop, ECellsNeighbours rightTop, ECellsNeighbours leftBot, ECellsNeighbours rightBot)
        {
            _directionLeftTop = leftTop;
            _directionRightTop = rightTop;
            _directionLeftBot = leftBot;
            _directionRightBot = rightBot;
        }
        private void SetNeighbourCells(CellView startCell)
        {
            _leftNeighbour = null;
            _rightNeighbour = null;

            if (startCell.MyNeighbours.ContainsKey(_directionLeftTop))
            {
                _leftNeighbour = startCell.MyNeighbours[_directionLeftTop];
            }

            if (startCell.MyNeighbours.ContainsKey(_directionRightTop))
            {
                _rightNeighbour = startCell.MyNeighbours[_directionRightTop];
            }
            
        }

        private void GetEnemieOnNeighbourCell(CellView cell)
        {
            if (cell != null)
            {
                if (!IsFreeCell(cell, out CheckerView checkerView))
                {
                    if (checkerView.ECheckerType != _clickedChecker.ECheckerType)
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
    }
}