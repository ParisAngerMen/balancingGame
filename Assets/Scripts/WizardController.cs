using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AI;

public class WizardController : EnemyController
{
    [Header("Wizard")]
    [SerializeField] private float attackRadius;

    public override void SetAttack()
    {
        AttackState = new WizardAttack(this, StateMachine);
    }
}
