using System;
using UnityEngine.UI;

namespace Overseer.Components {
    public class UnitUIBar : MonoBehaviour {
        public Image RadialChargeMeter;
        public Image UnitIcon;
        public TMPro.TextMeshProUGUI UnitCountText;
        internal OverseerController controller;

        public void FixedUpdate() {
            if (controller) {
                RadialChargeMeter.fillAmount = controller.GetAssemblyPercentage();
                UnitIcon.sprite = controller.GetUnitIcon();
                UnitCountText.text = controller.GetUnitCountText();
            }
        }
    }
}