using System;
using UnityEngine;

namespace GPCommon
{
    public class BaseListViewItem : MonoBehaviour
    {

        public int LineCount;
        public int ItemWidth;
        public int ItemHeight;

        public virtual void SetData(object data)
        {
            return;
        }
    }
}
