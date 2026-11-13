using UnityEngine;

public class WizardController : EnemyController
{
    [Header("Wizard")]
    [SerializeField] public float attackRadius;

    public override void SetAttack()
    {
        AttackState = new WizardAttack(this, StateMachine);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        // The state machine only exists while playing
        if (StateMachine == null || enemyHit == null || StateMachine.CurrentEnemyState != AttackState)
        {
            return;
        }

        Gizmos.color = Color.brown;
        Gizmos.DrawWireSphere(enemyHit.transform.position, attackRadius);
    }
}
