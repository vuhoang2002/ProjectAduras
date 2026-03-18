using UnityEngine;

public abstract class EnemyBaseState : State
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected EnemyStateMachine _SMch;
    protected CombatController _CbCtrl;


    public EnemyBaseState(EnemyStateMachine stateMachine)
    {
        _SMch = stateMachine;
        _CbCtrl = _SMch.CombatController;
    }
    public EnemyBaseState(EnemyStateMachine stateMachine, bool flag) : base()
    {
        _SMch = stateMachine;
    }
    protected void AdurasMove(Vector3 motion, float deltaTime)
    {
        _SMch.CharacterController.Move((motion + _SMch.ForceReceiver.getMovement()) * deltaTime);

    }
    protected void AdurasMove(float deltaTime)
    {
        _SMch.CharacterController.Move((Vector3.zero + _SMch.ForceReceiver.getMovement()) * deltaTime);
    }
    public void SearchingPlayerByFOV()
    {
        if (_SMch.TargetInRange.Count <= 0)
        {
            _SMch.Target = null;
            return;
        }
        foreach (var target in _SMch.TargetInRange)
        {
            Vector3 directionToTarget = (target.transform.position - _SMch.transform.position).normalized;
            float angleToTarget = Vector3.Angle(_SMch.transform.forward, directionToTarget);
            if (angleToTarget < _SMch.Fov / 2)
            {
                _SMch.Target = target;
                _SMch._SwitchState(new EnemyCombatState(_SMch));
                break;
            }
        }
    }

}
