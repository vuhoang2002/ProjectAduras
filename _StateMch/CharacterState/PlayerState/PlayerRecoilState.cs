using System.Collections;
using UnityEngine;

public class PlayerRecoilState : PlayerBaseState
{
    public PlayerRecoilState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }
    private Coroutine recoilCoroutine;

    public override void _OnEnter()
    {
        _SMch._RootMotion = true;
        _SMch.curentState = "Recoil";
        recoilCoroutine = _SMch.StartCoroutine(RecoilCoroutine());
    }

    public override void _OnExit()
    {
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
        _SMch._SwitchState(new PlayerFreeLookState(this._SMch));

    }

}
