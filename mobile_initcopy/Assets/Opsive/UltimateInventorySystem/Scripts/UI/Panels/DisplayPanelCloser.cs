/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.UI.Panels
{
    using System;
    using Opsive.Shared.Input;
    using Opsive.UltimateInventorySystem.Input;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    /// <summary>
    /// A component that allows to close panels when clicking the background image.
    /// </summary>
    public class DisplayPanelCloser : GraphicRaycasterTarget
    {
        [FormerlySerializedAs("m_CloseOnBackgroundClick")]
        [Tooltip("Close the panel when clicking the background image (Use an image that covers the entire screen).")]
        [SerializeField] protected bool m_CloseOnClick = true;
        [SerializeField] protected bool m_CloseOnInput = false;
        [SerializeField] protected SimpleInput m_CloseInput 
            = new SimpleInput("Close Panel",InputType.ButtonDown);
        [Tooltip("The display panel that needs to act as a pop up.")]
        [SerializeField] protected DisplayPanel m_DisplayPanel;

        protected PlayerInput m_PlayerInput;
        
        public bool CloseOnClick {
            get => m_CloseOnClick;
            set => m_CloseOnClick = value;
        }
        
        public DisplayPanel DisplayPanel
        {
            get => m_DisplayPanel;
            set
            {
                m_DisplayPanel = value;
                if (m_DisplayPanel != null) {
                    m_PlayerInput = m_DisplayPanel.Manager?.PanelOwner?.gameObject?.GetComponent<PlayerInput>();
                }
            }
        }

        /// <summary>
        /// Initialize the panel closer.
        /// </summary>
        protected override void Awake()
        {
            if (m_DisplayPanel == null) { DisplayPanel = GetComponent<DisplayPanel>(); }

            base.Awake();
        }

        /// <summary>
        /// Handle the on pointer click event and close the display panel.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (m_CloseOnClick) {
                m_DisplayPanel.Close();
            }

            base.OnPointerClick(eventData);
        }

        /// <summary>
        /// Check input to close
        /// </summary>
        protected virtual void Update()
        {
            if(m_CloseOnInput == false){ return;}
            if(m_DisplayPanel == null){ return; }

            if (m_PlayerInput == null) {
                m_PlayerInput = m_DisplayPanel.Manager?.PanelOwner?.gameObject?.GetComponent<PlayerInput>();
                if (m_PlayerInput == null) {
                    Debug.LogWarning("Player Input not found." ,gameObject);
                    return;
                }
            }

            if (m_CloseInput.CheckInput(m_PlayerInput)) {
                m_DisplayPanel.Close();
            }
            
        }
    }
}