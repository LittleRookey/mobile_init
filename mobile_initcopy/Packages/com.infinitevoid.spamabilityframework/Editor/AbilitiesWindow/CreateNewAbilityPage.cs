using System;
using System.IO;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Directory = UnityEngine.Windows.Directory;


namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow
{
    public class CreateNewAbilityPage : VisualElement
    {

        public CreateNewAbilityPage(VisualElement parent)
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/CreateNewAbilityPage.uxml");
            visualTree.CloneTree(this);

            var newTargetedAbilityButton = this.Q<Button>("new-targeted-ability-button");
            newTargetedAbilityButton.clicked += () => CreateNewAbility<TargetedAbilitySO>();
            var newDirectionalAbilityButton = this.Q<Button>("new-directional-ability-button");
            newDirectionalAbilityButton.clicked += () => CreateNewAbility<DirectionalAbilitySO>();
            var newProjectileAbilityButton = this.Q<Button>("new-projectile-ability-button");
            newProjectileAbilityButton.clicked += () => CreateNewAbility<ProjectileAbilitySO>();
            var newRaycastAbilityButton = this.Q<Button>("new-raycast-ability-button");
            newRaycastAbilityButton.clicked += () => CreateNewAbility<RaycastAbilitySO>();

        
            var getPathFromSelectionButton = this.Q<Button>("get-path-from-selection-button");
            getPathFromSelectionButton.clicked += () =>
            {
                var pathTextField = this.Q<TextField>("new-ability-path");
                var path = EditorUtils.GetCurrentAssetDirectory();
                pathTextField.value = path;
            };
        }

        private void CreateNewAbility<T>() where T : AbilityBaseSO
        {
            try
            {
                var pathInput = this.Q<TextField>("new-ability-path");
                if (pathInput.text[0] == '/')
                    pathInput.value = pathInput.text.Substring(1);

                if (!Directory.Exists(pathInput.text))
                    Directory.CreateDirectory(pathInput.text);

                var nameInput = this.Q<TextField>("new-ability-name");
                var asset = ScriptableObject.CreateInstance<T>();
                var serializedAsset = new SerializedObject(asset);
                var prop = serializedAsset.FindProperty("_name");
                prop.stringValue = nameInput.text;
                serializedAsset.ApplyModifiedProperties();
                var assetName = GetAssetName(nameInput.text);
                AssetDatabase.CreateAsset(asset, Path.Combine(pathInput.text, $"{assetName}.asset"));
                Selection.activeObject = asset;
                SpamFramework.Editor.AbilitiesWindow.AbilitiesWindow.ReloadAbilities();
                SpamFramework.Editor.AbilitiesWindow.AbilitiesWindow.SetSelectionAsSelected();
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                EditorUtility.DisplayDialog("Error",
                    "Error creating new ability. Check console for errors. Common errors are an invalid path",
                    "Got it.");
            }
        }

        private string GetAssetName(string proposedName)
        {
            var assetExists = AssetDatabase.FindAssets($"t:AbilityBaseSO {proposedName}").Length > 0;
            if (!assetExists) return proposedName;

            int i = 1;
            while (assetExists)
            {
                assetExists = AssetDatabase.FindAssets($"t:AbilityBaseSO {proposedName}_{i}").Length > 0;
                if (assetExists)
                    i++;
            }
            return $"{proposedName}_{i}";
        }
    
    }
}