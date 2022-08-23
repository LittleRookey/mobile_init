using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace AssetInventory
{
    internal class AssetTreeViewControl : TreeViewWithTreeModel<AssetInfo>
    {
        private const float kRowHeights = 20f;
        private const float toggleWidth = 20f;

        private enum Columns
        {
            Name,
            Indexed
        }

        public AssetTreeViewControl(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<AssetInfo> model) : base(state, multiColumnHeader, model)
        {
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = toggleWidth;

            Reload();
        }

        // only build the visible rows, the backend has the full tree information 
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            IList<TreeViewItem> rows = base.BuildRows(root);
            return rows;
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return false;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            TreeViewItem<AssetInfo> item = (TreeViewItem<AssetInfo>) args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (Columns) args.GetColumn(i), ref args);
            }
        }

        private void CellGUI(Rect cellRect, TreeViewItem<AssetInfo> item, Columns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case Columns.Indexed:
                    if (item.Data.IsIndexed)
                    {
                        Texture indexedIcon = EditorGUIUtility.IconContent("Installed", "Indexed").image;
                        Rect indexedRect = cellRect;
                        indexedRect.x += 20;
                        indexedRect.width = 16;
                        indexedRect.height = 16;
                        GUI.DrawTexture(indexedRect, indexedIcon);
                    }
                    break;

                case Columns.Name:
                    Rect toggleRect = cellRect;
                    toggleRect.x += GetContentIndent(item);
                    toggleRect.width = toggleWidth;
                    if (item.Data.Id > 0)
                    {
                        if (item.Data.PreviewTexture != null)
                        {
                            GUI.DrawTexture(toggleRect, item.Data.PreviewTexture);
                        }
                    }
                    else
                    {
                        Texture folderIcon = EditorGUIUtility.IconContent("Folder Icon", "Category").image;
                        GUI.DrawTexture(toggleRect, folderIcon);
                    }

                    // show default icon and label
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                    break;
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            MultiColumnHeaderState.Column[] columns =
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Center,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Indexed"),
                    contextMenuText = "Indexed",
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Right,
                    width = 60,
                    minWidth = 30,
                    maxWidth = 60,
                    autoResize = false,
                    allowToggleVisibility = true
                }
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Columns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            MultiColumnHeaderState state = new MultiColumnHeaderState(columns);
            return state;
        }
    }
}