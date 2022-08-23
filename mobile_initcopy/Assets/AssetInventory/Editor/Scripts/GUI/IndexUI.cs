// reference for built-in icons: https://github.com/halak/unity-editor-icons

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JD.EditorAudioUtils;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AssetInventory
{
    public class IndexUI : BasicEditorUI
    {
        private const float SearchDelay = 0.3f;
        private readonly bool _debugMode = false;

        private List<AssetInfo> _files;
        private GUIContent[] _contents;
        private string[] _assetNames;
        private string[] _publisherNames;
        private string[] _categoryNames;
        private string[] _types;
        private string[] _resultSizes;
        private string[] _sortFields;
        private string[] _tileTitle;
        private string[] _groupByOptions;

        private int _gridSelection;
        private string _searchPhrase;
        private string _searchWidth;
        private string _searchHeight;
        private string _searchLength;
        private bool _checkMaxWidth;
        private bool _checkMaxHeight;
        private bool _checkMaxLength;
        private int _selectedPublisher;
        private int _selectedCategory;
        private int _selectedType;
        private int _selectedAsset;
        private bool _showSettings;
        private int _tab;
        private int _lastMainProgress;

        private Vector2 _searchScrollPos;
        private Vector2 _folderScrollPos;
        private Vector2 _assetScrollPos;
        private Vector2 _inspectorScrollPos;
        private Vector2 _usedAssetsScrollPos;

        private int _curPage = 1;
        private int _pageCount;

        private bool _previewInProgress;
        private EditorCoroutine _textureLoading;

        private string _token;

        private int _selectedFolderIndex;
        private AssetInfo _selectedEntry;
        private bool _showAdditionalFilters;
        private bool _showMaintenance;
        private bool _packageAvailable;
        private long _dbSize;
        private long _cacheSize;
        private int _resultCount;
        private int _packageCount;
        private int _packageFileCount;

        private AssetPurchases _purchasedAssets = new AssetPurchases();
        private int _purchasedAssetsCount;
        private List<AssetInfo> _assets;
        private int _indexedPackageCount;

        private List<AssetInfo> _assetUsage;
        private List<string> _usedAssets;
        private List<AssetInfo> _identifiedAssets;

        private SearchField SearchField => _searchField = _searchField ?? new SearchField();
        private SearchField _searchField;
        private float _nextSearchTime;
        [SerializeField] MultiColumnHeaderState _assetMCHState;

        private Rect AssetTreeRect => new Rect(20, 0, position.width - 40, position.height - 60);

        private TreeViewWithTreeModel<AssetInfo> AssetTreeView
        {
            get
            {
                if (_assetTreeViewState == null) _assetTreeViewState = new TreeViewState();

                MultiColumnHeaderState headerState = AssetTreeViewControl.CreateDefaultMultiColumnHeaderState(AssetTreeRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(_assetMCHState, headerState)) MultiColumnHeaderState.OverwriteSerializedFields(_assetMCHState, headerState);
                _assetMCHState = headerState;

                if (_assetTreeView == null)
                {
                    MultiColumnHeader mch = new MultiColumnHeader(headerState);
                    mch.canSort = false;
                    mch.height = MultiColumnHeader.DefaultGUI.minimumHeight;
                    mch.ResizeToFit();

                    _assetTreeView = new AssetTreeViewControl(_assetTreeViewState, mch, _assetTreeModel);
                    _assetTreeView.OnSelectionChanged += OnAssetTreeSelectionChanged;
                    _assetTreeView.Reload();
                }
                return _assetTreeView;
            }
        }

        private TreeViewWithTreeModel<AssetInfo> _assetTreeView;
        private TreeViewState _assetTreeViewState;
        private TreeModel<AssetInfo> _assetTreeModel = new TreeModel<AssetInfo>(new List<AssetInfo> {new AssetInfo().WithTreeData("Root", depth: -1)});
        private AssetInfo _selectedTreeAsset;

        private static bool _didReloadScripts;

        [MenuItem("Assets/Asset Inventory", priority = 9000)]
        public static void ShowWindow()
        {
            IndexUI window = GetWindow<IndexUI>("Asset Inventory");
            window.minSize = new Vector2(550, 200);
        }

        private void Awake()
        {
            AssetInventory.Init();
            InitUI();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnplayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
        }

        private void InitUI()
        {
            ReloadLookups();
            PerformSearch();
        }

        private void OnplayModeStateChanged(PlayModeStateChange state)
        {
            // UI will have lost all preview textures during play mode
            if (state == PlayModeStateChange.EnteredEditMode) PerformSearch();
        }

        private void ReloadLookups()
        {
            _resultSizes = new[] {"-All-", string.Empty, "10", "25", "50", "100", "250", "500", "1000"};
            _sortFields = new[] {"Asset", "File Name", "Size", "Type", "Length", "Width", "Height", "Category", "Last Updated", "Rating", "#Ratings"};
            _groupByOptions = new[] {"-None-", "Category"};
            _tileTitle = new[] {"Asset Path", "File Name", "File Name without Extension", "None"};

            _assets = AssetInventory.LoadAssets();
            _assetNames = AssetInventory.ExtractAssetNames(_assets);
            _publisherNames = AssetInventory.ExtractPublisherNames(_assets);
            _categoryNames = AssetInventory.ExtractCategoryNames(_assets);
            _types = AssetInventory.LoadTypes();
            _token = CloudProjectSettings.accessToken;
            _purchasedAssetsCount = AssetInventory.CountPurchasedAssets(_assets);

            UpdateStatistics();
        }

        [DidReloadScripts]
        private static void DidReloadScripts()
        {
            _didReloadScripts = true;
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("The asset inventory is not available during play mode.", MessageType.Info);
                return;
            }

            if (_didReloadScripts)
            {
                _didReloadScripts = false;
                ReloadLookups();
            }

            UIStyles.tile.fixedHeight = AssetInventory.Config.tileSize;
            UIStyles.tile.fixedWidth = AssetInventory.Config.tileSize;

            bool newTab = DrawToolbar();
            EditorGUILayout.Space();

            switch (_tab)
            {
                case 0:
                    DrawSearchTab();
                    break;

                case 1:
                    DrawAssetsTab();
                    break;

                case 2:
                    if (newTab) UpdateStatistics();
                    DrawIndexTab();
                    break;

                case 3:
                    DrawReportingTab();
                    break;

                case 4:
                    DrawAboutTab();
                    break;
            }

            // reload if there is new data
            if (_lastMainProgress != AssertImporter.MainProgress)
            {
                _lastMainProgress = AssertImporter.MainProgress;
                ReloadLookups();
                PerformSearch(true);
            }
        }

        private bool DrawToolbar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            List<string> strings = new List<string>
            {
                "Search",
                "Assets" + (AssetInventory.CurrentMain != null ? " (in progress)" : ""),
                "Index" + (AssetInventory.IndexingInProgress ? " (in progress)" : ""),
                "Reporting",
                "About"
            };
            _tab = GUILayout.Toolbar(_tab, strings.ToArray(), GUILayout.Height(32), GUILayout.MinWidth(500));
            bool newTab = EditorGUI.EndChangeCheck();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return newTab;
        }

        private void DrawSearchTab()
        {
            if (_packageFileCount == 0)
            {
                EditorGUILayout.HelpBox("The search index needs to be initialized. Start it right from here or go to the Index tab to configure the details.", MessageType.Info);

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box(Logo, EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(300), GUILayout.MaxHeight(300));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.Space(30);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUI.BeginDisabledGroup(AssetInventory.IndexingInProgress);
                if (GUILayout.Button("Start Indexing", GUILayout.Height(50), GUILayout.MaxWidth(400))) StartInitialIndexing();
                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                bool dirty = false;
                EditorGUIUtility.labelWidth = 60;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Search:", GUILayout.Width(60));
                EditorGUI.BeginChangeCheck();
                _searchPhrase = SearchField.OnGUI(_searchPhrase, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    // delay search to allow fast typing
                    _nextSearchTime = Time.realtimeSinceStartup + SearchDelay;
                }
                else if (_nextSearchTime > 0 && Time.realtimeSinceStartup > _nextSearchTime)
                {
                    _nextSearchTime = 0;
                    dirty = true;
                }
                EditorGUI.BeginChangeCheck();
                GUILayout.Space(10);
                _selectedType = EditorGUILayout.Popup(_selectedType, _types, GUILayout.ExpandWidth(false), GUILayout.MinWidth(85));
                GUILayout.Space(10);
                _showAdditionalFilters = EditorGUILayout.Foldout(_showAdditionalFilters, "More");
                if (EditorGUI.EndChangeCheck()) dirty = true;
                GUILayout.EndHorizontal();

                if (_showAdditionalFilters)
                {
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    _selectedAsset = EditorGUILayout.Popup("Asset:", _selectedAsset, _assetNames, GUILayout.ExpandWidth(true), GUILayout.MinWidth(200));
                    GUILayout.Space(10);
                    _selectedPublisher = EditorGUILayout.Popup("Publisher:", _selectedPublisher, _publisherNames, GUILayout.ExpandWidth(true), GUILayout.MinWidth(150));
                    GUILayout.Space(10);
                    _selectedCategory = EditorGUILayout.Popup("Category:", _selectedCategory, _categoryNames, GUILayout.ExpandWidth(true), GUILayout.MinWidth(150));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Width:", GUILayout.Width(60));
                    if (GUILayout.Button(_checkMaxWidth ? "<=" : ">=", GUILayout.Width(25))) _checkMaxWidth = !_checkMaxWidth;
                    _searchWidth = EditorGUILayout.TextField(_searchWidth, GUILayout.Width(58));

                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Height:", GUILayout.Width(60));
                    if (GUILayout.Button(_checkMaxHeight ? "<=" : ">=", GUILayout.Width(25))) _checkMaxHeight = !_checkMaxHeight;
                    _searchHeight = EditorGUILayout.TextField(_searchHeight, GUILayout.Width(58));

                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Length:", GUILayout.Width(60));
                    if (GUILayout.Button(_checkMaxLength ? "<=" : ">=", GUILayout.Width(25))) _checkMaxLength = !_checkMaxLength;
                    _searchLength = EditorGUILayout.TextField(_searchLength, GUILayout.Width(58));

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck()) dirty = true;
                }

                // result
                EditorGUILayout.Space();
                if (_contents.Length > 0)
                {
                    if (_files == null) PerformSearch(); // happens during recompilation

                    GUILayout.BeginHorizontal();

                    // assets
                    GUILayout.BeginVertical();
                    EditorGUI.BeginChangeCheck();
                    _searchScrollPos = GUILayout.BeginScrollView(_searchScrollPos, false, false);
                    if (EditorGUI.EndChangeCheck())
                    {
                        // TODO: implement paged endless scrolling, needs some pixel calculations though
                        // if (_textureLoading != null) EditorCoroutineUtility.StopCoroutine(_textureLoading);
                        // _textureLoading = EditorCoroutineUtility.StartCoroutine(LoadTextures(false), this);
                    }

                    int cells = Mathf.RoundToInt(Mathf.Clamp(Mathf.Floor((position.width - UIStyles.INSPECTOR_WIDTH - UIStyles.BORDER_WIDTH) / AssetInventory.Config.tileSize), 1, 99));
                    if (cells < 2) cells = 2;
                    EditorGUI.BeginChangeCheck();
                    _gridSelection = GUILayout.SelectionGrid(_gridSelection, _contents, cells, UIStyles.tile);
                    bool isAudio = AssetInventory.IsFileType(_selectedEntry?.Path, "Audio");
                    if (EditorGUI.EndChangeCheck())
                    {
                        _showSettings = false;
                        _selectedEntry = _files[_gridSelection];
                        EditorAudioUtility.StopAllPreviewClips();
                        isAudio = AssetInventory.IsFileType(_selectedEntry?.Path, "Audio");
                        if (AssetInventory.Config.autoPlayAudio && isAudio && !_previewInProgress) PlayAudio(_selectedEntry);
                        if (_selectedEntry != null)
                        {
                            _selectedEntry.ProjectPath = AssetDatabase.GUIDToAssetPath(_selectedEntry.Guid);
                            _selectedEntry.IsMaterialized = AssetInventory.IsMaterialized(_selectedEntry.ToAsset(), _selectedEntry);
                            EditorCoroutineUtility.StartCoroutine(AssetUtils.LoadTexture(_selectedEntry), this);

                            // if entry is already materialized calculate dependencies immediately
                            if (!_previewInProgress && _selectedEntry.DepState == AssetInfo.DependencyState.Unknown && _selectedEntry.IsMaterialized) CalculateDependencies(_selectedEntry);

                            _packageAvailable = File.Exists(_selectedEntry.Location);
                            if (AssetInventory.Config.pingSelected && _selectedEntry.InProject) PingAsset(_selectedEntry);
                        }
                        else
                        {
                            _packageAvailable = false;
                        }
                    }

                    GUILayout.EndScrollView();

                    // navigation
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (_pageCount > 1)
                    {
                        EditorGUI.BeginDisabledGroup(_curPage <= 1);
                        if (GUILayout.Button("<", GUILayout.ExpandWidth(false))) SetPage(_curPage - 1);
                        EditorGUI.EndDisabledGroup();

                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.LabelField(new GUIContent($"Page {_curPage}/{_pageCount}", $"{_resultCount} results in total"), UIStyles.centerLabel, GUILayout.Width(150));
                        EditorGUIUtility.labelWidth = 60;

                        EditorGUI.BeginDisabledGroup(_curPage >= _pageCount);
                        if (GUILayout.Button(">", GUILayout.ExpandWidth(false))) SetPage(_curPage + 1);
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"{_resultCount} results", UIStyles.centerLabel, GUILayout.ExpandWidth(true));
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(EditorGUIUtility.IconContent("Settings", "Show/Hide Settings Tab").image)) _showSettings = !_showSettings;
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    GUILayout.EndVertical();

                    // inspector
                    string[] selection = Selection.assetGUIDs;
                    string selectedPath = null;
                    if (selection != null && selection.Length > 0)
                    {
                        selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
                        if (!Directory.Exists(selectedPath)) selectedPath = null;
                    }

                    GUILayout.BeginVertical();
                    GUILayout.BeginVertical("Details Inspector", "window", GUILayout.Width(UIStyles.INSPECTOR_WIDTH));
                    EditorGUILayout.Space();
                    _inspectorScrollPos = GUILayout.BeginScrollView(_inspectorScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
                    if (_selectedEntry == null || string.IsNullOrEmpty(_selectedEntry.SafeName))
                    {
                        // will happen after script reload
                        EditorGUILayout.HelpBox("Select an asset for details", MessageType.Info);
                    }
                    else
                    {
                        if (!_packageAvailable)
                        {
                            EditorGUILayout.HelpBox("Not cached anymore. Download the asset in the Package Manager again.", MessageType.Info);
                            EditorGUILayout.Space();
                        }

                        EditorGUILayout.LabelField("File", EditorStyles.largeLabel);
                        GUILabelWithText("Name", Path.GetFileName(_selectedEntry.Path));
                        GUILabelWithText("Size", EditorUtility.FormatBytes(_selectedEntry.Size));
                        if (_selectedEntry.Width > 0) GUILabelWithText("Dimensions", $"{_selectedEntry.Width}x{_selectedEntry.Height} px");
                        if (_selectedEntry.Length > 0) GUILabelWithText("Length", $"{_selectedEntry.Length} seconds");
                        GUILabelWithText("In Project", _selectedEntry.InProject ? "Yes" : "No");
                        if (_packageAvailable)
                        {
                            if (AssetInventory.ScanDependencies.Contains(_selectedEntry.Type))
                            {
                                switch (_selectedEntry.DepState)
                                {
                                    case AssetInfo.DependencyState.Unknown:
                                        GUILayout.BeginHorizontal();
                                        EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel, GUILayout.Width(85));
                                        EditorGUI.BeginDisabledGroup(_previewInProgress);
                                        if (GUILayout.Button("Calculate", GUILayout.ExpandWidth(false))) CalculateDependencies(_selectedEntry);
                                        EditorGUI.EndDisabledGroup();
                                        GUILayout.EndHorizontal();
                                        break;

                                    case AssetInfo.DependencyState.Calculating:
                                        GUILabelWithText("Dependencies", "Calculating...");
                                        break;

                                    case AssetInfo.DependencyState.NotPossible:
                                        GUILabelWithText("Dependencies", "Cannot determine (binary)");
                                        break;

                                    case AssetInfo.DependencyState.Done:
                                        GUILayout.BeginHorizontal();
                                        GUILabelWithText("Dependencies", $"{_selectedEntry.MediaDependencies?.Count} + {_selectedEntry.ScriptDependencies?.Count} ({EditorUtility.FormatBytes(_selectedEntry.DependencySize)})");
                                        if (_selectedEntry.Dependencies.Count > 0 && GUILayout.Button("Show..."))
                                        {
                                            string list = string.Join("\n", _selectedEntry.Dependencies.Select(af => af.Path.Replace("Assets/", string.Empty).Replace(" ", "\u00A0")));
                                            EditorUtility.DisplayDialog("Asset Dependencies", list, "Close");
                                        }

                                        GUILayout.EndHorizontal();
                                        break;
                                }
                            }

                            if (!_selectedEntry.InProject && selectedPath == null)
                            {
                                EditorGUILayout.Space();
                                EditorGUILayout.LabelField("Select a folder in Project View for import options", EditorStyles.centeredGreyMiniLabel);
                            }
                            EditorGUI.BeginDisabledGroup(_previewInProgress);
                            if (!_selectedEntry.InProject && !string.IsNullOrEmpty(selectedPath))
                            {
                                GUILabelWithText("Import To", selectedPath);
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Import File" + (_selectedEntry.DependencySize > 0 ? " Only" : ""))) CopyTo(_selectedEntry, selectedPath);
                                if (_selectedEntry.DependencySize > 0 && AssetInventory.ScanDependencies.Contains(_selectedEntry.Type))
                                {
                                    if (GUILayout.Button("Import With Dependencies")) CopyTo(_selectedEntry, selectedPath, true);
                                    if (_selectedEntry.ScriptDependencies.Count > 0)
                                    {
                                        if (GUILayout.Button("Import With Dependencies + Scripts")) CopyTo(_selectedEntry, selectedPath, true, true);
                                    }

                                    EditorGUILayout.Space();
                                }
                            }
                            else
                            {
                                EditorGUILayout.Space();
                            }

                            if (isAudio)
                            {
                                if (GUILayout.Button("Play")) PlayAudio(_selectedEntry);
                            }

                            if (_selectedEntry.InProject)
                            {
                                if (GUILayout.Button("Ping")) PingAsset(_selectedEntry);
                            }

                            if (GUILayout.Button("Open")) Open(_selectedEntry);
                            if (GUILayout.Button("Open in Explorer")) OpenExplorer(_selectedEntry);

                            if (!_selectedEntry.IsMaterialized && !_previewInProgress)
                            {
                                EditorGUILayout.LabelField("Asset will be extracted before any actions are performed", EditorStyles.centeredGreyMiniLabel);
                            }

                            if (isAudio)
                            {
                                EditorGUILayout.Space();
                                AssetInventory.Config.autoPlayAudio = GUILayout.Toggle(AssetInventory.Config.autoPlayAudio, "Automatically play selected audio");
                            }
                        }

                        if (_previewInProgress)
                        {
                            EditorGUILayout.LabelField("Extracting...", EditorStyles.centeredGreyMiniLabel);
                        }

                        EditorGUILayout.Space();
                        DrawAssetDetails(_selectedEntry);
                    }

                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                    if (_showSettings)
                    {
                        EditorGUILayout.Space();
                        GUILayout.BeginVertical("Settings", "window", GUILayout.Width(UIStyles.INSPECTOR_WIDTH));
                        EditorGUILayout.Space();

                        EditorGUI.BeginChangeCheck();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Sort by:", "Specify sort order"), GUILayout.Width(100));
                        AssetInventory.Config.sortField = EditorGUILayout.Popup(AssetInventory.Config.sortField, _sortFields);
                        if (GUILayout.Button(AssetInventory.Config.sortDescending ? new GUIContent("˅", "Descending") : new GUIContent("˄", "Ascending"), GUILayout.Width(15)))
                        {
                            AssetInventory.Config.sortDescending = !AssetInventory.Config.sortDescending;
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Results:", "Maximum number of results to show"), GUILayout.Width(100));
                        AssetInventory.Config.maxResults = EditorGUILayout.Popup(AssetInventory.Config.maxResults, _resultSizes);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            dirty = true;
                            _curPage = 1;
                            AssetInventory.SaveConfig();
                        }

                        EditorGUILayout.Space();
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Tile Size:", "Dimensions of search result previews. Preview images will still be 128x128 max."), GUILayout.Width(100));
                        AssetInventory.Config.tileSize = EditorGUILayout.IntSlider(AssetInventory.Config.tileSize, 50, 300);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck()) AssetInventory.SaveConfig();

                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Tile Text:", "Text to be shown on the tile"), GUILayout.Width(100));
                        AssetInventory.Config.tileText = EditorGUILayout.Popup(AssetInventory.Config.tileText, _tileTitle);
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            dirty = true;
                            AssetInventory.SaveConfig();
                        }

                        EditorGUILayout.Space();
                        EditorGUI.BeginChangeCheck();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Auto-Play Audio:", "Will automatically extract unity packages to play the sound file if they were not extracted yet. This is the most convenient option but will require sufficient hard disk space."), GUILayout.Width(100));
                        AssetInventory.Config.autoPlayAudio = EditorGUILayout.Toggle(AssetInventory.Config.autoPlayAudio);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Ping Selected:", "Highlight selected items in the Unity project tree if they are found in the current project."), GUILayout.Width(100));
                        AssetInventory.Config.pingSelected = EditorGUILayout.Toggle(AssetInventory.Config.pingSelected);
                        GUILayout.EndHorizontal();

                        if (EditorGUI.EndChangeCheck()) AssetInventory.SaveConfig();

                        EditorGUILayout.Space();
                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label("No Results", UIStyles.whiteCenter, GUILayout.MinHeight(AssetInventory.Config.tileSize));
                }

                if (dirty) PerformSearch();
                EditorGUIUtility.labelWidth = 0;
            }
        }

        private void DrawAssetDetails(AssetInfo info, bool showMaintenance = false)
        {
            EditorGUILayout.LabelField("Asset", EditorStyles.largeLabel);
            GUILabelWithText("Name", Path.GetFileName(info.GetDisplayName));
            if (!string.IsNullOrWhiteSpace(info.GetDisplayPublisher)) GUILabelWithText("Publisher", $"{info.GetDisplayPublisher}");
            if (!string.IsNullOrWhiteSpace(info.GetDisplayCategory)) GUILabelWithText("Category", $"{info.GetDisplayCategory}");
            if (info.PackageSize > 0) GUILabelWithText("Size", EditorUtility.FormatBytes(info.PackageSize));
            if (!string.IsNullOrWhiteSpace(info.AssetRating)) GUILabelWithText("Rating", $"{info.AssetRating} stars ({info.RatingCount} ratings)");
            if (info.AssetSource == Asset.Source.AssetStorePackage && info.LastRelease.Year > 1)
            {
                GUILabelWithText("Last Update", info.LastRelease.ToLongDateString() + (!string.IsNullOrEmpty(info.Version) ? $" ({info.Version})" : string.Empty));
            }
            GUILabelWithText("Source", info.AssetSource == Asset.Source.AssetStorePackage ? "Asset Store" : "Custom Package");
            if (info.AssetSource == Asset.Source.AssetStorePackage || info.AssetSource == Asset.Source.CustomPackage)
            {
                EditorGUILayout.Space();
                if (info.Downloaded)
                {
                    if (GUILayout.Button("Import Full Package")) AssetDatabase.ImportPackage(info.Location, true);
                    if (showMaintenance && GUILayout.Button("Reindex Package on Next Run")) AssetInventory.ForgetAsset(info.AssetId);
                }
            }

            EditorGUI.EndDisabledGroup();
            if (info.PreviewTexture != null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Box(info.PreviewTexture, EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(UIStyles.INSPECTOR_WIDTH), GUILayout.MaxHeight(100));
                GUILayout.FlexibleSpace();
            }
        }

        private void DrawIndexTab()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            // folders
            EditorGUILayout.LabelField("Unity Asset Store downloads will be indexed automatically. Specify custom locations below to scan for unitypackages downloaded from somewhere else than the Asset Store.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            AssetInventory.Config.indexAssetStore = GUILayout.Toggle(AssetInventory.Config.indexAssetStore, "Index Asset Store Downloads");
            if (EditorGUI.EndChangeCheck()) AssetInventory.SaveConfig();
#if UNITY_2022_1_OR_NEWER
            EditorGUILayout.LabelField("Only the default asset cache location will be scanned. Custom locations are not yet supported.", EditorStyles.miniLabel);
#endif
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Additional Folders to Index", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            _folderScrollPos = GUILayout.BeginScrollView(_folderScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandWidth(true));
            Event currentEvent = Event.current;
            if (AssetInventory.Config.folders.Count == 0)
            {
                EditorGUILayout.HelpBox("Use the + button to the right to add additional folders to scan", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < AssetInventory.Config.folders.Count; i++)
                {
                    FolderSpec spec = AssetInventory.Config.folders[i];
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(8);

                    EditorGUILayout.BeginHorizontal();
                    Rect toggleRect = GUILayoutUtility.GetRect(GUIContent.none, UIStyles.toggleStyle);
                    GUILayout.Space(4);
                    Rect nameRect = GUILayoutUtility.GetRect(GUIContent.none, UIStyles.entryStyle, GUILayout.ExpandWidth(true));
                    EditorGUILayout.EndHorizontal();

                    Rect entryRect = GUILayoutUtility.GetLastRect();
                    EditorGUILayout.EndHorizontal();

                    if (currentEvent.type == EventType.Repaint)
                    {
                        if (_selectedFolderIndex == i)
                        {
                            UIStyles.background.Draw(entryRect, false, false, true, false);
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                UIStyles.evenRow.Draw(entryRect, false, false, false, false);
                            }
                            else
                            {
                                UIStyles.oddRow.Draw(entryRect, false, false, false, false);
                            }
                        }
                    }

                    EditorGUI.BeginChangeCheck();
                    spec.Enabled = GUI.Toggle(toggleRect, spec.Enabled, GUIContent.none, UIStyles.toggleStyle);
                    if (EditorGUI.EndChangeCheck()) _selectedFolderIndex = i;
                    GUI.Label(nameRect, spec.Location, UIStyles.entryStyle);

                    DoMouseEvent(entryRect, i);
                }
            }

            GUILayout.EndScrollView();

            EditorGUILayout.BeginVertical(GUILayout.Width(UIStyles.BUTTON_WIDTH), GUILayout.Height(140));
            GUILayout.Space(2);
            if (GUILayout.Button("+", UIStyles.buttonStyles))
            {
                OnAddButtonClicked();
            }

            EditorGUI.BeginDisabledGroup(_selectedFolderIndex == -1 || AssetInventory.Config.folders.Count == 0);
            if (GUILayout.Button("-", UIStyles.buttonStyles))
            {
                OnRemoveButtonClicked();
                GUIUtility.keyboardControl = 0;
            }

            EditorGUI.EndDisabledGroup();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // status
            if (AssetInventory.IndexingInProgress)
            {
                EditorGUILayout.Space();
                if (AssertImporter.MainCount > 0)
                {
                    EditorGUILayout.LabelField($"Indexing Package: {AssertImporter.MainProgress}/{AssertImporter.MainCount} " + Path.GetFileName(AssertImporter.CurrentMain));
                }

                if (AssertImporter.SubCount > 0)
                {
                    EditorGUILayout.LabelField($"Indexing File: {AssertImporter.SubProgress}/{AssertImporter.SubCount} " + Path.GetFileName(AssertImporter.CurrentSub));
                }
            }

            // commands
            EditorGUILayout.Space();
            if (AssetInventory.IndexingInProgress)
            {
                EditorGUI.BeginDisabledGroup(AssertImporter.CancellationRequested);
                if (GUILayout.Button("Stop Indexing")) AssertImporter.CancellationRequested = true;
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (GUILayout.Button("Update Index", GUILayout.Height(50))) EditorCoroutineUtility.StartCoroutine(AssetInventory.RefreshIndex(), this);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Statistics", "window", GUILayout.Width(UIStyles.INSPECTOR_WIDTH));
            EditorGUILayout.Space();
            GUILabelWithText("Indexed Packages", $"{_indexedPackageCount}/{_packageCount}", 120);
            GUILabelWithText("Indexed Files", $"{_packageFileCount}", 120);
            GUILabelWithText("Database Size", EditorUtility.FormatBytes(_dbSize), 120);
            GUILabelWithText("Cache Size", EditorUtility.FormatBytes(_cacheSize), 120);

            if (_indexedPackageCount < _packageCount && !AssetInventory.IndexingInProgress)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("To index the remaining assets, download them in the Package Manager. Tip: Use Unity 2022.1 or newer to bulk download many/all assets at once.", MessageType.Info);
            }

            EditorGUILayout.Space();
            _showMaintenance = EditorGUILayout.Foldout(_showMaintenance, "Maintenance Functions");
            if (_showMaintenance)
            {
                EditorGUI.BeginDisabledGroup(AssetInventory.IndexingInProgress);
                if (GUILayout.Button("Clear Database"))
                {
                    if (!DBAdapter.DeleteDB()) EditorUtility.DisplayDialog("Error", "Database seems to be in use by another program and could not be cleared.", "OK");
                    UpdateStatistics();
                    _assets = new List<AssetInfo>();
                }

                EditorGUI.BeginDisabledGroup(AssetInventory.ClearCacheInProgress);
                if (GUILayout.Button("Clear Cache")) AssetInventory.ClearCache(UpdateStatistics);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Reset Configuration")) AssetInventory.ResetConfig();
                if (DBAdapter.IsDBOpen())
                {
                    if (GUILayout.Button("Close Database (to allow copying)")) DBAdapter.Close();
                }

                EditorGUI.BeginDisabledGroup(AssetInventory.CurrentMain != null || AssetInventory.IndexingInProgress);
                if (GUILayout.Button("Change Database Location...")) SetDatabaseLocation();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Database Location", EditorStyles.boldLabel);
                EditorGUILayout.SelectableLabel(AssetInventory.GetStorageFolder(), EditorStyles.wordWrappedLabel);

                EditorGUILayout.LabelField(new GUIContent("Config Location", "Copy the file into your project to use a project-specific configuration instead."), EditorStyles.boldLabel);
                EditorGUILayout.SelectableLabel(AssetInventory.UsedConfigLocation, EditorStyles.wordWrappedLabel);

                EditorGUI.EndDisabledGroup();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawAssetsTab()
        {
            GUILayout.BeginHorizontal();

            // asset list
            GUILayout.BeginVertical();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.BeginChangeCheck();
            AssetInventory.Config.assetGrouping = EditorGUILayout.Popup(new GUIContent("Group by:", "Select if assets should be grouped or not"), AssetInventory.Config.assetGrouping, _groupByOptions, GUILayout.Width(140));
            if (EditorGUI.EndChangeCheck()) CreateAssetTree();
            EditorGUIUtility.labelWidth = 0;
            if (GUILayout.Button("Collapse All", GUILayout.ExpandWidth(false))) AssetTreeView.CollapseAll();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (_assets.Count > 0)
            {
                AssetTreeView.OnGUI(new Rect(0, 80, position.width - UIStyles.INSPECTOR_WIDTH - 5, position.height - 80));
            }
            else
            {
                EditorGUILayout.HelpBox("No assets were indexed yet. Start the indexing process to fill this list.", MessageType.Info);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Overview", "window", GUILayout.Width(UIStyles.INSPECTOR_WIDTH));
            EditorGUILayout.Space();
            GUILabelWithText("Total", _assets.Count.ToString());
            GUILabelWithText("Indexed", _indexedPackageCount.ToString());
            if (string.IsNullOrEmpty(_token))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Could not retrieve Asset Store purchases. Open the package manager once and go to the My Assets section.", MessageType.Warning);
                if (GUILayout.Button("Retry")) _token = CloudProjectSettings.accessToken;
                EditorGUILayout.Space();
            }
            else
            {
                GUILabelWithText("Asset Store", _purchasedAssetsCount.ToString());

                EditorGUILayout.Space();
                if (AssetInventory.CurrentMain != null)
                {
                    EditorGUILayout.LabelField($"{AssetInventory.CurrentMain} {AssetInventory.MainProgress}/{AssetInventory.MainCount}", EditorStyles.centeredGreyMiniLabel);
                }
                EditorGUI.BeginDisabledGroup(AssetInventory.CurrentMain != null);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Retrieve & Update Purchases")) FetchAssetPurchases();
                EditorGUI.EndDisabledGroup();
                if (AssetInventory.CurrentMain != null)
                {
                    if (GUILayout.Button("Cancel")) AssetStore.CancellationRequested = true;
                }
                GUILayout.EndHorizontal();
                if (_debugMode && GUILayout.Button("Update Asset Details")) FetchAssetDetails();
            }
            if (GUILayout.Button("Open Package Manager")) UnityEditor.PackageManager.UI.Window.Open("");

            if (_selectedTreeAsset != null)
            {
                EditorGUILayout.Space();
                DrawAssetDetails(_selectedTreeAsset, true);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawReportingTab()
        {
            int assetUsageCount = _assetUsage?.Count ?? 0;
            int identifiedAssetsCount = _identifiedAssets?.Count ?? 0;

            EditorGUILayout.HelpBox("This view tries to identify used assets inside the current project. It will use guids of the assets. If asset owners have copied files from somewhere this can result in false positives. The view is a preview with more functionality to come soon.", MessageType.Info);

            GUILabelWithText("Project files", assetUsageCount.ToString());
            if (assetUsageCount > 0)
            {
                GUILabelWithText("Identified files", identifiedAssetsCount + " (" + Mathf.RoundToInt((float) identifiedAssetsCount / assetUsageCount * 100f) + "%)");
            }
            else
            {
                GUILabelWithText("Identified files", "None");
            }

            if (_usedAssets != null && _usedAssets.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Identified Assets", EditorStyles.largeLabel);
                _usedAssetsScrollPos = GUILayout.BeginScrollView(_usedAssetsScrollPos, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
                foreach (string assetName in _usedAssets)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(8);
                    EditorGUILayout.LabelField(assetName);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Calculate Asset Usage", GUILayout.Height(50))) CalculateAssetUsage();
        }

        private void DrawAboutTab()
        {
            EditorGUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(Logo, EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(300), GUILayout.MaxHeight(300));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("A tool by Impossible Robert", UIStyles.whiteCenter);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Developer: Robert Wetzold", UIStyles.whiteCenter);
            EditorGUILayout.LabelField("www.wetzold.com/tools", UIStyles.whiteCenter);
            EditorGUILayout.LabelField($"Version {AssetInventory.ToolVersion}", UIStyles.whiteCenter);
            EditorGUILayout.Space(30);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.HelpBox("If you like this asset please consider leaving a review on the Unity Asset Store. Thanks a million!", MessageType.Info);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            if (_debugMode && GUILayout.Button("Get Token")) Debug.Log(CloudProjectSettings.accessToken);
            if (_debugMode && GUILayout.Button("Reload Lookups")) ReloadLookups();
        }

        private void BulkImportAssets(IList<int> assetIds)
        {
            if (assetIds.Count == 0) return;
            if (assetIds.Count > 1 || assetIds[0] < 0)
            {
                EditorUtility.DisplayDialog("Error", "Importing more than one asset at a time is not yet supported.", "OK");
                return;
            }

            AssetInfo asset = _assets.FirstOrDefault(info => info.AssetId == assetIds[0]);
            if (asset == null) return;
            if (!asset.Downloaded)
            {
                EditorUtility.DisplayDialog("Error", "The asset was not downloaded yet. Open the package manager and download it first.", "OK");
                return;
            }

            AssetDatabase.ImportPackage(asset.Location, true);
        }

        private void CalculateAssetUsage()
        {
            _assetUsage = AssetInventory.CalculateAssetUsage();
            _usedAssets = _assetUsage.Select(ai => ai.DisplayName).Distinct().Where(a => !string.IsNullOrEmpty(a)).OrderBy(s => s).ToList();
            _identifiedAssets = _assetUsage.Where(ai => ai.CurrentState != Asset.State.Unknown).ToList();
        }

        private void StartInitialIndexing()
        {
            EditorCoroutineUtility.StartCoroutine(AssetInventory.RefreshIndex(), this);

            // start also asset download if not already done before manually
            if (string.IsNullOrEmpty(AssetInventory.CurrentMain)) FetchAssetPurchases();
        }

        private void GUILabelWithText(string label, string text, int width = 85)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Width(width));
            EditorGUILayout.LabelField(text, GUILayout.MaxWidth(UIStyles.INSPECTOR_WIDTH - width));
            GUILayout.EndHorizontal();
        }

        private void SetDatabaseLocation()
        {
            string targetFolder = EditorUtility.OpenFolderPanel("Select folder for database and cache", AssetInventory.GetStorageFolder(), "");
            if (string.IsNullOrEmpty(targetFolder)) return;

            // check if same folder selected
            if (IOUtils.IsSameDirectory(targetFolder, AssetInventory.GetStorageFolder())) return;

            // check for existing database
            if (File.Exists(Path.Combine(targetFolder, DBAdapter.DB_NAME)))
            {
                if (EditorUtility.DisplayDialog("Use Existing?", "The target folder contains a database. Switch to this one? Otherwise please select an empty directory.", "Yes", "No"))
                {
                    AssetInventory.SwitchDatabase(targetFolder);
                    ReloadLookups();
                    PerformSearch();
                }

                return;
            }

            // target must be empty
            if (!IOUtils.IsDirectoryEmpty(targetFolder))
            {
                EditorUtility.DisplayDialog("Error", "The target folder needs to be empty or contain an existing database.", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog("Keep Old Database", "Should a new database be created or the current one moved?", "New", "Move"))
            {
                AssetInventory.SwitchDatabase(targetFolder);
                ReloadLookups();
                PerformSearch();
                return;
            }

            _previewInProgress = true;
            AssetInventory.MoveDatabase(targetFolder);
            _previewInProgress = false;
        }

        private void OnAddButtonClicked()
        {
            string folder = EditorUtility.OpenFolderPanel("Select folder to index", "", "");
            if (string.IsNullOrEmpty(folder)) return;

            FolderSpec spec = new FolderSpec();
            spec.Location = Path.GetFullPath(folder);
            AssetInventory.Config.folders.Add(spec);
            AssetInventory.SaveConfig();
        }

        private void OnRemoveButtonClicked()
        {
            if (_selectedFolderIndex < AssetInventory.Config.folders.Count) AssetInventory.Config.folders.RemoveAt(_selectedFolderIndex);
            AssetInventory.SaveConfig();
        }

        private void DoMouseEvent(Rect rect, int tagIndex)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                switch (e.button)
                {
                    case 0:
                        _selectedFolderIndex = _selectedFolderIndex == tagIndex ? -1 : tagIndex;
                        e.Use();
                        GUIUtility.keyboardControl = 0;
                        break;
                }
            }
        }

        private async void PlayAudio(AssetInfo assetInfo)
        {
            _previewInProgress = true;

            await AssetInventory.PlayAudio(assetInfo);

            _previewInProgress = false;
        }

        private void PingAsset(AssetInfo assetInfo)
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(assetInfo.ProjectPath);
            if (Selection.activeObject == null) assetInfo.ProjectPath = null; // probably got deleted again
        }

        private async void CalculateDependencies(AssetInfo assetInfo)
        {
            _previewInProgress = true;
            assetInfo.DepState = AssetInfo.DependencyState.Calculating;
            await AssetInventory.CalculateDependencies(_selectedEntry);
            if (assetInfo.DepState == AssetInfo.DependencyState.Calculating) assetInfo.DepState = AssetInfo.DependencyState.Done; // otherwise error along the way
            _previewInProgress = false;
        }

        private async void Open(AssetInfo assetInfo)
        {
            _previewInProgress = true;
            string targetPath;
            if (assetInfo.InProject)
            {
                targetPath = assetInfo.ProjectPath;
            }
            else
            {
                targetPath = await AssetInventory.EnsureMaterializedAsset(assetInfo);
            }

            if (targetPath != null) EditorUtility.OpenWithDefaultApp(targetPath);
            _previewInProgress = false;
        }

        private async void OpenExplorer(AssetInfo assetInfo)
        {
            _previewInProgress = true;
            string targetPath;
            if (assetInfo.InProject)
            {
                targetPath = assetInfo.ProjectPath;
            }
            else
            {
                targetPath = await AssetInventory.EnsureMaterializedAsset(assetInfo);
            }

            if (targetPath != null) EditorUtility.RevealInFinder(targetPath);
            _previewInProgress = false;
        }

        private async void CopyTo(AssetInfo assetInfo, string selectedPath, bool withDependencies = false, bool withScripts = false)
        {
            _previewInProgress = true;

            string mainFile = await AssetInventory.CopyTo(assetInfo, selectedPath, withDependencies, withScripts);
            if (mainFile != null) PingAsset(new AssetInfo().WithProjectPath(mainFile));

            _previewInProgress = false;
        }

        private async void FetchAssetPurchases()
        {
            AssetPurchases result = await AssetInventory.FetchOnlineAssets();
            if (AssetStore.CancellationRequested) return;

            _purchasedAssets = result;
            _purchasedAssetsCount = _purchasedAssets?.total ?? 0;
            ReloadLookups();
            FetchAssetDetails();
        }

        private void CreateAssetTree()
        {
            List<AssetInfo> data = new List<AssetInfo>();
            AssetInfo root = new AssetInfo().WithTreeData("Root", depth: -1);
            data.Add(root);

            switch (AssetInventory.Config.assetGrouping)
            {
                case 0: // none
                    _assets.OrderBy(a => a.GetDisplayName, StringComparer.OrdinalIgnoreCase)
                        .ToList()
                        .ForEach(a => data.Add(a.WithTreeData(a.GetDisplayName, a.AssetId)));
                    break;

                case 1: // category
                    _assets = _assets
                        .OrderBy(a => a.GetDisplayCategory, StringComparer.OrdinalIgnoreCase)
                        .ThenBy(a => a.GetDisplayName, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    string[] lastCats = Array.Empty<string>();
                    foreach (AssetInfo info in _assets)
                    {
                        // create hierarchy
                        string[] cats = string.IsNullOrEmpty(info.GetDisplayCategory) ? Array.Empty<string>() : info.GetDisplayCategory.Split('/');

                        // find first difference to previous cat
                        if (!ArrayUtility.ArrayEquals(cats, lastCats))
                        {
                            int firstDiff = 0;
                            bool diffFound = false;
                            for (int i = 0; i < Mathf.Min(cats.Length, lastCats.Length); i++)
                            {
                                if (cats[i] != lastCats[i])
                                {
                                    firstDiff = i;
                                    diffFound = true;
                                    break;
                                }
                            }
                            if (!diffFound) firstDiff = lastCats.Length;

                            for (int i = firstDiff; i < cats.Length; i++)
                            {
                                AssetInfo catItem = new AssetInfo().WithTreeData(cats[i], 0, i);
                                data.Add(catItem);
                            }
                        }

                        AssetInfo item = info.WithTreeData(info.GetDisplayName, info.AssetId, cats.Length);
                        data.Add(item);

                        lastCats = cats;
                    }
                    break;
            }

            // assign non-conflicting ids to all remaining elements to ensure expand/collapse works
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].TreeId == 0) data[i].WithTreeId(-i);
            }

            _assetTreeModel.SetData(data, true);
            AssetTreeView.Reload();
        }

        private void FetchAssetDetails()
        {
            AssetInventory.FetchAssetsDetails();
            ReloadLookups();
        }

        private void SetPage(int newPage)
        {
            newPage = Mathf.Clamp(newPage, 1, _pageCount);
            if (newPage != _curPage)
            {
                _curPage = newPage;
                if (_curPage > 0) PerformSearch(true);
            }
        }

        private async void UpdateStatistics()
        {
            _assets = AssetInventory.LoadAssets();
            _packageCount = _assets.Count;
            _indexedPackageCount = _assets.Count(a => a.FileCount > 0);
            _packageFileCount = DBAdapter.DB.Table<AssetFile>().Count();
            _dbSize = DBAdapter.GetDBSize();
            _cacheSize = await AssetInventory.GetCacheFolderSize();

            CreateAssetTree();
        }

        private void PerformSearch(bool keepPage = false)
        {
            int lastCount = _resultCount; // a bit of a heuristic but works great and is very performant
            string selectedSize = _resultSizes[AssetInventory.Config.maxResults];
            int.TryParse(selectedSize, out int maxResults);
            List<string> wheres = new List<string>();
            List<object> args = new List<object>();
            string escape = "";
            if (!string.IsNullOrWhiteSpace(_searchPhrase))
            {
                string phrase = _searchPhrase;

                // check for sqlite escaping requirements
                if (phrase.Contains("_"))
                {
                    phrase = phrase.Replace("_", "\\_");
                    escape = "ESCAPE '\\'";
                }

                wheres.Add($"AssetFile.Path like ? {escape}");
                args.Add($"%{phrase}%");
            }

            if (_selectedType > 0 && _types.Length > _selectedType)
            {
                string rawType = _types[_selectedType];
                string[] type = rawType.Split('/');
                if (type.Length > 1)
                {
                    wheres.Add("AssetFile.Type = ?");
                    args.Add(type.Last());
                }
                else if (AssetInventory.TypeGroups.ContainsKey(rawType))
                {
                    // sqlite does not support binding lists, parameters must be spelled out
                    List<string> paramCount = new List<string>();
                    foreach (string t in AssetInventory.TypeGroups[rawType])
                    {
                        paramCount.Add("?");
                        args.Add(t);
                    }

                    wheres.Add("AssetFile.Type in (" + string.Join(",", paramCount) + ")");
                }
            }

            // only add detail filters if section is open to not have confusing search results
            if (_showAdditionalFilters)
            {
                if (_selectedPublisher > 0 && _publisherNames.Length > _selectedPublisher)
                {
                    string publisher = _publisherNames[_selectedPublisher];
                    wheres.Add("Asset.SafePublisher = ?");
                    args.Add($"{publisher}");
                }

                if (_selectedAsset > 0 && _assetNames.Length > _selectedAsset)
                {
                    string asset = _assetNames[_selectedAsset];
                    wheres.Add("Asset.SafeName = ?"); // TODO: going via In would be more efficient but not available at this point
                    args.Add($"{asset}");
                }

                if (_selectedCategory > 0 && _categoryNames.Length > _selectedCategory)
                {
                    string category = _categoryNames[_selectedCategory];
                    wheres.Add("Asset.SafeCategory = ?");
                    args.Add($"{category}");
                }

                if (!string.IsNullOrWhiteSpace(_searchWidth))
                {
                    if (int.TryParse(_searchWidth, out int width) && width > 0)
                    {
                        string widthComp = _checkMaxWidth ? "<=" : ">=";
                        wheres.Add($"AssetFile.Width > 0 and AssetFile.Width {widthComp} ?");
                        args.Add(width);
                    }
                }

                if (!string.IsNullOrWhiteSpace(_searchHeight))
                {
                    if (int.TryParse(_searchHeight, out int height) && height > 0)
                    {
                        string heightComp = _checkMaxHeight ? "<=" : ">=";
                        wheres.Add($"AssetFile.Height > 0 and AssetFile.Height {heightComp} ?");
                        args.Add(height);
                    }
                }

                if (!string.IsNullOrWhiteSpace(_searchLength))
                {
                    if (int.TryParse(_searchLength, out int length) && length > 0)
                    {
                        string lengthComp = _checkMaxLength ? "<=" : ">=";
                        wheres.Add($"AssetFile.Length > 0 and AssetFile.Length {lengthComp} ?");
                        args.Add(length);
                    }
                }
            }

            // ordering, can only be done on DB side since post-processing results would only work on the paged results which is incorrect
            string orderBy = "order by ";
            switch (AssetInventory.Config.sortField)
            {
                case 1:
                    orderBy += "AssetFile.FileName";
                    break;

                case 2:
                    orderBy += "AssetFile.Size";
                    break;

                case 3:
                    orderBy += "AssetFile.Type";
                    break;

                case 4:
                    orderBy += "AssetFile.Length";
                    break;

                case 5:
                    orderBy += "AssetFile.Width";
                    break;

                case 6:
                    orderBy += "AssetFile.Height";
                    break;

                case 7:
                    orderBy += "Asset.DisplayCategory";
                    break;

                case 8:
                    orderBy += "Asset.LastRelease";
                    break;

                case 9:
                    orderBy += "Asset.AssetRating";
                    break;

                case 10:
                    orderBy += "Asset.RatingCount";
                    break;

                default:
                    orderBy += "AssetFile.Path";
                    break;
            }

            orderBy += " COLLATE NOCASE";
            if (AssetInventory.Config.sortDescending) orderBy += " desc";
            orderBy += ", AssetFile.Path"; // always sort by path in case of equality of first level sorting

            string where = wheres.Count > 0 ? "where " + string.Join(" and ", wheres) : "";
            string baseQuery = $"from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId {where}";
            string countQuery = $"select count(*) {baseQuery}";
            string dataQuery = $"select * {baseQuery} {orderBy}";
            if (maxResults > 0) dataQuery += $" limit {maxResults} offset {(_curPage - 1) * maxResults}";
            _resultCount = DBAdapter.DB.QueryScalars<int>($"{countQuery}", args.ToArray())[0];
            _files = DBAdapter.DB.Query<AssetInfo>($"{dataQuery}", args.ToArray());

            // preview images
            if (_textureLoading != null) EditorCoroutineUtility.StopCoroutine(_textureLoading);
            _textureLoading = EditorCoroutineUtility.StartCoroutine(LoadTextures(false), this); // TODO: should be true once pages endless scrolling is in

            // pagination
            _contents = _files.Select(file =>
            {
                string text = "";
                switch (AssetInventory.Config.tileText)
                {
                    case 0:
                        text = file.ShortPath;
                        break;

                    case 1:
                        text = file.FileName;
                        break;

                    case 2:
                        text = Path.GetFileNameWithoutExtension(file.FileName);
                        break;
                }

                text = text.Replace('/', Path.DirectorySeparatorChar);

                return new GUIContent {text = text};
            }).ToArray();
            _pageCount = AssetUtils.GetPageCount(_resultCount, maxResults);
            if (!keepPage && lastCount != _resultCount) _curPage = 1;
            SetPage(_curPage);
        }

        private IEnumerator LoadTextures(bool firstPageOnly)
        {
            string previewFolder = AssetInventory.GetPreviewFolder();
            int idx = -1;
            IEnumerable<AssetInfo> files = _files.Take(firstPageOnly ? 20 * 8 : _files.Count);
            foreach (AssetInfo file in files)
            {
                idx++;
                if (string.IsNullOrEmpty(file.PreviewFile)) continue;
                string previewFile = Path.Combine(previewFolder, file.PreviewFile);
                if (!File.Exists(previewFile)) continue;

                yield return AssetUtils.LoadTexture(previewFile, result =>
                {
                    if (_contents.Length > idx) _contents[idx].image = result;
                });
            }
        }

        private void OnAssetTreeSelectionChanged(IList<int> ids)
        {
            _selectedTreeAsset = null;
            if (ids.Count != 1) return;
            if (ids[0] <= 0) return;
            _selectedTreeAsset = _assetTreeModel.Find(ids[0]);
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}