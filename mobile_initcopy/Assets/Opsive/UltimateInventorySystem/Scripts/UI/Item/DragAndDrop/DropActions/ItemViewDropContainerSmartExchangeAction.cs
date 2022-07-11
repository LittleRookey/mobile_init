﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Item.DragAndDrop.DropActions
{
    using Opsive.UltimateInventorySystem.UI.Grid;
    using Opsive.UltimateInventorySystem.UI.Panels.Hotbar;
    using System;
    using UnityEngine;

    /// <summary>
    /// The Item View Drop Container Can Smart Exchange Condition.
    /// </summary>
    [Serializable]
    public class ItemViewDropContainerCanSmartExchangeCondition : ItemViewDropCondition
    {
        [Tooltip("Log some useful information to debug smart exchanges.")]
        [SerializeField] protected bool m_Debug = false;

        /// <summary>
        /// Can the item be dropped.
        /// </summary>
        /// <param name="itemViewDropHandler">The Item View Drop Handler.</param>
        /// <returns>True if it can be dropped.</returns>
        public override bool CanDrop(ItemViewDropHandler itemViewDropHandler)
        {
            // If they are equal items should be moved not exchanged.
            if (itemViewDropHandler.SourceContainer == itemViewDropHandler.DestinationContainer) { return false; }

            //Item Shapes work a bit differently so this needs to return false in case we are using an item shape grid.
            if (itemViewDropHandler.DestinationContainer is ItemShapeGrid) { return false; }

            var sourceCanGive = itemViewDropHandler.SourceContainer?.CanGiveItem(
                itemViewDropHandler.SourceItemInfo,
                itemViewDropHandler.SourceIndex) ?? false;

            var destinationCanGive = itemViewDropHandler.DestinationContainer?.CanGiveItem(
                itemViewDropHandler.DestinationItemInfo,
                itemViewDropHandler.DestinationIndex) ?? false;

            var sourceCanAdd = itemViewDropHandler.SourceContainer?.CanAddItem(
                itemViewDropHandler.StreamData.DestinationItemInfo,
                itemViewDropHandler.SourceIndex) ?? false;

            var destinationCanAdd = itemViewDropHandler.DestinationContainer?.CanAddItem(
                itemViewDropHandler.StreamData.SourceItemInfo,
                itemViewDropHandler.DestinationIndex) ?? false;
            
            var sourceIsNull = itemViewDropHandler.SourceItemInfo.Item == null;
            var destinationIsNull = itemViewDropHandler.DestinationItemInfo.Item == null;
            
            // Can contain looks only at the slot restrictions, not the itemRestrictions of the item collection or inventory.
            var sourceSlotCanContain = destinationIsNull || (itemViewDropHandler.SourceItemViewSlot?.CanContain(itemViewDropHandler.DestinationItemInfo) ?? false);
            var destinationSlotCanContain = sourceIsNull || (itemViewDropHandler.StreamData.DestinationItemViewSlot?.CanContain(itemViewDropHandler.SourceItemInfo) ?? false);

            if (m_Debug)
            {
                Debug.Log($"sourceCanGive {sourceCanGive}\n" +
                          $"destinationCanGive {destinationCanGive}\n" +
                          $"sourceCanAdd {sourceCanAdd}\n" +
                          $"destinationCanAdd {destinationCanAdd}\n" +
                          $"sourceIsNull {sourceIsNull}\n" +
                          $"destinationIsNull {destinationIsNull}\n" +
                          $"sourceSlotCanContain {sourceSlotCanContain}\n" +
                          $"destinationSlotCanContain {destinationSlotCanContain}\n");
            }

            if (sourceIsNull && destinationIsNull) { return false; }

            return (sourceSlotCanContain && destinationSlotCanContain && sourceCanGive && destinationCanAdd);
        }
    }

    /// <summary>
    /// The Item View Drop Container Exchange Action.
    /// </summary>
    [Serializable]
    public class ItemViewDropContainerSmartExchangeAction : ItemViewDropAction
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemViewDropContainerSmartExchangeAction()
        {
        }

        /// <summary>
        /// Drop Action.
        /// </summary>
        /// <param name="itemViewDropHandler">The Item View Drop Handler.</param>
        public override void Drop(ItemViewDropHandler itemViewDropHandler)
        {

            var sourceCanGive = itemViewDropHandler.SourceContainer.CanGiveItem(
                itemViewDropHandler.SourceItemInfo,
                itemViewDropHandler.SourceIndex);

            var destinationCanGive = itemViewDropHandler.DestinationContainer.CanGiveItem(
                itemViewDropHandler.DestinationItemInfo,
                itemViewDropHandler.DestinationIndex); ;

            var sourceCanAdd = itemViewDropHandler.SourceContainer.CanAddItem(
                itemViewDropHandler.StreamData.DestinationItemInfo,
                itemViewDropHandler.SourceIndex);

            var destinationCanAdd = itemViewDropHandler.DestinationContainer.CanAddItem(
                itemViewDropHandler.StreamData.SourceItemInfo,
                itemViewDropHandler.DestinationIndex);

            var sourceIsNull = itemViewDropHandler.SourceItemInfo.Item == null;
            var destinationIsNull = itemViewDropHandler.DestinationItemInfo.Item == null;
            
            // Can contain looks only at the slot restrictions, not the itemRestrictions of the item collection or inventory.
            var sourceSlotCanContain = destinationIsNull || itemViewDropHandler.SourceItemViewSlot.CanContain(itemViewDropHandler.DestinationItemInfo);
            var destinationSlotCanContain = sourceIsNull || itemViewDropHandler.StreamData.DestinationItemViewSlot.CanContain(itemViewDropHandler.SourceItemInfo);

            if (!sourceSlotCanContain || !destinationSlotCanContain) return;

            var sourceGiveDestinationReceive = sourceIsNull == false && (sourceCanGive && destinationCanAdd);
            var destinationGiveSourceReceive = destinationIsNull == false && (destinationCanGive && sourceCanAdd);

            if (itemViewDropHandler.SourceContainer.Inventory == itemViewDropHandler.DestinationContainer.Inventory) {
                //Make an exception for Inventory Mirror Hotbar as they are a direct copy of the Inventory
                if (itemViewDropHandler.DestinationContainer is InventoryMirrorHotbar destinationMirrorHotbar &&
                    itemViewDropHandler.SourceContainer is InventoryGrid sourceInventoryGrid) {
            
                    sourceInventoryGrid.MoveItem(itemViewDropHandler.SourceIndex, destinationMirrorHotbar.SlotIndexToGridIndex(itemViewDropHandler.DestinationIndex));
                    return;
                }
                if (itemViewDropHandler.SourceContainer is InventoryMirrorHotbar sourceMirrorHotbar &&
                    itemViewDropHandler.DestinationContainer is InventoryGrid destinationInventoryGrid) {
            
                    destinationInventoryGrid.MoveItem(sourceMirrorHotbar.SlotIndexToGridIndex(itemViewDropHandler.SourceIndex), itemViewDropHandler.DestinationIndex);
                    return;
                }
            }

            if (sourceGiveDestinationReceive) {
                // Make an exception for Item Hotbars as they look for items within the Inventory.
                if (!(itemViewDropHandler.DestinationContainer is ItemHotbar) || itemViewDropHandler.DestinationContainer is InventoryMirrorHotbar) {
                    itemViewDropHandler.StreamData.SourceItemInfo = itemViewDropHandler.SourceContainer.RemoveItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.SourceIndex);
                }
            }

            if (destinationGiveSourceReceive) {
                itemViewDropHandler.StreamData.DestinationItemInfo = itemViewDropHandler.DestinationContainer.RemoveItem(itemViewDropHandler.StreamData.DestinationItemInfo, itemViewDropHandler.DestinationIndex);
            }

            if (sourceGiveDestinationReceive) {
                var addToDestination = false;
                
                // Make an exception for Item Hotbars as they look for items within the Inventory.
                if (!(itemViewDropHandler.SourceContainer is ItemHotbar itemHotbar) || itemViewDropHandler.SourceContainer is InventoryMirrorHotbar ) {
                    addToDestination = true;
                } else {
                    if (itemHotbar.Inventory != itemViewDropHandler.DestinationContainer.Inventory) {
                        addToDestination = true;
                    }
                }

                if (addToDestination) {
                    itemViewDropHandler.DestinationContainer.AddItem(itemViewDropHandler.StreamData.SourceItemInfo, itemViewDropHandler.DestinationIndex);
                }
            }

            if (destinationGiveSourceReceive) {
                itemViewDropHandler.SourceContainer.AddItem(itemViewDropHandler.StreamData.DestinationItemInfo, itemViewDropHandler.SourceIndex);
            }
        }
    }
}