/*using System;
using Overseer.Components;
using Overseer.Units;

namespace Overseer.States {
    public class Reconfigure : BaseState {
        public GameObject prefab => Main.assets.LoadAsset<GameObject>("ReconfigureMenu.prefab");
        public OverseerController controller;
        public GameObject instance;
        private bool hasDone = false;

        public override void OnEnter()
        {
            base.OnEnter();
            controller = GetComponent<OverseerController>();
            instance = GameObject.Instantiate(prefab);
            instance.GetComponent<ActionSelector>().onActionFired.AddListener(OnFired);
        }

        public override void FixedUpdate() {
            if (!hasDone) {
                try { // we dont know when ActionSelector Start() will have run, so just catch the nullref and do nothing
                    Unit offense = (controller.offenseFamily.skillDef as UnitSkillDef).UnitType;
                    Unit defense = (controller.defenseFamily.skillDef as UnitSkillDef).UnitType;
                    Unit support = (controller.supportFamily.skillDef as UnitSkillDef).UnitType;
                    instance.GetComponent<ActionSelector>().UpdateBasedOnUnitChoices(offense, defense, support);
                    hasDone = true;
                }
                catch
                {

                }
            }
        }

        public void OnFired(ActionType type) {
            Destroy(instance);
            controller.ReconfigureUnits(type);
            outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}*/