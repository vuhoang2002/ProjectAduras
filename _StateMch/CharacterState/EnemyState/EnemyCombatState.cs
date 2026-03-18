
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyCombatState : EnemyBaseState
{
    public float stoppingDistand = 3f;//distance to stand
    float adujstDistaneThrehold = 0.5f;
    [SerializeField] private Vector2 idleRandomTime = new Vector2(1f, 3f);
    [SerializeField] private Vector2 circlingRandomTime = new Vector2(2f, 5f);
    int cirlingDirection = 1;//left or right
    private float circlingSpeed = 30f;


    private float timer = 0f;

    public EnemyCombatState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }


    public override void _OnEnter()
    {
        _SMch.Agent.stoppingDistance = stoppingDistand;// the space distand between player and gameobject when stop run
        _SMch.Animator.CrossFadeInFixedTime(AdurasAnimHash.CombatBlendTreeHash, 0.2f);
        _SMch.CombatMovementTimer = 0f;
        _SMch.eCbBehavius = eCombatState.Ready;
    }

    public override void _OnExit()
    {
        //_SM.Agent.ResetPath();
        _SMch.CombatMovementTimer = 0f;
    }
    public override void _OnUpdate(float tick)
    {
        if (_SMch.Target == null)
        { _SMch._SwitchState(new EnemyIdleState(_SMch)); return; }
        //  Vector3 distanToPlayer = Vector3.Distance(_SM.transform.position, _SM.Target.transform.position);
        if (Vector3.Distance(_SMch.transform.position, _SMch.Target.transform.position) > stoppingDistand + adujstDistaneThrehold)
        {
            StartChase();
        }
        if (_SMch.eCbBehavius == eCombatState.Ready)
        {
            if (timer <= 0f)
            {
                int cbBehavius = Random.Range(0, 3);
                switch (cbBehavius)
                {
                    case 0:
                        StartIdle();
                        break;
                    case 1:
                        StartCircling();
                        break;
                    case 2:
                        _SMch._SwitchState(new EnemyGuardState(_SMch));
                        break;
                }
            }

        }
        else if (_SMch.eCbBehavius == eCombatState.Chase)//use this if player go far away form this enemy 
        {
            if (Vector3.Distance(_SMch.transform.position, _SMch.Target.transform.position) < stoppingDistand + adujstDistaneThrehold)
            {
                StartIdle();
                return;
            }
            //enemy run toward player
            _SMch.Agent.SetDestination(_SMch.Target.transform.position);


        }
        else if (_SMch.eCbBehavius == eCombatState.Circling)
        {

            if (timer <= 0f)
            {
                StartIdle();
                return;
            }
            //  _SM.transform.RotateAround(_SM.Target.transform.position, Vector3.up, cirlingDirection * -circlingSpeed * Time.deltaTime);
            Vector3 vectorToTarget = _SMch.transform.position - _SMch.Target.transform.position;
            var rotatePos = Quaternion.Euler(0, -cirlingDirection * circlingSpeed * Time.deltaTime, 0) * vectorToTarget;
            Vector3 finalPos = rotatePos - vectorToTarget;
            _SMch.Agent.Move(finalPos);
            _SMch.transform.rotation = Quaternion.LookRotation(-rotatePos);
            // _SM.transform.LookAt(_SM.Target.transform);
        }
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        _SMch.CombatMovementTimer += Time.deltaTime;
    }

    private void StartIdle()
    {
        _SMch.eCbBehavius = eCombatState.Ready;
        _CbCtrl.SetGuard(false);
        timer = Random.Range(idleRandomTime.x, idleRandomTime.y);
    }

    private void StartChase()
    {
        _SMch.eCbBehavius = eCombatState.Chase;
    }
    private void StartCircling()
    {
        _SMch.eCbBehavius = eCombatState.Circling;
        if (Random.Range(0, 3) == 0)
            _CbCtrl.SetGuard(true);
        _SMch.Agent.ResetPath();
        timer = Random.Range(circlingRandomTime.x, circlingRandomTime.y);
        cirlingDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    }
    private void MoveTowardPlayer(float deltaTime)
    {
        _SMch.Agent.destination = _SMch.Target.transform.position;
        AdurasMove(_SMch.Agent.desiredVelocity.normalized * _SMch.MoveSpeed, deltaTime);
        _SMch.Agent.velocity = _SMch.CharacterController.velocity;
    }
    private void MoveTowardPlayerOld(float deltaTime)
    {
        _SMch.Agent.SetDestination(_SMch.Target.transform.position);

    }

}
