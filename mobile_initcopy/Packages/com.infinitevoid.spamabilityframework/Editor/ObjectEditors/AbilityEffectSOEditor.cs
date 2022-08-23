using InfiniteVoid.SpamFramework.Core.Effects;
using UnityEditor;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AbilityEffectSO), true)]
    [CanEditMultipleObjects]
    public class AbilityEffectSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            var item = (AbilityEffectSO)target;
            
            EditorGUILayout.HelpBox(item.HelpDescription, MessageType.None);
            DrawDefaultInspector();
        }
    }
}