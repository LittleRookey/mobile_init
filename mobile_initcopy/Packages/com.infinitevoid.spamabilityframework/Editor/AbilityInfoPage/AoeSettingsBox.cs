using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.AbilityInfoPage
{
    public class AoeSettingsBox : VisualElement
    {
        public AoeSettingsBox(AbilityBaseSO ability, SerializedObject serializedAbility, Box parent)
        {
            var aoeWrapperBox = new Box();
            parent.Add(aoeWrapperBox);
            aoeWrapperBox.Add(CommonUI.CreateHeader3("Area of effect settings"));
            aoeWrapperBox.AddToClassList("u-margin-top");
            if (ability is DirectionalAbilitySO)
                CreateDirectionalAbilityAoeBox(serializedAbility, aoeWrapperBox);
            else
                CreateTargetedAbilityAoeBox(serializedAbility, aoeWrapperBox);
        }

        private void CreateTargetedAbilityAoeBox(SerializedObject serializedAbility, Box parentBox)
        {
            var aoeBox = new Box();
            aoeBox.AddToClassList("c-aoe-box");
            var toggle = new Toggle("AOE");
            var effectRadiusProp = serializedAbility.FindProperty("_effectRadius");
            var maxTargetsProp = serializedAbility.FindProperty("_maxEffectTargets");
            var field = new PropertyField(effectRadiusProp);
            field.Bind(serializedAbility);
            toggle.value = effectRadiusProp.floatValue > 0;
            parentBox.Add(toggle);
            parentBox.Add(aoeBox);
            toggle.RegisterCallback<ChangeEvent<bool>>(AoeAbilityCbChanged);

            if (toggle.value)
            {
                aoeBox.Clear();
                aoeBox.Add(field);
                AddCommonElementsToAoeBox(serializedAbility, aoeBox);
            }

            void AoeAbilityCbChanged(ChangeEvent<bool> evt)
            {
                if (evt.newValue)
                {
                    if (effectRadiusProp.floatValue == 0)
                    {
                        effectRadiusProp.floatValue = 1;
                        maxTargetsProp.intValue = 1;
                        effectRadiusProp.serializedObject.ApplyModifiedProperties();
                    }

                    aoeBox.Add(field);
                    AddCommonElementsToAoeBox(serializedAbility, aoeBox);
                }
                else
                {
                    effectRadiusProp.floatValue = 0;
                    maxTargetsProp.intValue = 1;
                    effectRadiusProp.serializedObject.ApplyModifiedProperties();
                    aoeBox.Clear();
                }
            }
        }

        private void CreateDirectionalAbilityAoeBox(SerializedObject serializedAbility, Box parentBox)
        {
            AddPropToElement("_angle", serializedAbility, parentBox);
            AddPropToElement("_distance", serializedAbility, parentBox);
            AddCommonElementsToAoeBox(serializedAbility, parentBox);
        }


        private void AddCommonElementsToAoeBox(SerializedObject serializedAbility, Box parentBox)
        {
            AddPropToElement("_maxEffectTargets", serializedAbility, parentBox, evt =>
            {
                if (evt.changedProperty.intValue <= 0)
                {
                    evt.changedProperty.intValue = 1;
                    serializedAbility.ApplyModifiedProperties();
                }
                
            });
            AddPropToElement("_lineOfSightCheck", serializedAbility, parentBox, evt =>
            {
                var losLayersField = parentBox.Q<VisualElement>("los-layers");
                losLayersField.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            });
            AddPropToElement("_losBlockingLayers", serializedAbility, parentBox, controlName:"los-layers");
            AddPropToElement("_effectLayers", serializedAbility, parentBox);
            AddPropToElement("_effectApplication", serializedAbility, parentBox,
                tooltip:
                "Per effect: applies effects to all targets at the same time. Per target: applies effects to one target at a time",
                onChange:
                evt =>
                {
                    var box = parentBox.Q<Box>("effect-application-wrapper");
                    box.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                });
            var applicationWrapper = new Box();
            parentBox.Add(applicationWrapper);
            applicationWrapper.name = "effect-application-wrapper";
            applicationWrapper.Add(CreateHelpBox(
                "Effects will be applied to each target separately. Time between effects will still be honored",
                "application-delay-help"));
            AddPropToElement("_perTargetEffectApplicationDelay", serializedAbility, applicationWrapper);
        }
    }
}