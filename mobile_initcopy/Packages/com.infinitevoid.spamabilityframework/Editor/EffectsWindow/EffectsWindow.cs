using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.EffectsWindow
{
    public class EffectsWindow : EditorWindow
    {
        private static IList<AbilityEffectSO> _effects;
        private IEnumerable<AbilityEffectSO> _effectTypes;
        private ListView _effectsListView;

        public static void ReloadEffects()
        {
            _effects = GetAllEffects();
            EffectsWindow wnd = GetWindow<EffectsWindow>();
            wnd.titleContent = new GUIContent("Effects");
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.itemsSource = _effects.ToList();
            listView.Rebuild();
            listView.SetSelection(0);
        }

        public static void SetSelectionAsSelected()
        {
            EffectsWindow wnd = GetWindow<EffectsWindow>();
            wnd.titleContent = new GUIContent("Effects");
            var selectedIndex = _effects.IndexOf(Selection.activeObject as AbilityEffectSO);
            var listView = wnd.rootVisualElement.Q<ListView>();
            listView.ClearSelection();
            listView.SetSelection(selectedIndex);
        }

        public void CreateGUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectsWindow/EffectsWindow.uxml");

            var tree = visualTree.Instantiate();
            tree.StretchToParentSize();
            rootVisualElement.Add(tree);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectsWindow/EffectsWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            var createNewButton = tree.Q<Button>("create-new-effect");
            createNewButton.clicked += SwitchToCreateNewEffectPage;


            _effects = GetAllEffects();
            _effectTypes = GetAllEffectTypes();
            _effectsListView = rootVisualElement.Q<ListView>(className: "c-effects-list-view");
            _effectsListView.styleSheets.Add(styleSheet);
            _effectsListView.makeItem = () => new Label();
            _effectsListView.bindItem = (e, i) =>
                (e as Label).text = string.IsNullOrWhiteSpace(_effects[i].Name) ? _effects[i].name : _effects[i].Name;
            _effectsListView.itemsSource = _effects.ToList();
            _effectsListView.selectionType = SelectionType.Single;
            _effectsListView.onSelectionChange += OnEffectSelected;
        }

        public void SwitchToCreateNewEffectPage()
        {
            var effectInfoContainer = rootVisualElement.Q<Box>("effect-info");
            effectInfoContainer.style.display = DisplayStyle.None;
            var newEffectPage = rootVisualElement.Q<Box>("new-effect-page");
            newEffectPage.style.display = DisplayStyle.Flex;
            var newEffectContainer = rootVisualElement.Q<Box>("new-effect");
            newEffectContainer.Clear();
            newEffectContainer.Add(new CreateNewEffectPage(newEffectContainer, _effectTypes));
        }

        private void OnEffectSelected(IEnumerable<object> objects)
        {
            foreach (var item in objects)
            {
                var effectSO = (AbilityEffectSO) item;
                if (effectSO == null)
                    return;

                var effectInfoContainer = rootVisualElement.Q<Box>("effect-info");
                effectInfoContainer.style.display = DisplayStyle.Flex;

                var serializedEffect = new SerializedObject(effectSO);
                var header = rootVisualElement.Q<Label>("effect-header");
                header.text
                    = GetHeaderText(effectSO);


                var goToAssetButton = rootVisualElement.Q<Button>("select-asset-button");
                goToAssetButton.clicked += () => { Selection.activeObject = effectSO; };


                var effectType = effectSO.GetType().Name;
                rootVisualElement.Q<Label>("effect-type-name").text = effectType.Remove(effectType.Length - 2);

                var desc = rootVisualElement.Q<TextElement>("effect-desc");
                desc.text = effectSO.HelpDescription;

                var settingsWrapper = rootVisualElement.Q<Box>("effect-settings");
                settingsWrapper.Clear();
                var settingsBox = CreateBox("General settings", settingsWrapper);
                AddPropToElement("_name", serializedEffect, settingsBox, evt =>
                {
                    header.text = GetHeaderText(effectSO);
                    _effectsListView.Rebuild();
                });
                AddPropToElement("_description", serializedEffect, settingsBox, additionalClass: "c-effect-description");
                settingsBox.Add(CreateHelpBox(
                    "If this is turned off then the effect won't be included in the list of effects an ability publicly has. Useful for removing an effect from an ability's public interface so the ability's effects can be listen in the GUI.",
                    string.Empty));
                AddPropToElement("_includedInAbilityEffects", serializedEffect, settingsBox);


                var specificSettingsWrapper = rootVisualElement.Q<Box>("effect-specific-settings");
                specificSettingsWrapper.Clear();
                var specificSettingsBox = CreateBox("Effect specific settings", specificSettingsWrapper);

                var prop = serializedEffect.GetIterator();
                prop.Next(true);
                while (prop.NextVisible(false))
                {
                    if (prop.propertyPath == "m_Script"
                        || prop.propertyPath == "_name"
                        || prop.propertyPath == "_description"
                        || prop.propertyPath == "_includedInAbilityEffects")
                        continue;

                    var field = new PropertyField(prop);
                    field.AddToClassList("u-wide-label");


                    // Special case since we style PropertyField with a greater height and this breaks
                    // lists. audioclips is as array.
                    if (prop.propertyPath == "_audioClips")
                        field.AddToClassList("u-list-property");

                    field.Bind(serializedEffect);
                    specificSettingsBox.Add(field);
                }
            }
        }

        private string GetHeaderText(AbilityEffectSO effectSO)
        {
            return string.IsNullOrWhiteSpace(effectSO.Name)
                ? effectSO.name
                : $"{effectSO.Name} ({effectSO.name})";
        }

        private static IList<AbilityEffectSO> GetAllEffects()
        {
            return AssetDatabase
                .FindAssets("t:AbilityEffectSO", null)
                .Select(guid => AssetDatabase.LoadAssetAtPath<AbilityEffectSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }

        private static IEnumerable<AbilityEffectSO> GetAllEffectTypes()
        {
            return Assembly.GetAssembly(typeof(DamageEffectSO)).GetTypes()
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(AbilityEffectSO)))
                .Select(t => (AbilityEffectSO) ScriptableObject.CreateInstance(t));
        }
    }
}