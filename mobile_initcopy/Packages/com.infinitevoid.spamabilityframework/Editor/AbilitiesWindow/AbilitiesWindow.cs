using System.Collections.Generic;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow
{
    public class AbilitiesWindow : EditorWindow
    {
        private static List<AbilityBaseSO> _allAbilities;
        private static List<AbilityBaseSO> _currentAbilitySet;
        private ListView _abilitiesListView;
        private Box _contentBox;
        
        

        [MenuItem("Window/Abilities and Effects", false, 74)]
        [MenuItem("Tools/Abilities and Effects", false, 74)]
        public static void ShowWindow()
        {
            AbilitiesWindow wnd = GetWindow<AbilitiesWindow>("Abilities");
            GetWindow<EffectsWindow.EffectsWindow>("Effects", typeof(AbilitiesWindow));
            GetWindow<AbilitiesWindow>("Abilities");
            
            wnd.minSize = new Vector2(300, 300);
            wnd.Show();
        }

        private void OnFocus()
        {
            var searchbutton = rootVisualElement.Q<TextField>("search");
            if (searchbutton == null) return;
            var input = searchbutton.Q<VisualElement>("unity-text-input");
            input.Focus();
            searchbutton.SelectAll();
        }

        public void CreateGUI()
        {
            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/AbilitiesWindow.uxml");
            VisualElement tree = visualTree.Instantiate();
            tree.StretchToParentSize();
            rootVisualElement.Add(tree);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/AbilitiesWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Common/CommonStyles.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{EditorUtils.PACKAGE_BASE_PATH}/Editor/AbilitiesWindow/CreateNewAbilityPage.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _contentBox = rootVisualElement.Q<Box>(className: "c-content-container");

            _allAbilities = GetAllAbilities();
            _currentAbilitySet = _allAbilities;
            var unknownIcon = AssetDatabase.LoadAssetAtPath<Texture>($"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Textures/unknown.png");
            _abilitiesListView = rootVisualElement.Q<ListView>(className: "c-abilities-list-view");

            var searchbutton = rootVisualElement.Q<TextField>("search");
            searchbutton.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (string.IsNullOrWhiteSpace(evt.newValue))
                {
                    _currentAbilitySet = _allAbilities;
                }
                else
                {
                    var searchText = evt.newValue.ToLower();
                    _currentAbilitySet = _allAbilities.Where(ab => ab.Name.ToLower().Contains(searchText) || ab.name.ToLower().Contains(searchText)).ToList();
                    
                }

                _abilitiesListView.itemsSource = _currentAbilitySet;
                _abilitiesListView.Rebuild();
            });

            // _abilitiesListView.styleSheets.Add(styleSheet);
            _abilitiesListView.makeItem = () =>
            {
                var box = new Box();
                var image = new Image();
                var label = new Label();
                box.Add(image);
                box.Add(label);
                return box;
            };
            _abilitiesListView.bindItem = (e, i) =>
            {
                var box = e as Box;
                var label = box.Q<Label>();
                label.text = string.IsNullOrWhiteSpace(_currentAbilitySet[i].Name)
                    ? _currentAbilitySet[i].name
                    : _currentAbilitySet[i].Name;
                var image = box.Q<Image>();
                if (_currentAbilitySet[i].Icon)
                    image.image = _currentAbilitySet[i].Icon.texture;
                else image.image = unknownIcon;
            };
            _abilitiesListView.itemsSource = _currentAbilitySet.ToList();
            _abilitiesListView.selectionType = SelectionType.Single;
            _abilitiesListView.onSelectionChange += OnAbilitySelected;

            var createNewButton = rootVisualElement.Q<Button>("create-new-ability");
            createNewButton.clicked += SwitchToCreateNewAbilityView;
        }

        public static void ReloadAbilities()
        {
            _allAbilities = GetAllAbilities();
            _currentAbilitySet = _allAbilities;
            AbilitiesWindow wnd = GetWindow<AbilitiesWindow>();
            var listView = wnd.rootVisualElement.Q<ListView>(className: "c-abilities-list-view");
            listView.itemsSource = _currentAbilitySet.ToList();
            listView.Rebuild();
            listView.SetSelection(0);
        }

        public static void SetSelectionAsSelected()
        {
            AbilitiesWindow wnd = GetWindow<AbilitiesWindow>();
            var selectedIndex = _currentAbilitySet.IndexOf(Selection.activeObject as AbilityBaseSO);
            var listView = wnd.rootVisualElement.Q<ListView>(className: "c-abilities-list-view");
            listView.ClearSelection();
            listView.SetSelection(selectedIndex);
        }

        private void SwitchToCreateNewAbilityView()
        {
            _contentBox.Clear();
            _contentBox.Add(new CreateNewAbilityPage(_contentBox));
        }

        private static List<AbilityBaseSO> GetAllAbilities()
        {
            return AssetDatabase
                .FindAssets("t:AbilityBaseSO", null)
                .Select(guid => AssetDatabase.LoadAssetAtPath<AbilityBaseSO>(AssetDatabase.GUIDToAssetPath(guid)))
                .OrderBy(x => x.Name)
                .ToList();
        }


        private void OnAbilitySelected(IEnumerable<object> objects)
        {
            foreach (var item in objects)
            {
                _contentBox.Clear();

                var ability = (AbilityBaseSO) item;
                if (ability == null)
                    return;
                _contentBox.Add(new AbilityInfoPage.AbilityInfoPage(ability, _contentBox, _abilitiesListView.Rebuild));
            }
        }
    }
}