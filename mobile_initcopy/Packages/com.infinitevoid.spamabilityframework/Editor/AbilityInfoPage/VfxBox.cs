using System.IO;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Editor.Common.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;
using static UnityEngine.ScriptableObject;
using Object = UnityEngine.Object;

namespace InfiniteVoid.SpamFramework.Editor.AbilityInfoPage
{
    public class VfxBox : VisualElement
    {
        private AbilityBaseSO _ability;

        public VfxBox(AbilityBaseSO ability, SerializedObject serializedAbility, SerializedProperty vfxProp, Box parent)
        {
            _ability = ability;
            var vfxBox = CreateBox("VFX", parent);

            vfxBox.AddToClassList("c-vfx-box");
            var emptyReferenceButtonText = "Create VFX";

            if (vfxProp.objectReferenceValue == null)
            {
                CreateEmptyReferenceBox(vfxBox, emptyReferenceButtonText,
                    () => CreateNewVfxAsset(serializedAbility));
                var propField = new PropertyField(vfxProp);
                propField.Bind(serializedAbility);
                propField.RegisterValueChangeCallback(evt =>
                {
                    vfxBox.Clear();
                    if (evt.changedProperty.objectReferenceValue == null)
                    {
                        CreateEmptyReferenceBox(vfxBox, emptyReferenceButtonText,
                            () => CreateNewVfxAsset(serializedAbility));
                        vfxBox.Add(propField);
                        return;
                    }

                    vfxBox.Clear();
                    vfxBox.Add(propField);
                    ListVFXProperties(evt.changedProperty.objectReferenceValue, vfxBox);
                });

                vfxBox.Add(propField);
            }
            else
            {
                var propField = new PropertyField(vfxProp);
                propField.Bind(serializedAbility);
                propField.RegisterValueChangeCallback(evt =>
                {
                    vfxBox.Clear();
                    vfxBox.Add(propField);
                    ListVFXProperties(evt.changedProperty.objectReferenceValue, vfxBox);
                });
                vfxBox.Clear();
                vfxBox.Add(propField);
                ListVFXProperties(vfxProp.objectReferenceValue, vfxBox);
            }
        }

        private void ListVFXProperties(Object vfxObject, Box parentBox)
        {
            var serializedVfx = new SerializedObject(vfxObject);
            parentBox.Add(CreateHeader3("Warmup"));
            AddPropToElement("_warmupVfx", serializedVfx, parentBox);
            AddPropToElement("_warmupLifeTime", serializedVfx, parentBox);
            AddPropToElement("_warmupCustomScale", serializedVfx, parentBox);

            parentBox.Add(CreateHeader3("On Hit", "u-margin-top"));
            parentBox.Add(CreateHelpBox("The on-hit VFX contains particles that is set to loop. This might result in the VFX constantly playing at the hit area.", "looping-vfx-info", HelpBoxMessageType.Warning));
            AddPropToElement("_onHitVfx", serializedVfx, parentBox, evt =>
            {
                var loopVfxInfo = parentBox.Q<HelpBox>("looping-vfx-info");
                if (evt.changedProperty.objectReferenceValue is GameObject go)
                {
                    var particleSystems = go.GetComponentsInChildren<ParticleSystem>();
                    loopVfxInfo.SetVisible(particleSystems.Any(ps => ps.main.loop)); 
                }
                else loopVfxInfo.SetVisible(false);
            });
            AddPropToElement("_numOfOnHitInstances", serializedVfx, parentBox,
                evt =>
                {
                    var onHitInstanceProp = serializedVfx.FindProperty("_onHitInstances");
                    if (_ability.VFX.SameOnHitCountAsMaxTargets)
                        onHitInstanceProp.intValue = _ability.MaxEffectTargets;
                    serializedVfx.ApplyModifiedProperties();
                    parentBox.Q<PropertyField>("on-hit-instances").style.display =
                        _ability.VFX.SameOnHitCountAsMaxTargets
                            ? DisplayStyle.None
                            : DisplayStyle.Flex;
                },
                tooltip: "The number of on-hit VFX instances to be spawned per projectile instance."
            );
            AddPropToElement("_onHitInstances", serializedVfx, parentBox, controlName: "on-hit-instances");
            AddPropToElement("_onHitSpawnPoint", serializedVfx, parentBox,
                evt =>
                {
                    parentBox.Q<PropertyField>("spawn-at-target-base").style.display =
                        _ability.VFX.OnHitSpawnOnHitAtImpactPoint ? DisplayStyle.None : DisplayStyle.Flex;
                });

            AddPropToElement("_spawnOnHitAtTargetBase", serializedVfx, parentBox, controlName: "spawn-at-target-base");
            AddPropToElement("_onHitPositionOffset", serializedVfx, parentBox);
            AddPropToElement("_lifeTimeHandling", serializedVfx, parentBox, evt =>
            {
                parentBox.Q<PropertyField>("on-hit-lifetime").style.display =
                    _ability.VFX.AutomaticOnHitLifetime
                        ? DisplayStyle.None
                        : DisplayStyle.Flex;
            });
            AddPropToElement("_onHitLifeTime", serializedVfx, parentBox, controlName: "on-hit-lifetime", tooltip: "Set to 0 if the VFX should be persistent (max number of VFX will still apply and when there's no more available they will be reused).");
        }


        private void CreateNewVfxAsset(SerializedObject serializedAbility)
        {
            var vfxAsset = CreateInstance<AbilityVFXSO>();
            var path = AssetDatabase.GetAssetPath(serializedAbility.targetObject);
            var directory = Path.GetDirectoryName(path);
            AssetDatabase.CreateAsset(vfxAsset,
                Path.Combine(directory,
                    serializedAbility.FindProperty("_name").stringValue + "_vfx.asset"));
            var animProp = serializedAbility.FindProperty("_abilityVfx");
            animProp.objectReferenceValue = vfxAsset;
            serializedAbility.ApplyModifiedProperties();
            Selection.activeObject = vfxAsset;
        }
    }
}