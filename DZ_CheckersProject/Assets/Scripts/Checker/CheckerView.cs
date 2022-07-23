using UnityEngine;
using UnityEngine.EventSystems;

namespace Checker
{
    public class CheckerView : MonoBehaviour, IPointerClickHandler
    {
        public delegate void ClickEventHandler(CheckerView component);
        public event ClickEventHandler OnCheckerClick;
        public void OnPointerClick(PointerEventData eventData)
        {
            OnCheckerClick?.Invoke(this);
        }
    }
}