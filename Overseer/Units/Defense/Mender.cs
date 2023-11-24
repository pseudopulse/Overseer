using System;
using Overseer.Components;

namespace Overseer.Units.Defense {
    public class MenderController : UnitController
    {
        public override HurtBox Target { get; set; }
        private float stopwatch = 0f;
        public override HurtboxTracker.TargetType TargetType => HurtboxTracker.TargetType.Friendly;
        private bool formationClose = true;

        public override void OverrideTarget(HurtBox newTarget)
        {
            
        }

        public override void PerformMovement(int totalUnits, int thisUnit)
        {
            if (formationClose) {
                OrbitalMovement(totalUnits, thisUnit, 360f, 0f, 1.5f);
            }
            else {
                OrbitalMovement(totalUnits, thisUnit, 20f, (20f / 2f) * -1f, 3f);
            }
        }

        public override void PerformSecondaryAction()
        {
            return;
            
            List<CharacterBody> bodies = CharacterBody.readOnlyInstancesList.Where(x => x.GetComponent<OverseerController>() || x.GetComponent<UnitController>()).ToList();
            foreach (CharacterBody body in bodies) {
                if (Vector3.Distance(Target.transform.position, body.corePosition) < 15) {
                    body.healthComponent.AddBarrier(body.maxHealth * (0.10f * owner.UnitEffectModifier));
                }
            }

            EffectManager.SpawnEffect(Assets.GameObject.HealthOrbFlash, new EffectData {
                origin = base.transform.position,
                scale = 5f
            }, true);
        }

        public override void Recall()
        {
            formationClose = !formationClose;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            if (!Target) {
                Target = owner.body.mainHurtBox;
            }

            if (stopwatch >= 1f && Target) {
                stopwatch = 0f;
                
                List<CharacterBody> bodies = CharacterBody.readOnlyInstancesList.Where(x => x.GetComponent<OverseerController>() || x.GetComponent<UnitController>()).ToList();
                foreach (CharacterBody body in bodies) {
                    if (Vector3.Distance(Target.transform.position, body.corePosition) < 15) {
                        body.healthComponent.HealFraction(0.015f * owner.UnitEffectModifier, new());
                    }
                }
            }
        }
    }
}