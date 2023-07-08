using System;

namespace Overseer.Units {
    [CreateAssetMenu(menuName = "Overseer/Unit", fileName = "Unit")]
    public class Unit : ScriptableObject {
        public string Name;
        public string Description;
        public Sprite Icon;
        public GameObject Prefab;
        public float AssemblyTime;
        public UnitIndex UnitType;

        public void Initialize() {
            return;
            string langModifier = this.GetType().Name.ToUpper();
            ("UNIT_" + langModifier + "_NAME").Add(Name);
            ("UNIT_" + langModifier + "_DESC").Add(Description);
        }
    }

    public enum UnitIndex {
        Sentinel,
        Swarmer,
        Warden,
        Mender,
        Beacon,
        Battery
    }
}