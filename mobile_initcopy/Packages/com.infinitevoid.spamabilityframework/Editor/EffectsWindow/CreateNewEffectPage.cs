using System;
using System.Collections.Generic;
using System.IO;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;
using static UnityEngine.ScriptableObject;

namespace InfiniteVoid.SpamFramework.Editor.EffectsWindow
{
    public class CreateNewEffectPage : VisualElement
    {
        public CreateNewEffectPage(VisualElement parent, IEnumerable<AbilityEffectSO> effectTypes)
        {
            var newEffectGrid = new Box();
            newEffectGrid.AddToClassList("c-new-effect-grid");
            parent.Add(newEffectGrid);
            
            var getPathFromSelectionButton = parent.parent.Q<Button>("get-path-from-selection-button");
            getPathFromSelectionButton.clicked += () =>
            {
                var pathTextField = parent.parent.Q<TextField>("new-effect-path");
                var path = EditorUtils.GetCurrentAssetDirectory();
                pathTextField.value = path;
            };
            
            foreach (var effect in effectTypes)
            {
                var effectName = effect.GetType().Name.Replace("EffectSO", "");
                var newEffectItem = CreateBox(effectName, newEffectGrid, additionalClass: "c-new-effect-grid__item");
                
                var descriptionElement = new TextElement {text = effect.HelpDescription};
                descriptionElement.AddToClassList("c-new-effect-grid__description");
                newEffectItem.Add(descriptionElement);
                
                var createButton = new Button { text = "Create"};
                createButton.AddToClassList("c-button--blue");
                createButton.AddToClassList("c-new-effect-grid__button");
                createButton.clicked += () => CreateNewEffect(effect);
                newEffectItem.Add(createButton);
            }
        }

        private void CreateNewEffect(AbilityEffectSO effect)
        {
            try
            {
                var pathInput = parent.parent.Q<TextField>("new-effect-path");
                if (pathInput.text[0] == '/')
                    pathInput.value = pathInput.text.Substring(1);
                if (!Directory.Exists(pathInput.text))
                    Directory.CreateDirectory(pathInput.text);

                var asset = CreateInstance(effect.GetType());
                var serializedAsset = new SerializedObject(asset);
                serializedAsset.ApplyModifiedProperties();
                var assetName = GetAssetName(effect.GetType().Name);
                AssetDatabase.CreateAsset(asset, Path.Combine(pathInput.text, $"{assetName}.asset"));
                Selection.activeObject = asset;
                SpamFramework.Editor.EffectsWindow.EffectsWindow.ReloadEffects();
                SpamFramework.Editor.EffectsWindow.EffectsWindow.SetSelectionAsSelected();
                
                var newEffectPage = parent.parent.Q<Box>("new-effect-page");
                newEffectPage.style.display = DisplayStyle.None;

            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error",
                    "Error creating new ability. Check console for errors. Common errors are leading with a / (/Assets instead of just Assets) or an invalid path",
                    "Got it.");
                Debug.Log(ex);
            }
        }
        
        private string GetAssetName(string proposedName)
        {
            var assetExists = AssetDatabase.FindAssets($"t:AbilityEffectSO {proposedName}").Length > 0;
            if (!assetExists) return proposedName;

            int i = 1;
            while (assetExists)
            {
                assetExists = AssetDatabase.FindAssets($"t:AbilityEffectSO {proposedName}_{i}").Length > 0;
                if (assetExists)
                    i++;
            }
            return $"{proposedName}_{i}";
        }
    }
}