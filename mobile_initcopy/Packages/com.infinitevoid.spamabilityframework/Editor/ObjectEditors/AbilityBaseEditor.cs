using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AbilityBase), true)]
    [CanEditMultipleObjects]
    public class AbilityBaseEditor : UnityEditor.Editor
    {
        private SerializedProperty _invoker;
         ProjectileAbilitySO _selectedAbility;

        protected virtual void OnEnable()
        {
            _invoker = this.serializedObject.FindProperty("invoker");
        }

        public override void OnInspectorGUI()
        {
            var t = (AbilityBase) target;
            DrawDefaultInspector();
            GUILayout.Space(20);
            var serAbility = new SerializedObject(target);
            var invokerProp = serAbility.FindProperty("_invoker");
            if(invokerProp.objectReferenceValue == null)
                if (GUILayout.Button("Get invoker from children or parent(s)"))
                {
                    t.GetInvokerFromHierarchy();
                }

            
            if (t is ProjectileAbility projectileAbility)
                AddProjectileAbilityFields(serAbility, projectileAbility);
            
            
        }

        private void AddProjectileAbilityFields(SerializedObject serAbility, ProjectileAbility projectileAbility)
        {
            var poolProp = serAbility.FindProperty("_projectilePool");
            if (poolProp.objectReferenceValue != null)
                return;

            EditorGUILayout.LabelField("Create new pool", EditorStyles.boldLabel);
            _selectedAbility =
                EditorGUILayout.ObjectField("Projectile ability", _selectedAbility, typeof(ProjectileAbilitySO), false) as
                    ProjectileAbilitySO;
            if (GUILayout.Button("Create new projectile pool"))
            {
                if (_selectedAbility == null)
                {
                    EditorUtility.DisplayDialog("No ability selected", "You must set an ability before creating a pool", "Ok");
                    return;
                }

                var pool = projectileAbility.CreateAndSetPool(_selectedAbility);
                poolProp.objectReferenceValue = pool;
                serAbility.ApplyModifiedProperties();
                Selection.activeObject = pool;
            }
        }
    }
}