/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Views
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.Serialization;

    /// <summary>
    /// View UI component for changing an image when the view slot is being dragged.
    /// </summary>
    public class DragOffView : ViewModule, IViewModuleMovable
    {
        [Tooltip("Event called when the view is cleared")]
        [SerializeField] protected UnityEvent m_OnClear;
        
        [Header("On Drag Moving")]
        [Tooltip("Deactivate the gameobject on the moving view.")]
        [SerializeField] protected GameObject[] m_ActivateOnDragMoving;
        [Tooltip("Disable the components on the moving view.")]
        [SerializeField] protected Behaviour[] m_EnableOnDragMoving;
        [FormerlySerializedAs("m_DeactivateOnDrag")]
        [Tooltip("Deactivate the gameobject on the moving view.")]
        [SerializeField] protected GameObject[] m_DeactivateOnDragMoving;
        [FormerlySerializedAs("m_DisableOnDrag")]
        [Tooltip("Disable the components on the moving view.")]
        [SerializeField] protected Behaviour[] m_DisableOnDragMoving;
        [Tooltip("Event called when the view is Moving")]
        [SerializeField] protected UnityEvent m_OnDragMoving;
        
        [Header("On Drag Source")]
        [Tooltip("Deactivate the gameobjects on the Source view.")]
        [SerializeField] protected GameObject[] m_ActivateOnDragSource;
        [Tooltip("Disable the gameobjects on the Source view.")]
        [SerializeField] protected Behaviour[] m_EnableOnDragSource;
        [FormerlySerializedAs("m_DeactivateOnLeftOver")]
        [Tooltip("Deactivate the gameobjects on the Source view.")]
        [SerializeField] protected GameObject[] m_DeactivateOnDragSource;
        [FormerlySerializedAs("m_DisableOnLeftOver")]
        [Tooltip("Disable the gameobjects on the Source view.")]
        [SerializeField] protected Behaviour[] m_DisableOnDragSource;
        [Tooltip("Event called when the view is the source")]
        [SerializeField] protected UnityEvent m_OnDragSource;

        
        /// <summary>
        /// Enable or disable all the gamobjects in the array.
        /// </summary>
        /// <param name="gameObjects">The gameobjects array.</param>
        /// <param name="enable">Enable or Disable?</param>
        private void ActivateGameObjects(GameObject[] gameObjects, bool enable)
        {
            for (int i = 0; i < gameObjects.Length; i++) {
                if(gameObjects[i] == null){ continue; }
                gameObjects[i].SetActive(enable);
            }
        }
        
        /// <summary>
        /// Enable or disable all the gamobjects in the array.
        /// </summary>
        /// <param name="gameObjects">The gameobjects array.</param>
        /// <param name="enable">Enable or Disable?</param>
        private void EnableComponents(Behaviour[] gameObjects, bool enable)
        {
            for (int i = 0; i < gameObjects.Length; i++) {
                if(gameObjects[i] == null){ continue; }
                gameObjects[i].enabled = enable;
            }
        }
        

        /// <summary>
        /// Clear.
        /// </summary>
        public override void Clear()
        {
            m_OnClear?.Invoke();
            
            ActivateGameObjects(m_ActivateOnDragMoving, false);
            ActivateGameObjects(m_DeactivateOnDragMoving, true);
            EnableComponents(m_EnableOnDragMoving, false);
            EnableComponents(m_DisableOnDragMoving, true);
            
            ActivateGameObjects(m_ActivateOnDragSource, false);
            ActivateGameObjects(m_DeactivateOnDragSource, true);
            EnableComponents(m_EnableOnDragSource, false);
            EnableComponents(m_DisableOnDragSource, true);
        }

        /// <summary>
        /// Set the view as moving.
        /// </summary>
        public void SetAsMoving()
        {
            ActivateGameObjects(m_ActivateOnDragMoving, true);
            ActivateGameObjects(m_DeactivateOnDragMoving, false);
            EnableComponents(m_EnableOnDragMoving, true);
            EnableComponents(m_DisableOnDragMoving, false);
            
            m_OnDragMoving?.Invoke();
        }

        /// <summary>
        /// Set the view as the source of a movement.
        /// </summary>
        /// <param name="movingSource">started moving or stopped?</param>
        public void SetAsMovingSource(bool movingSource)
        {
            ActivateGameObjects(m_ActivateOnDragSource, true);
            ActivateGameObjects(m_DeactivateOnDragSource, false);
            EnableComponents(m_EnableOnDragSource, true);
            EnableComponents(m_DisableOnDragSource, false);
            
            m_OnDragSource?.Invoke();
        }
    }
}