using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Cell
{
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ClickEventHandler(CellView component);
        public event ClickEventHandler OnCellClick;

        [SerializeField] private Dictionary<CellsNeighbours, CellView> myNeighbours = new Dictionary<CellsNeighbours, CellView>();
        public Dictionary<CellsNeighbours, CellView> MyNeighbours 
        {
            get => myNeighbours;
            set => myNeighbours = value;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            OnCellClick?.Invoke(this);
        }

        public void PrintDictionary()
        {
            foreach (var keyValuePair in myNeighbours)
            {
                Debug.Log($"Current Cell {name}. Key = {keyValuePair.Key}, value {keyValuePair.Value}");
            }
        }
    }
}