using System.IO;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;
using static UnityEngine.ScriptableObject;
using Object = UnityEngine.Object;

namespace InfiniteVoid.SpamFramework.Editor.AbilityInfoPage
{
    public class AnimationsBox : VisualElement
    {
        public AnimationsBox(SerializedObject serializedAbility, SerializedProperty animationsProp, Box parent)
        {
            var animBox = CommonUI.CreateBox("Animations & Timings", parent);

            animBox.AddToClassList("c-animations-box");
            var emptyReferenceButtonText = "Create Animations & timings";

            if (animationsProp.objectReferenceValue == null)
            {
                CreateEmptyReferenceBox(animBox, emptyReferenceButtonText,
                    () => CreateNewAnimationAsset(serializedAbility));
                var propField = new PropertyField(animationsProp);
                propField.Bind(serializedAbility);
                propField.RegisterValueChangeCallback(evt =>
                {
                    animBox.Clear();
                    if (evt.changedProperty.objectReferenceValue == null)
                    {
                        CreateEmptyReferenceBox(animBox, emptyReferenceButtonText,
                            () => CreateNewAnimationAsset(serializedAbility));
                        animBox.Add(propField);
                        return;
                    }

                    animBox.Clear();
                    animBox.Add(propField);
                    ListAnimationProperties(evt.changedProperty.objectReferenceValue, animBox);
                });

                animBox.Add(propField);
            }
            else
            {
                var propField = new PropertyField(animationsProp);
                propField.Bind(serializedAbility);
                propField.RegisterValueChangeCallback(evt =>
                {
                    animBox.Clear();
                    animBox.Add(propField);
                    ListAnimationProperties(evt.changedProperty.objectReferenceValue, animBox);
                });
                animBox.Clear();
                animBox.Add(propField);
                ListAnimationProperties(animationsProp.objectReferenceValue, animBox);
            }
        }

        private void ListAnimationProperties(Object animationsObject, Box parentBox)
        {
            var serializedAnimations = new SerializedObject(animationsObject);
            parentBox.Add(CreateHeader3("Timings"));
            parentBox.Add(CreateHelpBox(
                "You can either set timings manually or you can add your animation-clips to the 'Animations' fields and click 'Set timings from animations'. Only the values in Timings are used at runtime. Note that the total animation time should in most cases be shorter than the ability's cooldown.",
                "anims-help"));
            AddPropToElement("_animationWarmupTime", serializedAnimations, parentBox);
            AddPropToElement("_animationCastTime", serializedAnimations, parentBox);
            AddPropToElement("_animationCooldownTime", serializedAnimations, parentBox);

            parentBox.Add(CreateHeader3("Animator parameters", "u-margin-top"));
            parentBox.Add(CreateHelpBox("If you use a bool for cast, you have to manually call ability.StopCasting() to turn it off.", "cast-bool-help"));
            AddPropToElement("_warmupTriggerName", serializedAnimations, parentBox);
            AddPropToElement("_castTriggerName", serializedAnimations, parentBox);
            AddPropToElement("_castBoolName", serializedAnimations, parentBox);
            AddPropToElement("_cooldownTriggerName", serializedAnimations, parentBox);

            parentBox.Add(CreateHeader3("Animations", "u-margin-top"));
            AddPropToElement("_warmupAnimation", serializedAnimations, parentBox);
            AddPropToElement("_castAnimation", serializedAnimations, parentBox);
            AddPropToElement("_cooldownAnimation", serializedAnimations, parentBox);
            var button = new Button();
            button.text = "Set timings from animations";
            button.clicked += () =>
            {
                var animations = serializedAnimations.targetObject as AbilityAnimationTimingsSO;
                animations.SetTimings();
            };
            parentBox.Add(button);
        }


        private void CreateNewAnimationAsset(SerializedObject serializedAbility)
        {
            var animationAsset = CreateInstance<AbilityAnimationTimingsSO>();
            var path = AssetDatabase.GetAssetPath(serializedAbility.targetObject);
            var directory = Path.GetDirectoryName(path);
            AssetDatabase.CreateAsset(animationAsset,
                Path.Combine(directory,
                    serializedAbility.FindProperty("_name").stringValue + "_anims.asset"));
            var animProp = serializedAbility.FindProperty("_animationTimings");
            animProp.objectReferenceValue = animationAsset;
            serializedAbility.ApplyModifiedProperties();
            Selection.activeObject = animationAsset;
        }
    }
}