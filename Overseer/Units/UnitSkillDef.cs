using System;

namespace Overseer.Units {
    [CreateAssetMenu(fileName = "UnitSkillDef", menuName = "Overseer/UnitSkillDef")]
    public class UnitSkillDef : SkillDef {
        public Unit UnitType;
    }
}