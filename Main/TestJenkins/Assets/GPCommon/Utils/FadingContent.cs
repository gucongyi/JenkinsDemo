using UnityEngine;
using System;

namespace GPCommon
{

    public class FadingContent : MonoBehaviour
    {
        public Vector3 position_end;
        public Vector3 scale_end;

        public Vector3 position_begin;
        public Vector3 scale_begin;

        public AnimationCurve positionCurve;
        public AnimationCurve scaleCurve;
        public Action<float> onProgressUpdate;
        public Action onComplete;
        public Func<bool> isPause;

        public float duration;

        public bool TriggerOnEnable = false;

        [ContextMenu("SaveAsEndPosition")]
        public void SaveAsEndPosition()
        {
            position_end = transform.localPosition;
        }

        [ContextMenu("SaveAsBegin")]
        public void SaveAsBegin()
        {
            position_begin = transform.localPosition;
            scale_begin = transform.localScale;
        }

        private float progress = 0f;
        private float targetProgress = 0f;

        [ContextMenu("Trigger")]
        public void Trigger()
        {
            progress = 0;
            targetProgress = 1f;
            UpdateDisplay();
        }



        [ContextMenu("ReverseTrigger")]
        public void ReverseTrigger()
        {
            progress = 1;
            targetProgress = 0f;
        }

        private void OnEnable()
        {
            gameObject.transform.localPosition = position_begin;
            if (TriggerOnEnable)
            {
                Trigger();
                UpdateDisplay();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isPause != null && isPause())
                return;

            if (targetProgress > progress)
            {
                progress = Mathf.Min(progress + Time.deltaTime / duration, targetProgress);
                UpdateDisplay();
            }
            else if (targetProgress < progress)
            {
                progress = Mathf.Max(progress - Time.deltaTime / duration, targetProgress);
                UpdateDisplay();
            }
        }

        void OnDestroy()
        {
            isPause = null;
            onProgressUpdate = null;
            onComplete = null;
        }

        private void UpdateDisplay()
        {
            transform.localPosition =
                Vector3.LerpUnclamped(position_begin, position_end, positionCurve.Evaluate(progress));
            transform.localScale = Vector3.LerpUnclamped(scale_begin, scale_end, scaleCurve.Evaluate(progress));

            if (onProgressUpdate != null)
                onProgressUpdate(progress);

            if (progress == targetProgress && onComplete != null)
            {
                onComplete();
                onComplete = null;
            }
        }
    }
}