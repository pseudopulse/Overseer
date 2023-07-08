using System;
using Overseer.Components;

namespace Overseer.Units {
    public static class UnitCatalog {
        public static List<Unit> allUnits;
        public static void Initialize() {
            allUnits = Main.assets.LoadAllAssets<Unit>().ToList();

            foreach (Unit u in allUnits) {
                u.Initialize();
            }
        }

        public static Unit GetUnitType(UnitIndex index) {
            return allUnits.FirstOrDefault(x => x.UnitType == index);
        }

        public static GameObject SpawnUnit(OverseerController owner, Unit type) {
            GameObject unit = GameObject.Instantiate(type.Prefab, owner.transform.position, Quaternion.identity);
            IUnitC unitC = unit.GetComponent<IUnitC>();
            unitC.Initialize(owner);
            NetworkServer.Spawn(unit);
            return unit;
        }
    }
}