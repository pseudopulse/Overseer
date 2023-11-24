using System;
using Overseer.Units;

namespace Overseer.Components {
    public class OverseerController : MonoBehaviour {
        public CharacterBody body;
        public int unitCap;
        public GenericSkill offenseFamily;
        public GenericSkill defenseFamily;
        public GenericSkill supportFamily;
        public GenericSkill passive;
        public Unit NullUnit;
        public GameObject OverseerUIPrefab;
        public SkillDef passiveInfernal;
        public SkillDef passiveSwarmer;

        private Unit currentUnitType = null;
        private float unitStopwatch = 0f;
        internal List<IUnitC> activeUnits = new();
        private GameObject UIPrefabInstance;
        private bool hasRecalled = false;
        internal Vector3 AimPoint;
        private bool isPassiveSwarmer = false;
        internal bool isPassiveInfernal = false;
        internal float UnitEffectModifier = 1f;
        internal float UnitAssemblyModifier = 1f;
        private GameObject MainHUDInstance;
        private List<Unit> UnitTypes = new();
        private int index = 0;

        public void Start() {
            currentUnitType = NullUnit;

            UIPrefabInstance = GameObject.Instantiate(OverseerUIPrefab);
            UIPrefabInstance.GetComponent<UnitUIBar>().controller = this;

            if (passive.skillDef == passiveSwarmer) {
                isPassiveSwarmer = true;
                unitCap *= 2;
                UnitAssemblyModifier = 0.5f;
                UnitEffectModifier = 0.5f;
            }

            if (passive.skillDef == passiveInfernal) {
                isPassiveInfernal = true;
            }
        }

        public void TriggerAltFire() {
            foreach (IUnitC unit in activeUnits) {
                unit.PerformSecondaryAction();
            }
        }

        public float GetAssemblyPercentage() {
            if (currentUnitType == NullUnit) {
                return 0f;
            };

            if (activeUnits.Count >= unitCap) {
                return 1f;
            }

            return unitStopwatch / (currentUnitType.AssemblyTime * UnitAssemblyModifier);
        }

        public Sprite GetUnitIcon() {
            return currentUnitType.Icon;
        }

        public string GetUnitCountText() {
            return activeUnits.Count.ToString() + " / " + unitCap;
        }

        public void SpawnUnit() {
            if (currentUnitType == NullUnit) {
                return;
            }
            if (activeUnits.Count > unitCap) {
                return;
            }

            GameObject unit = UnitCatalog.SpawnUnit(this, currentUnitType);
            IUnitC iunit = unit.GetComponent<IUnitC>();
            if (hasRecalled) {
                iunit.Recall();
            }

            CharacterBody ubody = unit.GetComponent<CharacterBody>();
            ubody.level = body.level; 
            ubody.baseDamage = body.baseDamage;
            ubody.levelDamage = body.levelDamage;
            activeUnits.Add(iunit);
        }

        public void HandleAction(ActionType type) {
            switch (type) {
                case ActionType.Recall:
                    foreach (IUnitC unit in activeUnits) {
                        unit.Recall();
                    }
                    hasRecalled = !hasRecalled;
                    break;
                case ActionType.Retarget:
                    break;
                case ActionType.Terminate:
                    List<IUnitC> units = new();
                    for (int i = 0; i < activeUnits.Count; i++) {
                        IUnitC unit = activeUnits[i];
                        units.Add(unit);
                    }

                    for (int i = 0; i < units.Count; i++) {
                        activeUnits.Remove(units[i]);
                        units[i].Destruct();
                    }
                    break;
            }
        }

        public void Retarget(HurtBox target) {
            foreach (IUnitC unit in activeUnits) {
                unit.OverrideTarget(target);
            }
        }

        public void RequestDestruction(IUnitC unit, bool ignoreVengeance = false) {
            activeUnits.Remove(unit);
            unit.Destruct();
        }

        public void ReconfigureUnits() {
            UnitTypes.Clear();

            UnitTypes.Add(NullUnit);
            UnitTypes.Add((offenseFamily.skillDef as UnitSkillDef).UnitType);
            UnitTypes.Add((defenseFamily.skillDef as UnitSkillDef).UnitType);
            UnitTypes.Add((supportFamily.skillDef as UnitSkillDef).UnitType);

            index++;
            if (index >= UnitTypes.Count) {
                index = 0;
            }

            currentUnitType = UnitTypes[index];
        }

        public float GetAssemblyTime() {
            return currentUnitType.AssemblyTime * UnitAssemblyModifier;
        }

        public void FixedUpdate() {
            foreach (Unit unitType in UnitCatalog.allUnits) {
                List<IUnitC> units = activeUnits.Where(x => x.UnitType == unitType).ToList();

                for (int i = 0; i < units.Count; i++) {
                    units[i].PerformMovement(units.Count, i);
                }
            }

            if (activeUnits.Count < unitCap) {
                unitStopwatch += Time.fixedDeltaTime;
            }

            if (currentUnitType && unitStopwatch >= GetAssemblyTime()) {
                unitStopwatch = 0f;
                SpawnUnit();
            }
            
            AimPoint = body.inputBank.GetAimRay().GetPoint(40f);

            if (!MainHUDInstance) {
                MainHUDInstance = GameObject.Find("HUDSimple(Clone)");
                if (MainHUDInstance) {
                    MainHUDInstance = MainHUDInstance.transform.Find("MainContainer").Find("MainUIArea").gameObject;
                }
            }

            if (MainHUDInstance) {
                UIPrefabInstance.SetActive(MainHUDInstance.activeInHierarchy);
            }
        }
    }
}