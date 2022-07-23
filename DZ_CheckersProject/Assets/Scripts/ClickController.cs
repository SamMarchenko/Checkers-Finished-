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


        private void Start()
        {
            foreach (var cell in _cellViews)
            {
                cell.OnCellClick += GetNeighbors;
            }

            foreach (var checker in _checkerViews)
            {
                checker.OnCheckerClick += OnChecker;
            }
        }
        private void GetNeighbors(CellView cellView)
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

        private void OnChecker(CheckerView checkerView)
        {
            Debug.Log("Test");
            var xChecker = (int)checkerView.transform.position.x;
            var zChecker = (int)checkerView.transform.position.z;
            //checkerView.transform.position = new Vector3(xChecker, 0, zChecker);
            
            foreach (var cell in _cellViews)
            {
                var xCell = (int)cell.transform.position.x;
                var zCell= (int)cell.transform.position.z;
                //cell.transform.position = new Vector3(xCell, 0, zCell);
                
                if (xChecker == xCell && zChecker == zCell)
                {
                    Debug.Log("In if");
                    GetNeighbors(cell);
                }
            }
        }
    }
}