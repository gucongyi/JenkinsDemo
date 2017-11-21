using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GPCommon;
using UnityEngine.SceneManagement;

namespace GPCommon
{
    public class SceneHashDisplay : ConsoleComponent
    {

        private string _display = "";
        public void Update()
        {
#if UNITY_EDITOR
            Scene s = SceneManager.GetActiveScene();

            string path = CommonUtils.GetFullPath(s.path);
            string hashCode = CommonUtils.ComputeMD5(path);

            // Just display last 3 characters
            _display = string.Format("{0}:{1}", s.name, hashCode.Substring(hashCode.Length - 3, 3));
#endif
        }

        public override void OnDrawing()
        {
            GUIHelper.PrintShadowedText(_display, GUIHelper.RatioRect(0.90446143f, 0.0004625f, 0.5f, 0.06330022f));
        }
    }
}