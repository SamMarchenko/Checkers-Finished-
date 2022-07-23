using UnityEngine;
using UnityEngine.EventSystems;

namespace Cell
{
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ClickEventHandler(CellView component);
        public event ClickEventHandler OnCellClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            OnCellClick?.Invoke(this);
        }
    }
}