using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JD.EditorAudioUtils;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace AssetInventory
{
    public class IndexUI : BasicEditorUI
    {
        private bool DEBUG_MODE = false;

        private List<AssetInfo> _files;
        private GUIContent[] _contents;
        private string[] _assetNames;
        private string[] _publisherNames;
        private string[] _categoryNames;
        private string[] _types;
        private string[] _resultSizes;

        private int _gridSelection;
        private string _searchPhrase;
        private int _selectedPublisher;
        private int _selectedCategory;
        private int _selectedType;
        private int _selectedAsset;
        private bool _autoPlayAudio = true;
        private int _tab;
        private int _lastMainProgress;
        private Vector2 _searchScrollPos;
        private Vector2 _folderScrollPos;
        private Vector2 _assetScrollPos;

        private int _curPage = 1;
        private int _pageCount;

        private bool _previewInProgress;
        private EditorCoroutine _textureLoading;

        private string _token;

        private int _selectedFolderIndex;
        private AssetInfo _selectedEntry;
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

        [MenuItem("Assets/Asset Inventory", priority = 9000)]
        public static void ShowWindow()
        {
            IndexUI window = GetWindow<IndexUI>("Asset Inventory");
            window.minSize = new Vector2(550, 200);
        }

        private void Awake()
        {
            AssetInventory.Init();
            ReloadLookups();
            PerformSearch();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnplayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnplayModeStateChanged;
        }

        private void OnplayModeStateChanged(PlayModeStateChange state)
        {
            // UI will have lost all preview textures during play mode
            if (state == PlayModeStateChange.EnteredEditMode) PerformSearch();
        }

        private void ReloadLookups()
        {
            _resultSizes = new[] {"-all-", string.Empty, "10", "25", "50", "100", "250", "500", "1000"};

            _assets = AssetInventory.LoadAssets();
            _assetNames = AssetInventory.ExtractAssetNames(_assets);
            _publisherNames = AssetInventory.ExtractPublisherNames(_assets);
            _categoryNames = AssetInventory.ExtractCategoryNames(_assets);
            _types = AssetInventory.LoadTypes();
            _token = CloudProjectSettings.accessToken;
            _purchasedAssetsCount = AssetInventory.CountPurchasedAssets(_assets);

            UpdateStatistics();
        }

        public override void OnGUI()
        {
            // independent of actual window
            base.OnGUI();

            if (EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("The asset inventory is not available during play mode.", MessageType.Info);
                return;
            }

            bool newTab = false;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            List<string> strings = new List<string>
            {
                "Search",
                "Index" + (AssetInventory.IndexingInProgress ? " (in progress)" : ""),
                "Assets" + (AssetInventory.CurrentMain != null ? " (in progress)" : ""),
                "About"
            };
            if (DEBUG_MODE) strings.Add("Compliance");
            _tab = GUILayout.Toolbar(_tab, strings.ToArray(), GUILayout.Height(32), GUILayout.MinWidth(500));
            if (EditorGUI.EndChangeCheck()) newTab = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            switch (_tab)
            {
                case 0:
                    // search
                    if (_contents.Length == 0) UpdateStatistics();
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
                        EditorGUI.BeginChangeCheck();
                        _searchPhrase = EditorGUILayout.TextField("Search:", _searchPhrase, GUILayout.ExpandWidth(true));

                        GUILayout.BeginHorizontal();
                        _selectedType = EditorGUILayout.Popup("Type:", _selectedType, _types, GUILayout.ExpandWidth(false), GUILayout.MinWidth(150));
                        _selectedAsset = EditorGUILayout.Popup("Asset:", _selectedAsset, _assetNames, GUILayout.ExpandWidth(true), GUILayout.MinWidth(200));
                        _selectedPublisher = EditorGUILayout.Popup("Publisher:", _selectedPublisher, _publisherNames, GUILayout.ExpandWidth(false), GUILayout.MinWidth(150));
                        _selectedCategory = EditorGUILayout.Popup("Category:", _selectedCategory, _categoryNames, GUILayout.ExpandWidth(false), GUILayout.MinWidth(150));
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck()) dirty = true;

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

                            int cells = Mathf.RoundToInt(Mathf.Clamp(Mathf.Floor((position.width - UIStyles.INSPECTOR_WIDTH - UIStyles.BORDER_WIDTH) / UIStyles.TILE_WIDTH), 1, 99));
                            EditorGUI.BeginChangeCheck();
                            _gridSelection = GUILayout.SelectionGrid(_gridSelection, _contents, cells, UIStyles.tile);
                            bool isAudio = AssetInventory.IsFileType(_selectedEntry?.Path, "Audio");
                            if (EditorGUI.EndChangeCheck())
                            {
                                _selectedEntry = _files[_gridSelection];
                                isAudio = AssetInventory.IsFileType(_selectedEntry?.Path, "Audio");
                                if (_autoPlayAudio && isAudio) PlayAudio(_selectedEntry);
                                if (_selectedEntry != null)
                                {
                                    _selectedEntry.ProjectPath = AssetDatabase.GUIDToAssetPath(_selectedEntry.Guid);
                                    EditorCoroutineUtility.StartCoroutine(AssetUtils.LoadTexture(_selectedEntry), this);

                                    // if entry is already materialized calculate dependencies immediately
                                    if (!_previewInProgress && _selectedEntry.DepState == AssetInfo.DependencyState.Unknown && AssetInventory.IsMaterialized(_selectedEntry.ToAsset(), _selectedEntry)) CalculateDependencies(_selectedEntry);

                                    _packageAvailable = File.Exists(_selectedEntry.Location);
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
                                if (GUILayout.Button("<")) SetPage(_curPage - 1);
                                EditorGUIUtility.labelWidth = 0;
                                EditorGUILayout.LabelField($"Page {_curPage}/{_pageCount}", UIStyles.centerLabel, GUILayout.ExpandWidth(true));
                                EditorGUIUtility.labelWidth = 60;
                                if (GUILayout.Button(">")) SetPage(_curPage + 1);
                            }
                            else
                            {
                                EditorGUILayout.LabelField($"{_resultCount} results", UIStyles.centerLabel, GUILayout.ExpandWidth(true));
                            }
                            GUILayout.FlexibleSpace();
                            EditorGUI.BeginChangeCheck();
                            AssetInventory.Config.maxResults = EditorGUILayout.Popup("Results:", AssetInventory.Config.maxResults, _resultSizes, GUILayout.ExpandWidth(false), GUILayout.Width(130));
                            if (EditorGUI.EndChangeCheck())
                            {
                                dirty = true;
                                AssetInventory.SaveConfig();
                            }
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

                            GUILayout.BeginVertical("Details Inspector", "window", GUILayout.Width(UIStyles.INSPECTOR_WIDTH));
                            EditorGUILayout.Space();
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
                                    if (AssetInventory.SCAN_DEPENDENCIES.Contains(_selectedEntry.Type))
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

                                    EditorGUI.BeginDisabledGroup(_previewInProgress);
                                    if (!_selectedEntry.InProject && !string.IsNullOrEmpty(selectedPath))
                                    {
                                        GUILabelWithText("Import To", selectedPath);
                                        EditorGUILayout.Space();
                                        if (GUILayout.Button("Import File" + (_selectedEntry.DependencySize > 0 ? " Only" : ""))) CopyTo(_selectedEntry, selectedPath);
                                        if (_selectedEntry.DependencySize > 0 && AssetInventory.SCAN_DEPENDENCIES.Contains(_selectedEntry.Type))
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
                                    if (isAudio)
                                    {
                                        EditorGUILayout.Space();
                                        _autoPlayAudio = GUILayout.Toggle(_autoPlayAudio, "Automatically play selected audio");
                                    }
                                    if (!_selectedEntry.InProject && selectedPath == null)
                                    {
                                        EditorGUILayout.LabelField("Select a folder in Project view for import options", EditorStyles.centeredGreyMiniLabel);
                                    }
                                }
                                if (_previewInProgress)
                                {
                                    EditorGUILayout.LabelField("Extracting...", EditorStyles.centeredGreyMiniLabel);
                                }

                                EditorGUILayout.Space(30);
                                EditorGUILayout.LabelField("Asset", EditorStyles.largeLabel);
                                GUILabelWithText("Name", Path.GetFileName(_selectedEntry.GetDisplayName));
                                GUILabelWithText("Size", EditorUtility.FormatBytes(_selectedEntry.PackageSize));
                                if (!string.IsNullOrWhiteSpace(_selectedEntry.GetDisplayPublisher)) GUILabelWithText("Publisher", $"{_selectedEntry.GetDisplayPublisher}");
                                if (!string.IsNullOrWhiteSpace(_selectedEntry.GetDisplayCategory)) GUILabelWithText("Category", $"{_selectedEntry.GetDisplayCategory}");
                                GUILabelWithText("Source", _selectedEntry.AssetSource == Asset.Source.AssetStorePackage ? "Asset Store" : "File System");
                                if (_selectedEntry.AssetSource == Asset.Source.AssetStorePackage || _selectedEntry.AssetSource == Asset.Source.CustomPackage)
                                {
                                    EditorGUILayout.Space();
                                    if (_packageAvailable)
                                    {
                                        if (GUILayout.Button("Import Full Package")) AssetDatabase.ImportPackage(_selectedEntry.Location, true);
                                    }
                                }
                                EditorGUI.EndDisabledGroup();
                                if (_selectedEntry.PreviewTexture != null)
                                {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Box(_selectedEntry.PreviewTexture, EditorStyles.centeredGreyMiniLabel, GUILayout.MaxWidth(UIStyles.INSPECTOR_WIDTH), GUILayout.MaxHeight(UIStyles.TILE_HEIGHT));
                                    GUILayout.FlexibleSpace();
                                }
                            }
                            GUILayout.EndVertical();
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.Label("No Results", UIStyles.whiteCenter, GUILayout.MinHeight(UIStyles.TILE_HEIGHT));
                        }

                        if (dirty) PerformSearch();
                        EditorGUIUtility.labelWidth = 0;
                    }
                    break;

                case 1:
                    if (newTab) UpdateStatistics();

                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();

                    // folders
                    EditorGUILayout.LabelField("Unity Asset Store downloads will be indexed automatically. Specify custom locations below to scan for unitypackages downloaded from somewhere else than the Asset Store.", EditorStyles.wordWrappedLabel);
                    EditorGUILayout.Space();
                    EditorGUI.BeginChangeCheck();
                    AssetInventory.Config.indexAssetStore = GUILayout.Toggle(AssetInventory.Config.indexAssetStore, "Index Asset Store Downloads");
                    if (EditorGUI.EndChangeCheck()) AssetInventory.SaveConfig();
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

                    EditorGUILayout.Space(30);
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
                        if (GUILayout.Button("Clear Cache")) AssetInventory.ClearCache(UpdateStatistics);
                        if (GUILayout.Button("Reset Configuration")) AssetInventory.ResetConfig();
                        if (DBAdapter.IsDBOpen())
                        {
                            if (GUILayout.Button("Close Database (to allow copying)")) DBAdapter.Close();
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    break;

                case 2:
                    if (string.IsNullOrEmpty(_token))
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("Could not retrieve Asset Store purchases. Open the package manager once and go to the My Assets section. Then reopen this window.", MessageType.Warning);
                        GUILayout.BeginVertical();
                        if (GUILayout.Button("Open Package Manager")) UnityEditor.PackageManager.UI.Window.Open("");
                        if (GUILayout.Button("Retry")) _token = CloudProjectSettings.accessToken;
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        GUILayout.BeginHorizontal("Asset Store", "window", GUILayout.Height(55));
                        EditorGUILayout.LabelField($"Purchased Assets: {_purchasedAssetsCount}", GUILayout.ExpandWidth(false));

                        EditorGUI.BeginDisabledGroup(AssetInventory.CurrentMain != null);
                        if (GUILayout.Button("Retrieve & Update Purchases")) FetchAssetPurchases();
                        if (DEBUG_MODE && GUILayout.Button("Update Asset Details")) FetchAssetDetails();
                        EditorGUI.EndDisabledGroup();
                        if (AssetInventory.CurrentMain != null)
                        {
                            EditorGUILayout.LabelField($"{AssetInventory.CurrentMain} {AssetInventory.MainProgress}/{AssetInventory.MainCount}");
                        }
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Open Package Manager")) UnityEditor.PackageManager.UI.Window.Open("");
                        GUILayout.EndHorizontal();
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField($"Your Assets ({_assets.Count})", EditorStyles.largeLabel);

                    EditorGUILayout.Space();
                    if (_assets.Count > 0)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Indexed", EditorStyles.boldLabel, GUILayout.Width(50));
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Asset", EditorStyles.boldLabel);
                        GUILayout.EndHorizontal();

                        _assetScrollPos = GUILayout.BeginScrollView(_assetScrollPos, false, false);
                        bool allIndexed = true;
                        for (int i = 0; i < _assets.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(25);
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.Toggle(_assets[i].FileCount > 0 && _assets[i].CurrentState == Asset.State.Done, GUILayout.Width(20));
                            if (_assets[i].FileCount == 0) allIndexed = false;
                            EditorGUI.EndDisabledGroup();
                            GUILayout.Space(25);
                            EditorGUILayout.LabelField(_assets[i].GetDisplayName);
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                        if (!allIndexed) EditorGUILayout.HelpBox("To index the remaining assets, download them in the Package Manager.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No assets were indexed yet. Start the indexing process to fill this list.", MessageType.Info);
                    }
                    if (DEBUG_MODE && GUILayout.Button("Get Token")) Debug.Log(CloudProjectSettings.accessToken);
                    break;

                case 3:
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
                    break;

                case 4:
                    if (GUILayout.Button("Calculate Asset Usage")) CalculateAssetUsage();

                    int assetUsageCount = _assetUsage?.Count ?? 0;
                    int identifiedAssetsCount = _identifiedAssets?.Count ?? 0;

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
                        foreach (string assetName in _usedAssets)
                        {
                            EditorGUILayout.LabelField(assetName);
                        }
                    }
                    break;
            }

            // reload if there is new data
            if (_lastMainProgress != AssertImporter.MainProgress)
            {
                _lastMainProgress = AssertImporter.MainProgress;
                ReloadLookups();
                PerformSearch();
            }
        }

        private void CalculateAssetUsage()
        {
            _assetUsage = AssetInventory.CalculateAssetUsage();
            _usedAssets = _assetUsage.Select(ai => ai.DisplayName).Distinct().Where(a => !string.IsNullOrEmpty(a)).ToList();
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

        private void OnAddButtonClicked()
        {
            string folder = EditorUtility.OpenFolderPanel("Select folder to index", "", "");
            if (string.IsNullOrEmpty(folder)) return;

            FolderSpec spec = new FolderSpec();
            spec.Location = folder;
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
            string targetPath = await AssetInventory.EnsureMaterializedAsset(assetInfo.ToAsset(), assetInfo);

            EditorAudioUtility.StopAllPreviewClips();
            if (targetPath != null)
            {
                AudioClip clip = await AssetUtils.LoadAudioFromFile(targetPath);
                if (clip != null) EditorAudioUtility.PlayPreviewClip(clip);
            }
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
                targetPath = await AssetInventory.EnsureMaterializedAsset(assetInfo.ToAsset(), assetInfo);
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
                targetPath = await AssetInventory.EnsureMaterializedAsset(assetInfo.ToAsset(), assetInfo);
            }
            if (targetPath != null) EditorUtility.RevealInFinder(targetPath);
            _previewInProgress = false;
        }

        private async void CopyTo(AssetInfo assetInfo, string selectedPath, bool withDependencies = false, bool withScripts = false)
        {
            _previewInProgress = true;
            string sourcePath = await AssetInventory.EnsureMaterializedAsset(assetInfo.ToAsset(), assetInfo);
            if (sourcePath != null)
            {
                string targetPath = Path.Combine(selectedPath, Path.GetFileName(sourcePath));
                DoCopyTo(sourcePath, targetPath);

                if (withDependencies)
                {
                    List<AssetFile> deps = withScripts ? assetInfo.Dependencies : assetInfo.MediaDependencies;
                    for (int i = 0; i < deps.Count; i++)
                    {
                        // check if already in target
                        if (!string.IsNullOrEmpty(deps[i].Guid))
                        {
                            if (!string.IsNullOrWhiteSpace(AssetDatabase.GUIDToAssetPath(deps[i].Guid))) continue;
                        }

                        sourcePath = await AssetInventory.EnsureMaterializedAsset(assetInfo.ToAsset(), deps[i]);
                        targetPath = Path.Combine(selectedPath, Path.GetFileName(deps[i].Path));
                        DoCopyTo(sourcePath, targetPath);
                    }
                }

                AssetDatabase.Refresh();
                assetInfo.ProjectPath = AssetDatabase.GUIDToAssetPath(assetInfo.Guid);
            }
            _previewInProgress = false;
        }

        private void DoCopyTo(string sourcePath, string targetPath)
        {
            File.Copy(sourcePath, targetPath, true);

            string sourceMetaPath = sourcePath + ".meta";
            string targetMetaPath = targetPath + ".meta";
            if (File.Exists(sourceMetaPath)) File.Copy(sourceMetaPath, targetMetaPath, true);
        }

        private async void FetchAssetPurchases()
        {
            _purchasedAssets = await AssetInventory.FetchOnlineAssets();
            _purchasedAssetsCount = _purchasedAssets?.total ?? 0;
            ReloadLookups();
            FetchAssetDetails();
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
                if (_curPage > 0) PerformSearch();
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
        }

        private void PerformSearch()
        {
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

                wheres.Add("AssetFile.Path like ?");
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

            string where = wheres.Count > 0 ? "where " + string.Join(" and ", wheres) : "";
            string baseQuery = $"from AssetFile inner join Asset on Asset.Id = AssetFile.AssetId {where} {escape}";
            string countQuery = $"select count(*) {baseQuery}";
            string dataQuery = $"select * {baseQuery}";
            if (maxResults > 0) dataQuery += $" limit {maxResults} offset {(_curPage - 1) * maxResults}";
            _resultCount = DBAdapter.DB.QueryScalars<int>($"{countQuery}", args.ToArray())[0];
            _files = DBAdapter.DB.Query<AssetInfo>($"{dataQuery}", args.ToArray());

            // preview images
            if (_textureLoading != null) EditorCoroutineUtility.StopCoroutine(_textureLoading);
            _textureLoading = EditorCoroutineUtility.StartCoroutine(LoadTextures(false), this); // TODO: should be true once pages endless scrolling is in

            // pagination
            _contents = _files.Select(file => new GUIContent {text = file.ShortPath}).ToArray();
            _pageCount = AssetUtils.GetPageCount(_resultCount, maxResults);
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

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}