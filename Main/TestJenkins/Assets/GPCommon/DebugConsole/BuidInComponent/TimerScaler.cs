using UnityEngine;
using System.Collections;

namespace GPCommon
{
    public class TimerScaler : ConsoleComponent
    {
        private float _timeScale = 1f;

        public override void OnDrawing()
        {
            GUIStyle sliderStyle = new GUIStyle(GUI.skin.horizontalSlider) {fixedHeight = Screen.height * 0.03f};
            GUIStyle thumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb) {fixedHeight = Screen.height * 0.03f};

            _timeScale = GUI.HorizontalSlider(GUIHelper.RatioRect(0.1522538f, 0.8427167f, 0.7863817f, 0.1159396f),
                _timeScale, 0, 3.0f, sliderStyle, thumbStyle);

            Time.timeScale = _timeScale;
        }
    }
}