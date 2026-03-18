using System.Threading;
using UnityEngine;

public class EnemyGuardState : EnemyBaseState
{
    private Vector2 guardTime = new Vector2(1.5f, 5);
    int cirlingDirection = Random.Range(0, 2) == 0 ? -1 : 1;
    float circlingSpeed = 1f;
    private float timer;
    public EnemyGuardState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void _OnEnter()
    {
        _SMch.eCbBehavius = eCombatState.Guard;
        timer = Random.Range(guardTime.x, guardTime.y);
        _CbCtrl.SetGuard(true);
        _SMch.EnterCombatMode = true;
    }

    public override void _OnExit()
    {
        _SMch.Animator.SetLayerWeight(1, 0);
        _CbCtrl.SetGuard(false);
    }

    public override void _OnUpdate(float tick)
    {
        AdurasMove(tick);
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            _SMch._SwitchState(new EnemyCombatState(this._SMch));
        }
    }
}
