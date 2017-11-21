using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace GPCommon
{
    public class DoubleClickEventTrigger : UIBehaviour, IPointerClickHandler, IPointerDownHandler
    {

        public float durationThreshold = 0.1f;
        public UnityEvent onDoubleClick = new UnityEvent();
        public bool doubleClickTriggered = false;

        private float lastClickTime;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (gameObject.GetComponent<LongPressEventTrigger>() != null && gameObject.GetComponent<LongPressEventTrigger>().longPressTriggered)
                return;

            if (Time.time - lastClickTime < durationThreshold)
            {
                onDoubleClick.Invoke();
                doubleClickTriggered = true;
            }
            else
            {
                lastClickTime = Time.time;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            doubleClickTriggered = false;
        }
    }
}