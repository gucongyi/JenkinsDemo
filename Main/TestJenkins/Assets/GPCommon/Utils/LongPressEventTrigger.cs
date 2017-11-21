using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace GPCommon
{
    /// <summary>
    /// All you need to do is add the LongPressEventTrigger component to  your UI object that you want to long press. 
    /// Then, the onLongPress event will fire after pressing for the duration you specify.
    /// 
    /// This component can exist side-by-side with a Button component as well.So you can support long press as well as regular press.
    /// 
    /// </summary>
    public class LongPressEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Tooltip("How long must pointer be down on this object to trigger a long press")]
        public float durationThreshold = 1.0f;

        public UnityEvent onLongPress = new UnityEvent();

        private bool isPointerDown = false;
        public bool longPressTriggered = false;
        private float timePressStarted;


        private void Update()
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationThreshold)
                {
                    longPressTriggered = true;
                    onLongPress.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerDown = false;
        }
    }
}