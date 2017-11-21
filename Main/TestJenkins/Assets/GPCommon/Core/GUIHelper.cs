using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


namespace GPCommon
{
    public static class GUIHelper
    {
        private static Texture2D _fullScreenBlackTex;

        public static Texture2D FullScreenBlackTex
        {
            get
            {
                if (_fullScreenBlackTex == null)
                    _fullScreenBlackTex = MakeTex(Screen.width, Screen.height, Color.black);

                return _fullScreenBlackTex;
            }
        }

        public static void PrintShadowedText(string text, Rect rect)
        {
            GUI.color = Color.black;
            GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), text);
            GUI.color = Color.white;
            GUI.Label(rect, text);
        }

        public static void PrintScrollText(string text, Rect rect, ref Vector2 scrollPosition)
        {
            GUILayout.BeginArea(rect);

            GUIStyle scrollViewStyle = new GUIStyle(GUI.skin.scrollView) {alignment = TextAnchor.UpperLeft};
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 23,
            };

            boxStyle.normal.background = FullScreenBlackTex;

            // Keep these two styles to revert
            GUIStyle verticalScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);
            GUIStyle thumbScrollbarStyle = new GUIStyle(GUI.skin.verticalScrollbar);

            GUI.skin.verticalScrollbar.fixedWidth = Screen.width * 0.03f;
            GUI.skin.verticalScrollbarThumb.fixedWidth = Screen.width * 0.03f;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, scrollViewStyle, GUILayout.Width(rect.width),
                GUILayout.Height(rect.height));

            GUILayout.Box(text, boxStyle);

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUI.skin.verticalScrollbar = verticalScrollbarStyle;
            GUI.skin.verticalScrollbarThumb = thumbScrollbarStyle;
        }

        public static Rect RatioRect(float x, float y, float widget, float heiht)
        {
            return new Rect(Screen.width * x, Screen.height * y, Screen.width * widget, Screen.height * heiht);
        }

        public static Rect LayoutIndexRect(int index, int countPerRow, float itemRWidget, float itemRHeight,
            float startRX, float startRY)
        {
            // Prepare variables for calculation
            int padding = 5;
            int startX = (int) (Screen.width * startRX);
            int startY = (int) (Screen.height * startRY);
            int w = (int) (Screen.width * itemRWidget);
            int h = (int) (Screen.height * itemRHeight);

            // Calculate position
            int x = startX + (index % countPerRow) * (w + padding);

            int row = (int) (index / countPerRow);
            int y = startY + row * (h + padding);

            return new Rect(x, y, w, h);
        }

        public static void DrawButton(string label, Action onClick, Color color, float width = -1, float height = -1)
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = color;

            var options = new List<GUILayoutOption>();

            if (width != -1)
                options.Add(GUILayout.Width(width));
            if (height != -1)
                options.Add(GUILayout.Height(height));

            if (GUILayout.Button(label, options.ToArray()))
            {
                onClick();
            }

            GUI.backgroundColor = previous;
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}