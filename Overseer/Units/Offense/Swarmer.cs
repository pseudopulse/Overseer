using System;

namespace Overseer.Units {
    public class SwarmerController : UnitController
    {
        public override HurtBox Target { get; set; }

        private bool isCurrentlyInDashAttack = false;
        private float stopwatch = 0f;
        private float attackCooldown = Random.Range(1f, 3f);
        private Vector3 forwardVec;
        private float stopwatch2;
        public ContactDamage contactDamage;
        private bool shouldTarget = true;

        public override void OverrideTarget(HurtBox newTarget)
        {
            Target = newTarget;
        }

        public override void PerformMovement(int totalUnits, int thisUnit)
        {
            if (isCurrentlyInDashAttack) {
                DashMovement();
            }
            else {
                OrbitalMovement(totalUnits, thisUnit, 360f, 0f, 3.5f);
            }
        }

        public override void PerformSecondaryAction()
        {
            
        }

        public void DashMovement() {
            Vector3 pos = Vector3.Lerp(base.transform.position, base.transform.position + (forwardVec * 3f), 20f * Time.fixedDeltaTime);
            base.transform.position = pos;
        }

        public void Cancel() {
            isCurrentlyInDashAttack = false;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            contactDamage.overlapAttack.attacker = owner.gameObject;

            if (!isCurrentlyInDashAttack) {
                stopwatch += Time.fixedDeltaTime;
            }

            if (stopwatch >= attackCooldown) {
                stopwatch = 0f;
                attackCooldown = Random.Range(1f, 3f);

                if (Target) {
                    isCurrentlyInDashAttack = true;
                    forwardVec = (Target.transform.position - base.transform.position).normalized;
                    Invoke(nameof(Cancel), 3f);
                }
                else {
                    Target = FindNearbyEnemy();
                }
            }

            if (!Target) {
                isCurrentlyInDashAttack = false;
            }

            if (Target && !Target.isActiveAndEnabled) {
                Target = null;
            }

            stopwatch2 += Time.fixedDeltaTime;

            if (stopwatch2 >= 0.5f && isCurrentlyInDashAttack) {
                forwardVec = (Target.transform.position - base.transform.position).normalized;
                stopwatch2 = 0f;
            }

            if (isCurrentlyInDashAttack) {
                base.transform.rotation = Quaternion.identity;
                base.transform.forward = forwardVec;
            }
            else {
                base.transform.rotation = Quaternion.Euler(-90, 0, 0);
            }
        }

        public override void Recall()
        {
            if (isCurrentlyInDashAttack) {
                isCurrentlyInDashAttack = false;
            }

            shouldTarget = !shouldTarget;
        }
    }
}