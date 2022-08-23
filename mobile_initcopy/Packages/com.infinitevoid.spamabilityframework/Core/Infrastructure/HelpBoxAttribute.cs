using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure
{
    public enum HelpBoxMessageType { None, Info, Warning, Error }
     
    /// <summary>
    /// Used to display information in an editor help-box about a given property
    /// </summary>
    public class HelpBoxAttribute : PropertyAttribute {
     
        public string text;
        public HelpBoxMessageType messageType;
     
        public HelpBoxAttribute(string text, HelpBoxMessageType messageType = HelpBoxMessageType.None) {
            this.text = text;
            this.messageType = messageType;
        }
    }
}