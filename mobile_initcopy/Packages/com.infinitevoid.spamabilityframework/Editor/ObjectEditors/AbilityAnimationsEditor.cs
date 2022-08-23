using InfiniteVoid.SpamFramework.Core.AbilityData;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AbilityAnimationTimingsSO))]
    [CanEditMultipleObjects]
    public class AbilityAnimationsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var t = (AbilityAnimationTimingsSO) target;
            EditorGUILayout.HelpBox("You can either set timings manually or you can add your animation-clips to the 'Animations' field and click 'Set timings from animations'. Only the values in Timings are used at runtime.", MessageType.Info);
            DrawDefaultInspector();
            GUILayout.Space(20);
            if (GUILayout.Button("Set timings from animations"))
            {
                t.SetTimings();
            }
        }
    }
}