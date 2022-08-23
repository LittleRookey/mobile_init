using InfiniteVoid.SpamFramework.Core.Components;
using UnityEditor;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AbilityInvoker))]
    [CanEditMultipleObjects]
    public class AbilityInvokerEditor : UnityEditor.Editor
    {
        
        public override void OnInspectorGUI()
        {
            var t = (AbilityInvoker) target;
            if(t.transform.parent != null)
                EditorGUILayout.HelpBox("An invoker has to be in the root GameObject of an Actor that can cast abilities.", MessageType.Error);
            DrawDefaultInspector();
        }
    }
}