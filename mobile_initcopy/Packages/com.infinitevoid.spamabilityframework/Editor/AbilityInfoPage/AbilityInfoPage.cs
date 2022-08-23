using System;
using System.IO;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using InfiniteVoid.SpamFramework.Core.Components.Abilities;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;
using Object = UnityEngine.Object;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;

namespace InfiniteVoid.SpamFramework.Editor.AbilityInfoPage
{
    public class AbilityInfoPage : VisualElement
    {
        private Box _parent;
        private AbilityBaseSO _ability;

        public AbilityInfoPage(AbilityBaseSO ability, Box parent, Action refreshListView)
        {
            this._parent = parent;
            this._ability = ability;
            var serializedAbility = new SerializedObject(ability);
            var header = CommonUI.CreateHeader(GetHeaderText(ability), "ability-info-header");
            parent.Add(header);
            var assetButtonsRow = new VisualElement();
            assetButtonsRow.AddToClassList("row");
            assetButtonsRow.Add(CreateGoToAssetButton(ability));
            // if(ability.name.ToLower() != ability.Name.ToLower())
            //     assetButtonsRow.Add(CreateRenameAssetButton(ability));
            parent.Add(assetButtonsRow);

            var addToGoBox = CommonUI.CreateBox("Add to selected GameObject", parent);
            AddAddAbilityButtons(ability, addToGoBox);

            var settingsWrapper = new Box();
            settingsWrapper.AddToClassList("c-ability-settings-wrapper");
            parent.Add(settingsWrapper);
            var abilitySettingsBox = CommonUI.CreateBox("Ability settings", settingsWrapper, true);


            var unknownIcon = AssetDatabase.LoadAssetAtPath<Texture>($"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Textures/unknown.png");
            var image = new Image();
            image.AddToClassList("ability-image");
            abilitySettingsBox.Add(image);
            if (ability.Icon)
                image.image = ability.Icon.texture;
            else image.image = unknownIcon;
            var propertiesWrapper = new Box();
            abilitySettingsBox.Add(propertiesWrapper);

            AddPropToElement("_name", serializedAbility, propertiesWrapper, _ =>
            {
                refreshListView();
                header.text = GetHeaderText(ability);
            });
            var multilineText = new TextField { multiline = true, bindingPath = "_description", label = "Description" };
            multilineText.AddToClassList("c-multiline-input");
            multilineText.Bind(serializedAbility);
            propertiesWrapper.Add(multilineText);
            // AddPropToElement("_description", serializedAbility, propertiesWrapper);
            AddPropToElement("_icon", serializedAbility, propertiesWrapper, evt =>
            {
                var sprite = evt.changedProperty.objectReferenceValue as Sprite;
                image.image = sprite == null ? unknownIcon : sprite.texture;
                refreshListView();
            });
            AddPropToElement("_abilityCooldown", serializedAbility, propertiesWrapper);
            AddPropToElement("_cooldownType", serializedAbility, propertiesWrapper,
                tooltip: "Automatic uses Time.deltaTime, Manual ticks when Ability.TickCooldown() is called");
            if (!(ability is RaycastAbilitySO))
            {
                AddPropToElement("_castRange", serializedAbility, propertiesWrapper);
            }
            else
            {
                AddPropToElement("_raycastLength", serializedAbility, propertiesWrapper);
                AddPropToElement("_raycastLayers", serializedAbility, propertiesWrapper);
            }

            AddPropToElement("_requiresAbilityTarget", serializedAbility, propertiesWrapper, evt =>
            {
                var distanceCheckWrapper = settingsWrapper.Q<VisualElement>("cast-on-ground-wrapper");
                if (distanceCheckWrapper == null) return;
                distanceCheckWrapper.style.display =
                    evt.changedProperty.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
            });
            
            AddPropToElement("_telegraph", serializedAbility, propertiesWrapper);

            if (ability is TargetedAbilitySO)
            {
                AddPropToElement("_castOnSelf", serializedAbility, propertiesWrapper);
            }
            else if (ability is ProjectileAbilitySO)
            {
                var projectileSettingsBox = CommonUI.CreateBox("Projectile settings", settingsWrapper);
                AddPropToElement("_projectilePrefab", serializedAbility, projectileSettingsBox);
                AddPropToElement("_projectileSpeed", serializedAbility, projectileSettingsBox);
                AddPropToElement("_timeToLive", serializedAbility, projectileSettingsBox);
                AddPropToElement("_inFlightSound", serializedAbility, projectileSettingsBox);
                projectileSettingsBox.Add(CreateHelpBox(
                    "Disable: Stop movement and disable collider. Deactivate: Completely deactivate the projectile",
                    "hit-action-help"));
                AddPropToElement("_onHitAction", serializedAbility, projectileSettingsBox);

                var castOnGroundSettings = new VisualElement { name = "cast-on-ground-wrapper" };
                castOnGroundSettings.Add(CreateHelpBox(
                    "Set this if the projectile should be able to travel in Y-space after it has spawned. Only applicable if cast on ground.",
                    "movement-help-box"));
                AddPropToElement("_3dMovement", serializedAbility, castOnGroundSettings);
                castOnGroundSettings.Add(CreateHelpBox(
                    "When cast on ground this distance will be used to check if the projectile has reached it's target.",
                    "distance-check-help"));
                AddPropToElement("_distanceCheckRange", serializedAbility, castOnGroundSettings);
                projectileSettingsBox.Add(castOnGroundSettings);
            }


            CreateEffectsBox(ability, serializedAbility, parent);

            AddReferencesRow(serializedAbility, parent);
        }

        private Button CreateGoToAssetButton(AbilityBaseSO ability)
        {
            var goToAssetButton = new Button(() => { Selection.activeObject = ability; });
            goToAssetButton.text = "Select asset in project";
            goToAssetButton.AddToClassList("u-margin-bottom");
            return goToAssetButton;
        }

        private Button CreateRenameAssetButton(AbilityBaseSO ability)
        {
            var renameAssetButton = new Button(() =>
            {
                // ability.name = ability.Name;
            });
            renameAssetButton.text = "Rename asset(s) to ability name";
            renameAssetButton.AddToClassList("u-margin-bottom");
            return renameAssetButton;
        }

        private void AddAddAbilityButtons(AbilityBaseSO ability, Box abilityInfoBox)
        {
            var wrapper = new Box();
            wrapper.AddToClassList("c-add-ability-wrapper");
            abilityInfoBox.Add(wrapper);
            var firstRow = new Box();
            firstRow.AddToClassList("c-add-ability-box");
            firstRow.AddToClassList("row");
            wrapper.Add(firstRow);
            var secondRow = new Box();
            secondRow.AddToClassList("c-add-ability-box");
            secondRow.AddToClassList("row");
            wrapper.Add(secondRow);

            if (ability is ProjectileAbilitySO)
            {
                var addAsTargetedButton = new Button(AddAsPooledToSelected<TargetedProjectileAbility>);
                addAsTargetedButton.text = "As targeted projectile";
                firstRow.Add(addAsTargetedButton);
                var addAsDirectionButton = new Button(AddAsPooledToSelected<DirectionalProjectileAbility>);
                addAsDirectionButton.text = "As directional projectile";
                firstRow.Add(addAsDirectionButton);
            }
            else if (ability is TargetedAbilitySO)
            {
                var addAsTargetedButton = new Button(AddComponentToSelected<TargetedAbility>);
                addAsTargetedButton.text = "As targeted ability";
                firstRow.Add(addAsTargetedButton);
            }
            else if (ability is DirectionalAbilitySO)
            {
                var addAsTargetedButton = new Button(AddComponentToSelected<DirectionalAbility>);
                addAsTargetedButton.text = "As directional ability";
                firstRow.Add(addAsTargetedButton);
            }
            else if (ability is RaycastAbilitySO)
            {
                var addAsTargetedButton = new Button(AddComponentToSelected<RaycastAbility>);
                addAsTargetedButton.text = "As raycast ability";
                firstRow.Add(addAsTargetedButton);
            }

            void AddComponentToSelected<T>() where T : AbilityBase
            {
                var selected = Selection.activeGameObject;
                if (selected == null)
                {
                    Debug.Log("No object selected");
                    return;
                }

                var abilityComponent = selected.AddComponent<T>();
                abilityComponent.GetInvokerFromHierarchy();
                var serializedAbility = new SerializedObject(abilityComponent);
                serializedAbility.Update();
                SerializedProperty serializedProp;
                if (abilityComponent is ProjectileAbility)
                    serializedProp = serializedAbility.FindProperty("_projectileAbilitySo");
                else if (abilityComponent is DirectionalAbility)
                    serializedProp = serializedAbility.FindProperty("_directionalAbility");
                else if (abilityComponent is TargetedAbility)
                    serializedProp = serializedAbility.FindProperty("_targetedAbility");
                else
                    serializedProp = serializedAbility.FindProperty("_raycastAbility");

                serializedProp.objectReferenceValue = ability;
                serializedAbility.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }

            void AddAsPooledToSelected<T>() where T : ProjectileAbility
            {
                var selected = Selection.activeGameObject;
                if (selected == null)
                {
                    Debug.Log("No object selected");
                    return;
                }

                var poolGo = new GameObject();
                poolGo.name = $"{ability.Name}_Pool";
                var projectilePool = poolGo.AddComponent<ProjectilePool>();
                var serializedPool = new SerializedObject(projectilePool);
                var serializedProp = serializedPool.FindProperty("_projectileInPool");
                serializedProp.objectReferenceValue = ability;
                serializedPool.ApplyModifiedProperties();

                var abilityComponent = selected.AddComponent<T>();
                abilityComponent.GetInvokerFromHierarchy();
                var serializedAbility = new SerializedObject(abilityComponent);
                serializedAbility.Update();
                serializedProp = serializedAbility.FindProperty("_projectilePool");
                serializedProp.objectReferenceValue = projectilePool;
                serializedAbility.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }

        private void CreateEffectsBox(AbilityBaseSO ability, SerializedObject serializedAbility, Box abilityInfoBox)
        {
            var effectsBox = CommonUI.CreateBox("Effects", abilityInfoBox);
            effectsBox.AddToClassList("c-effects-box");
            effectsBox.Add(new EffectAndTimeList.EffectAndTimeList(ability, serializedAbility));
            effectsBox.Add(new AoeSettingsBox(ability, serializedAbility, effectsBox));
        }

        /// <summary>
        /// Adds Animations and VFX boxen to a separate row
        /// </summary>
        /// <param name="serializedAbility"></param>
        /// <param name="abilityInfoBox"></param>
        private void AddReferencesRow(SerializedObject serializedAbility, Box abilityInfoBox)
        {
            var referencesRow = new Box();
            referencesRow.AddToClassList("c-references-wrapper");
            referencesRow.AddToClassList("row");

            var prop = serializedAbility.FindProperty("_animationTimings");
            referencesRow.Add(new AnimationsBox(serializedAbility, prop, referencesRow));

            prop = serializedAbility.FindProperty("_abilityVfx");
            referencesRow.Add(new VfxBox(_ability, serializedAbility, prop, referencesRow));

            prop = serializedAbility.FindProperty("_abilitySfx");
            var sfxBox = CommonUI.CreateBox("SFX", referencesRow);
            sfxBox.Add(
                CreateObjectBox(serializedAbility,
                    prop,
                    "c-vfx-box",
                    "Create SFX", () =>
                    {
                        var abilitySfxAsset = CreateInstance<AbilitySFXSO>();
                        var path = AssetDatabase.GetAssetPath(serializedAbility.targetObject);
                        var directory = Path.GetDirectoryName(path);
                        AssetDatabase.CreateAsset(abilitySfxAsset,
                            Path.Combine(directory,
                                serializedAbility.FindProperty("_name").stringValue + "_sfx.asset"));
                        var animProp = serializedAbility.FindProperty("_abilitySfx");
                        animProp.objectReferenceValue = abilitySfxAsset;
                        serializedAbility.ApplyModifiedProperties();
                        Selection.activeObject = abilitySfxAsset;
                    }));
            abilityInfoBox.Add(referencesRow);
        }

        private Box CreateObjectBox(SerializedObject serializedAbility,
            SerializedProperty serializedProperty,
            string className,
            string emptyReferenceButtonText,
            Action onNewReferenceClick)
        {
            Box parentBox = new Box();
            parentBox.AddToClassList(className);
            if (serializedProperty.objectReferenceValue == null)
            {
                CreateEmptyReferenceBox(parentBox, emptyReferenceButtonText, onNewReferenceClick);
                var prop = new PropertyField(serializedProperty);
                prop.Bind(serializedAbility);
                prop.RegisterValueChangeCallback(evt =>
                {
                    parentBox.Clear();
                    if (evt.changedProperty.objectReferenceValue == null)
                    {
                        CreateEmptyReferenceBox(parentBox, emptyReferenceButtonText, onNewReferenceClick);
                        parentBox.Add(prop);
                        return;
                    }

                    CreateReferenceBox(evt.changedProperty.objectReferenceValue, parentBox,
                        prop);
                });

                parentBox.Add(prop);
            }
            else
            {
                var prop = new PropertyField(serializedProperty);
                prop.Bind(serializedAbility);
                prop.RegisterValueChangeCallback(evt =>
                {
                    CreateReferenceBox(evt.changedProperty.objectReferenceValue, parentBox,
                        prop);
                });
                CreateReferenceBox(serializedProperty.objectReferenceValue, parentBox, prop);
            }

            return parentBox;
        }


        private void CreateEmptyReferenceBox(Box parentBox, string buttonText,
            Action onNewReferenceClick)
        {
            var button = new Button();
            button.text = buttonText;
            button.clicked += onNewReferenceClick;
            parentBox.Add(button);
        }

        private void CreateReferenceBox(Object objectReference, Box parentBox, PropertyField prop)
        {
            parentBox.Clear();
            parentBox.Add(prop);

            var serializedObject = new SerializedObject(objectReference);
            SerializedProperty objectProps = serializedObject.GetIterator();
            objectProps.Next(true);
            while (objectProps.NextVisible(false))
            {
                if (objectProps.name == "m_Script")
                    continue;

                var propField = new PropertyField(objectProps);

                if (objectProps.name == "_onHitSfx")
                {
                    // Special case since we style PropertyField with a greater height and this breaks
                    // lists. On Hit SFX is as array.
                    propField.AddToClassList("u-list-property");
                }

                propField.Bind(serializedObject);
                parentBox.Add(propField);
            }
        }

        private string GetNameFromAbilityType(string abilityType)
        {
            switch (abilityType)
            {
                case "ProjectileAbilitySO":
                    return "Projectile";
                case "TargetedAbilitySO":
                    return "Targeted ability";
                case "DirectionalAbilitySO":
                    return "Directional ability";
                case "RaycastAbilitySO":
                    return "Raycast ability";
                default:
                    return "Unknown";
            }
        }

        private string GetHeaderText(AbilityBaseSO ability)
        {
            return $"{ability.Name} | {GetNameFromAbilityType(ability.GetType().Name)}";
        }
    }
}