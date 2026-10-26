using UnityEngine;
[System.Serializable]
public class EnemyStateMachine
{
    public EnemyState CurrentEnemyState {get; set;}

    public void Initialize(EnemyState startingState)
    {
        CurrentEnemyState = startingState;
        CurrentEnemyState.EnterState();
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentEnemyState.ExitState(); 
        Debug.Log("Changning state: " + newState);
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();  
    }
    
}
