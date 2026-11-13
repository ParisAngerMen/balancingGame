using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WizardAttack : EnemyAttackState
{
    private readonly WizardController wizard;
    private readonly HashSet<IDamageable> damagedTargets = new HashSet<IDamageable>();

    public WizardAttack(WizardController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        wizard = enemy;
    }

    protected override void PerformAttack(GameObject target)
    {
        // Area damage centred on the target
        Collider[] hits = Physics.OverlapSphere(target.transform.position, wizard.attackRadius,
            enemy.enemyMask, QueryTriggerInteraction.Collide);

        damagedTargets.Clear();

        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            // Each unit is only damaged once per attack, even if it has several colliders
            if (damageable != null && damagedTargets.Add(damageable))
            {
                damageable.TakeDamage(enemy.damage);
            }
        }

        if (damagedTargets.Count > 0)
        {
            PlayAttackSound();
        }

        damagedTargets.Clear();
    }
}
