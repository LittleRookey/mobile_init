using System;
using UnityEngine.UIElements;

namespace InfiniteVoid.SpamFramework.Editor.Common.Extensions
{
    public static class VisualElementExtensions
    {
        public static void Toggle(this VisualElement ve) =>
            ve.style.display = ve.style.display == DisplayStyle.None
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        
        public static void VisibleIf(this VisualElement ve, Func<bool> predicate) => 
            ve.style.display = predicate.Invoke()  
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        
        public static void SetVisible(this VisualElement ve, bool visible) => 
            ve.style.display = visible  
                ? DisplayStyle.Flex
                : DisplayStyle.None;

 
    }
}