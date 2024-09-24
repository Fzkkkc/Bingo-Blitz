using Services;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(GameInstance))]
    public class PlayerPrefsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            if (GUILayout.Button("Clear PlayerPrefs"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Debug.Log("PlayerPrefs have been cleared.");
            }
        }
    }
}