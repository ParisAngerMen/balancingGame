using UnityEngine;

[System.Serializable]
public class ArcherAttack : EnemyAttackState
{
    private readonly ArcherController archer;

    public ArcherAttack(ArcherController enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
        archer = enemy;
    }

    protected override void FaceTarget(GameObject target)
    {
        // Uses the archer's current position (not the one it was spawned at)
        Vector3 dir = target.transform.position - archer.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            archer.transform.rotation = Quaternion.Slerp(
                archer.transform.rotation,
                Quaternion.LookRotation(dir),
                archer.TurnSpeed * Time.deltaTime);
        }
    }

    protected override void PerformAttack(GameObject target)
    {
        archer.Shoot(target);
    }
}
