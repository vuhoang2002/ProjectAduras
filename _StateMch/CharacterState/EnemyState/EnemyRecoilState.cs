using System.Collections;
using UnityEngine;

public class EnemyRecoilState : EnemyBaseState
{
    private Coroutine recoilCoroutine;
    public EnemyRecoilState(EnemyStateMachine stateMachine) : base(stateMachine)
    {

    }

    public override void _OnEnter()
    {
        _SMch.eCbBehavius = eCombatState.Recoil;
        _SMch.Agent.ResetPath();
        _SMch.Agent.enabled = false;
        _SMch._RootMotion = true;
        recoilCoroutine = _SMch.StartCoroutine(RecoilCoroutine());

    }

    public override void _OnExit()
    {
        _SMch.Agent.enabled = true;
        _SMch._RootMotion = false;
        if (recoilCoroutine != null)
        {
            _SMch.StopCoroutine(recoilCoroutine);
        }
    }

    public override void _OnUpdate(float tick)
    {
        AdurasMove(tick);
    }
    private IEnumerator RecoilCoroutine()
    {
        _SMch.Animator.CrossFade(AdurasAnimHash.Recoil, 0.1f);
        yield return null;
        var aState = _SMch.Animator.GetNextAnimatorClipInfo(0);
        float timer = 0f;
        while (timer < aState.Length)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _SMch._SwitchState(new EnemyIdleState(this._SMch));

    }
}
