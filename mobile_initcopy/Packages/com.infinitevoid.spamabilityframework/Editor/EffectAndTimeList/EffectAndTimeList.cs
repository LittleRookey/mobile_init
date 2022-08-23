using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Effects;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace InfiniteVoid.SpamFramework.Editor.EffectAndTimeList
{
    public class EffectAndTimeList : VisualElement
    {
        public EffectAndTimeList(AbilityBaseSO ability, SerializedObject serializedAbility)
        {
            VisualTreeAsset visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectAndTimeList/EffectAndTimeList.uxml");
            visualTree.CloneTree(this);
            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/EffectAndTimeList/EffectAndTimeList.uss");
            this.styleSheets.Add(styleSheet);
            
            styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/Icons.uss");
            this.styleSheets.Add(styleSheet);
            

            
            var listview = this.Q<ListView>("effect-list-view");
            var prop = serializedAbility.FindProperty("_abilityEffects");
            listview.itemsSource = ability.AbilityEffects;
            listview.selectionType = SelectionType.Single;
            listview.reorderable = true;
            listview.makeItem = MakeEffectTimeListItem;
            listview.bindItem = (e, i) =>
            {
                var container = e as Box;
                var curProp = prop.GetArrayElementAtIndex(i);
                var effectProp = curProp.FindPropertyRelative("Effect");
                var objectField = container.Q<ObjectField>();
                objectField.value = effectProp.objectReferenceValue;
                objectField.RegisterCallback<ChangeEvent<Object>>(e =>
                {
                    ability.AbilityEffects[i].Effect = (AbilityEffectSO) e.newValue;
                    serializedAbility.Update();
                });

                var label = container.Q<Label>("info-text");
                var abilityEffect = ability.AbilityEffects[i].Effect;
                var abilityName = abilityEffect == null || string.IsNullOrWhiteSpace(abilityEffect.Name) ? string.Empty : $"{abilityEffect.Name}: ";
                var desc = abilityEffect != null ? abilityEffect.Description : string.Empty;
                label.text = $"{abilityName}{desc}";
                container.Q<Button>("remove-effect").clicked += () =>
                {
                    ability.AbilityEffects.RemoveAt(i);
                    serializedAbility.Update();
                    listview.style.height = listview.itemHeight * ability.AbilityEffects.Count;
                    listview.Rebuild();
                };
                container.Q<Button>("edit-effect").clicked += () =>
                {
                    Selection.activeObject = abilityEffect;
                    EditorWindow.FocusWindowIfItsOpen<SpamFramework.Editor.EffectsWindow.EffectsWindow>();
                    SpamFramework.Editor.EffectsWindow.EffectsWindow.SetSelectionAsSelected();
                };
            };
            listview.style.height = listview.itemHeight * ability.AbilityEffects.Count;

            this.Q<Button>("create-effect-button").clicked += () =>
            {
                EditorWindow.FocusWindowIfItsOpen<SpamFramework.Editor.EffectsWindow.EffectsWindow>();
                EditorWindow.GetWindow<SpamFramework.Editor.EffectsWindow.EffectsWindow>().SwitchToCreateNewEffectPage();
            };

            this.Q<Button>("add-effect-button").clicked += () =>
            {
                ability.AbilityEffects.Add(new EffectAndTime());
                prop.isExpanded = true;
                serializedAbility.Update();
                listview.style.height = listview.itemHeight * ability.AbilityEffects.Count;
                listview.Rebuild();
            };

            #if UNITY_2021_2_OR_NEWER
                var infoText = this.Q<HelpBox>("visual-bug-text");
                infoText.style.display = DisplayStyle.None;
            #endif
        }

        private VisualElement MakeEffectTimeListItem()
        {
            var box = new Box();
            var button = new Button {name ="remove-effect"};
            box.Add(button);
            button.style.width = 20;
            button.AddToClassList("icon-minus");
            box.AddToClassList("c-effects-list-view__item");

            var objectField = new ObjectField();
            objectField.objectType = typeof(AbilityEffectSO);
            box.Add(objectField);

            var editEffectButton = new Button {name="edit-effect"};
            editEffectButton.style.width = 20;
            editEffectButton.AddToClassList("icon-settings");
            box.Add(editEffectButton);
            
            var floatField = new FloatField();
            floatField.label = "Time";
            box.Add(floatField);

            var label = new Label();
            label.name = "info-text";
            box.Add(label);
            
            return box;
        }
    }
}