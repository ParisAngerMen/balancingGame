using UnityEngine;

[System.Serializable]
public class EnemyStateMachine
{
    public EnemyState CurrentEnemyState {get; set;}

    public void Initialize(EnemyState startingState)
    {
        CurrentEnemyState = startingState;
        CurrentEnemyState?.EnterState();
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == null || newState == CurrentEnemyState)
        {
            return;
        }

        CurrentEnemyState?.ExitState();
        Debug.Log("Changing state: " + newState);
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }
}
