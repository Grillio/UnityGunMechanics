using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EGHiddenNamespace
{
    public class BulletTemplate : VisualElement
    {
        public BulletTemplate()
        {
            VisualTreeAsset visual = Resources.Load<VisualTreeAsset>("UXML/BulletTemplate");
            visual.CloneTree(this);

            StyleSheet style = Resources.Load<StyleSheet>("StyleSheets/BulletStyle");
            styleSheets.Add(style);
        }
    }
}
