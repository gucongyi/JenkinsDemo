using UnityEngine;
using UnityEditor;

namespace GPCommon
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GameObjectPool))]
    public class GameObjectPoolInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextArea(serializedObject.targetObject.ToString());
            GUILayout.EndHorizontal();
        }
    }
}