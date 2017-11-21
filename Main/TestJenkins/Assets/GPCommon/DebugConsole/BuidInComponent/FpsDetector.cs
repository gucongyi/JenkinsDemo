using UnityEngine;
using System.Collections;

namespace GPCommon
{
    public class FpsDetector : ConsoleComponent
    {
        private readonly float _frequency = 0.5f; // The update frequency of the fps
        private readonly int _nbDecimal = 1; // How many decimal do you want to display

        private float _accum = 0f; // FPS accumulated over the interval
        private int _frames = 0; // Frames drawn over the interval
        private string _sFps = ""; // The fps formatted into a string.

        private float _minFps = 9999f;
        private float _maxFps = 0f;

        public FpsDetector()
        {
            // Start recording fps
            DebugConsole.GetInstance().StartCoroutine(Fps());
        }

        public override void OnDrawing()
        {
            string fpsString = _sFps + " / L" + _minFps.ToString("#0.0") + " / H" + _maxFps.ToString("#0.0") + " FPS";
            GUIHelper.PrintShadowedText(fpsString, GUIHelper.RatioRect(0.005446143f, 0.9274625f, 0.5f, 0.06330022f));
        }

        public override void OnUpdate()
        {
            _accum += Time.timeScale / Time.deltaTime;
            _frames++;
        }

        IEnumerator Fps()
        {
            // Infinite loop executed every "frequency" seconds.
            while (true)
            {
                // Update the FPS
                float fps = _accum / _frames;

                if (fps < _minFps)
                    _minFps = fps;
                if (fps > _maxFps)
                    _maxFps = fps;

                _sFps = fps.ToString("f" + Mathf.Clamp(_nbDecimal, 0, 10));

                _accum = 0.0F;
                _frames = 0;

                yield return new WaitForSeconds(_frequency);
            }
        }
    }
}