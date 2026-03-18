using UnityEngine;
using static cbValue;

public abstract class PlayerBaseState : State
{
    protected PlayerStateMachine _SMch;
    protected CombatController _CbCtrl;
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        _SMch = stateMachine;
        _CbCtrl = _SMch.GetComponent<CombatController>();
    }
    // protected void Move(Vector3 motion, float deltaTime)
    // {
    //     _SM.CharacterController.Move(motion + _SM.ForceReceiver.Movement * deltaTime);
    // }
    protected bool CheckPressingAttack()
    {
        //if (menuOn) { return; }
        if (_SMch.InputReader.IsPActive || _SMch.InputReader.IsSActive)
        {
            return true;
        }
        return false;
    }
    protected void AdurasMove(Vector3 motion, float deltaTime)
    {
        _SMch.CharacterController.Move((motion + _SMch.ForceReceiver.getMovement()) * deltaTime);
    }
    protected void AdurasMove(float deltaTime)
    {
        _SMch.CharacterController.Move((Vector3.zero + _SMch.ForceReceiver.getMovement()) * deltaTime);
    }
    public bool AttackCostCalculation(AttackData atkDt)
    {
        bool allowAttack = true; // Biến kiểm tra tổng thể cho phép tấn công

        // Kiểm tra chi phí cho Health (HP)
        if (atkDt.AttackCostTypes[AttackCost.Health] != 0)
        {
            allowAttack &= _SMch.Health.HP.Consume(atkDt.AttackCostTypes[AttackCost.Health]); // Giả sử có hàm Consume cho HP
        }

        // Kiểm tra chi phí cho Magika (Mana)
        if (atkDt.AttackCostTypes[AttackCost.Magika] != 0) // Thay thế Magika bằng Mana
        {
            allowAttack &= _SMch.Health.MP.Consume(atkDt.AttackCostTypes[AttackCost.Magika]);
        }

        // Kiểm tra chi phí cho Stamina (SP)
        if (atkDt.AttackCostTypes[AttackCost.Stamina] != 0)
        {
            allowAttack &= _SMch.Health.SP.Consume(atkDt.AttackCostTypes[AttackCost.Stamina]);
        }

        return allowAttack; // Trả về true nếu đủ tài nguyên, ngược lại false
    }
}

