using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GPCommon
{
    public interface IUIBgMaskListener
    {
        void OnMaskPress(bool isPress);
        void OnMaskClick();
    }

    public class BgMask : MonoBehaviour
    {
        protected IUIBgMaskListener Listener;
        public void SetListener(IUIBgMaskListener listener)
        {
            Listener = listener;
        }
    }
}