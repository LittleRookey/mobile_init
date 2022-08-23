using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(AoeIndicator))]
    [CanEditMultipleObjects]
    public class AoeIndicatorEditor : UnityEditor.Editor
    {
        private static string CIRLE_BOX_NAME = "circle-box";
        private static string WEDGE_BOX_NAME = "wedge-box";

        public override VisualElement CreateInspectorGUI()
        {
            // Create a new VisualElement to be the root of our inspector UI
            VisualElement inspector = new VisualElement();
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            inspector.styleSheets.Add(styleSheet);

            var aoeIndicator = (AoeIndicator) target;
            if (aoeIndicator == null) return inspector;
            
            var serializedIndicator = new SerializedObject(aoeIndicator);
            inspector.Add(CreateHeader2("General settings"));
            AddPropToElement("_type", serializedIndicator, inspector, evt =>
            {
                var isCircle = evt.changedProperty.enumValueIndex == (int) AoeIndicator.IndicatorType.Circle;
                var circleBox = inspector.Q<VisualElement>(CIRLE_BOX_NAME);
                var wedgeBox = inspector.Q<VisualElement>(WEDGE_BOX_NAME);
                circleBox.style.display = isCircle ? DisplayStyle.Flex : DisplayStyle.None;
                wedgeBox.style.display = !isCircle ? DisplayStyle.Flex : DisplayStyle.None;
            });
            inspector.Add(CreateHelpBox(
                "Enable this if the indicator is controlled via the AbilitySystemMediator instead of via direct reference. Useful if you have only one type of indicator for your player.",
                "event-help"));
            AddPropToElement("_eventDriven", serializedIndicator, inspector);

            AddCirclePropsBox(inspector, serializedIndicator);
            AddWedgePropsBox(inspector, serializedIndicator);


            return inspector;
        }

        private void AddWedgePropsBox(VisualElement inspector, SerializedObject serializedIndicator)
        {
            var box = new VisualElement {name = WEDGE_BOX_NAME};
            box.AddToClassList("u-margin-top");
            box.Add(CreateHeader3("Wedge indicator settings"));
            AddPropToElement("_distance", serializedIndicator, box);
            AddPropToElement("_angle", serializedIndicator, box);
            AddPropToElement("_previewWedge", serializedIndicator, box);
            AddPropToElement("_wedgePreviewColor", serializedIndicator, box);
            inspector.Add(box);
        }

        private void AddCirclePropsBox(VisualElement inspector, SerializedObject serializedIndicator)
        {
            var box = new VisualElement {name = CIRLE_BOX_NAME};
            box.AddToClassList("u-margin-top");
            box.Add(CreateHeader3("Circle indicator settings"));
            box.Add(CreateHelpBox(
                "Get this value by placing a sphere over the AOE-quad and scale the sphere so it exactly matches the edges of the indicator. The scale factor will then be .5/SPHERE_SCALE",
                "scale-help"));
            AddPropToElement("_scaleFactor", serializedIndicator, box);
            inspector.Add(box);
        }
    }
}