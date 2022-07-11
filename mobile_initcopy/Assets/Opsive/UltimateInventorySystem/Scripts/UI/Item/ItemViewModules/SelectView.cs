namespace Opsive.UltimateInventorySystem.UI.Item.ItemViewModules
{
    using Opsive.UltimateInventorySystem.Input;
    using Opsive.UltimateInventorySystem.UI.Menus.Crafting;
    using Opsive.UltimateInventorySystem.UI.Views;
    using UnityEngine;

    /// <summary>
    /// View UI component for changing an image when the box is selected.
    /// </summary>
    public class SelectView : ViewModule, IViewModuleSelectable
    {
        [Tooltip("Enable On Select")]
        [SerializeField] protected GameObject[] m_EnabledOnSelect;
        [Tooltip("Enable On Deselect")]
        [SerializeField] protected GameObject[] m_EnabledOnDeselect;
        [Tooltip("The image.")]
        [SerializeField] protected BoolUnityEvent m_OnSelect;

        /// <summary>
        /// Clear.
        /// </summary>
        public override void Clear()
        {
            var view = m_BaseView;
            if (view == null) {
                Select(false);
                return;
            }

            if (view.ViewSlot == null) {
                Select(false);
                return;
            }

            var viewSlotGameObject = view.ViewSlot.gameObject;
            var @select = EventSystemManager.GetEvenSystemFor(viewSlotGameObject).currentSelectedGameObject == viewSlotGameObject;
            Select(@select);
        }
        
        /// <summary>
        /// Select the box.
        /// </summary>
        /// <param name="select">select or unselect.</param>
        public void Select(bool select)
        {
            if (m_EnabledOnSelect != null) {
                for (int i = 0; i < m_EnabledOnSelect.Length; i++) {
                    if (m_EnabledOnSelect[i] != null) {
                        m_EnabledOnSelect[i].SetActive(select);
                    }
                }
            }

            if (m_EnabledOnDeselect != null) {
                for (int i = 0; i < m_EnabledOnDeselect.Length; i++) {
                    if (m_EnabledOnDeselect[i] != null) {
                        m_EnabledOnDeselect[i].SetActive(!select);
                    }
                }
            }
            
            m_OnSelect?.Invoke(select);
        }
    }
}