using System;
using Overseer.Units;
using UnityEngine.UI;

namespace Overseer.Components {
    public class ActionSelection : MonoBehaviour {
        public Image image;
        public UnitAction action;
        public TMPro.TextMeshProUGUI text;

        public void Initialize() {
            image.sprite = action.Icon;
            text.text = action.KeyCodeText;
        }
    }
}