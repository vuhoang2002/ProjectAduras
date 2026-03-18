using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static cbValue;

public class CombatController : AdurasMonobehavius
{
    public CombatHandData combatHandData;

    public int comboCount = 0;
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float targetDistance = 50f;
    public float firingTime = 10f;
    Animator animator;
    public HumanBodyBones leftHandBone = HumanBodyBones.LeftHand;
    public Camera mainCamera;
    public WeaponHit equipWeapon;
    public Vector2 offSetScreen;
    public CombatGetter cbGetter { get; set; }
    StateMachine _SM
    {
        get { return GetComponent<StateMachine>(); }
    }
    public SkillData currentSkillData;
    private AttackPhase currentPhase = AttackPhase.None;
    public AttackPhase CurrentPhase
    {
        get { return currentPhase; }
        set
        {
            currentPhase = value;
        }
    }
    public Transform Sword;
    public List<Transform> weaponTransforms = new List<Transform>();
    public string currentPhaseString;

    [field: SerializeField] public bool isGuard { get; private set; }
    private bool isStanding = true;
    public bool IsStanding
    {
        get { return isStanding; }
        set
        {
            isStanding = value;
            // if (isStanding)
            //     _SM.Animator.SetLayerWeight(2, 1);
            // if (!isStanding)
            //     _SM.Animator.SetLayerWeight(2, 0);
        }
    }

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        animator = GetComponent<Animator>();
        cbGetter = new CombatGetter(this);
        if (equipWeapon != null)
        {
            equipWeapon.setAttacker(GetComponent<Character>());
        }
        //firePoint = animator.GetBoneTransform(leftHandBone);

        // **LẤY VECTOR TRỌNG LỰC TỪ CÀI ĐẶT CỦA UNITY**
    }

    //called by impact and cooldown of AttackState
    #region CastAttack
    public void CastAttack(AttackProperty attackProperty, SkillDefinition skillDefinition, AttackData atkData, bool enable, int impactCount, bool isBaseOrSpecial)
    {
        SkillContext ctx = new SkillContext();
        DamageInfo damageInfo = new DamageInfo();
        if (enable)
        {
            ctx = new SkillContext
            {
                caster = transform,
                SkillDefinition = skillDefinition,
                damageMultiplier = 1f,

                AttackData = atkData
            };
            if (skillDefinition != null)
                ctx.castPos = GetCastPos(skillDefinition.spawnEffectPos);

            damageInfo = new DamageInfo
            {
                _damagePercent = atkData.DamagePercent[impactCount / 2],
                _schoolType = atkData.AttackElemental,
                _Type = atkData.AttackDamageType,
                _effectStatus = atkData.EffectStatus,
                _statScale = atkData.statPri
            };
            if (atkData.forceData.Count != 0)
            {
                damageInfo._force = atkData.forceData[impactCount / 2].forceDirection;
                damageInfo._knockOnEffect = atkData.forceData[impactCount / 2].knockOnEffect;
            }
        }
        switch (attackProperty)
        {
            case AttackProperty.Melee:
                ActiveWeaponHixBox(enable, damageInfo);
                break;
            case AttackProperty.Shooter:
                if (!enable) break;
                Vector3 endPoint = GetAnimPos();
                // Tự định nghĩa vận tốc ban đầu (ví dụ hướng từ firePoint tới endPoint, nhân với tốc độ)
                Vector3 direction = (endPoint - firePoint.position).normalized;
                ctx.initialVelocity = direction.normalized; // 20f là tốc độ, bạn chỉnh tùy ý
                break;
            case AttackProperty.MeleeRanger:
                if (!enable) break;
                Vector3 endPoint2 = GetAnimPos();
                Vector3 direction2 = new Vector3(firePoint.forward.x, 0, firePoint.forward.z).normalized;
                ctx.initialVelocity = direction2.normalized;
                break;
        }
        if (skillDefinition != null && enable)
        {
            foreach (var effect in skillDefinition.effects)
            {
                if (effect is ImpactEffect) continue;
                effect.Excute(ctx);
            }
        }
    }
    public void CastAttack(bool enable, AttackProperty attackProperty)
    {
        switch (attackProperty)
        {
            case AttackProperty.Melee:
                ActiveWeaponHixBox(enable);
                break;

        }
    }
    void ActiveWeaponHixBox(bool enable, DamageInfo info)
    {
        if (equipWeapon != null)
        {
            equipWeapon.EnableCollider(enable, info);
        }
    }
    public void ActiveWeaponHixBox(bool enable)
    {
        if (equipWeapon != null)
        {
            equipWeapon.EnableCollider(enable);
        }
    }
    #endregion
    public Vector3 GetAnimPos()
    {
        if (this.gameObject.CompareTag(AdurasLayer.Enemy))
        {
            if (_SM.currentTarget != null)
                return _SM.currentTarget.GetComponent<PlayerStateMachine>().Targeter.transform.position;
            else
                _SM._SwitchState(new EnemyIdleState(_SM as EnemyStateMachine));
        }
        return GetAimTargetPointForPlayer();
    }
    //player aim
    Vector3 GetAimTargetPointForPlayer()
    {
        Vector3 targetPoint;
        // Khởi tạo Raycast từ Camera, đi qua trung tâm màn hình (hồng tâm)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f + offSetScreen.x, 0.5f + offSetScreen.y, 0f));
        RaycastHit hit;
        // Định nghĩa một LayerMask để loại trừ (Ignore) các layer không cần thiết (Ví dụ: Player)
        // LayerMask.GetMask("Player") sẽ tạo ra một mask chỉ chứa layer "Player"
        // ~LayerMask.GetMask("Player") sẽ ĐẢO MASK, tức là CHỌN TẤT CẢ layer NGOẠI TRỪ "Player"
        int layerMask = ~LayerMask.GetMask("Player", "Ignore Raycast", "Default", "VisionSensor"); // Loại trừ Layer Player và Layer Ignore Raycast
                                                                                                   // Thực hiện Raycast. Sử dụng layerMask để loại bỏ va chạm với Player.
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // Trúng vật thể: Điểm kết thúc là điểm va chạm
            targetPoint = hit.point;
            Debug.Log("🎯 Ngắm: " + hit.collider.name + " tại " + targetPoint);
        }
        else
        {
            // Không trúng vật thể (hoặc chỉ trúng vật thể đã bị loại trừ trong LayerMask)
            // Lấy điểm giả định cách xa camera một khoảng (targetDistance)
            targetPoint = ray.GetPoint(targetDistance);
        }
        return targetPoint;
    }
    public void RotateToTarget(Transform target, float tick)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0; // Giữ nguyên trục Y để tránh nghiêng
        if (direction == Vector3.zero) return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, tick * 50f);
    }
    public void ResetComboCount()
    {
        comboCount = 0;
    }
    public void addComboCount()
    {
        comboCount++;
    }
    public int getComboCount()
    {
        return comboCount;
    }
    public void SetAttackPhase(AttackPhase phase, SkillData skdt, bool isBaseOrSpecial)
    {
        CurrentPhase = phase;
        currentPhaseString = phase.ToString();
        if (phase == AttackPhase.WindUp || phase == AttackPhase.Charging)
        {
            IsStanding = false;
        }
        else if (phase == AttackPhase.Over) IsStanding = true;
    }
    public void SetAttackPhase(AttackPhase phase, SkillData skdt, bool isBaseOrSpecial, int impactCount)
    {
        CurrentPhase = phase;
        currentPhaseString = phase.ToString();
        if (phase == AttackPhase.Impact)
        {
            //todo: turn on hitbox
            CastAttack(cbGetter.GetAttackPropertyFromSKill(skdt),//for effect
            cbGetter.GetSkillDefinition_Attack(skdt, getComboCount(), impactCount / 2, isBaseOrSpecial),//combo count 
            cbGetter.GetAttackData_Attack(skdt, getComboCount(), isBaseOrSpecial),
          true, impactCount, isBaseOrSpecial);//on hitbox
        }
        else if (phase == AttackPhase.CoolDown)
        {
            //todo: turn off hitbox
            Debug.Log("Cooldown : " + impactCount / 2);
            // CastAttack(cbGetter.GetAttackPropertyFromSKill(skdt),
            //  cbGetter.GetSkillDefinition_Attack(skdt, getComboCount(), impactCount / 2 - 1, isBaseOrSpecial),
            // cbGetter.GetAttackData_Attack(skdt, getComboCount(), isBaseOrSpecial),
            //false, impactCount, isBaseOrSpecial);//off hitbox
            CastAttack(false, cbGetter.GetAttackPropertyFromSKill(skdt));
        }
        else if (phase == AttackPhase.WindUp || phase == AttackPhase.Charging)
        {
            IsStanding = false;
        }
        else if (phase == AttackPhase.Over) IsStanding = true;
    }
    public void ResetAttackPhase()
    {
        CurrentPhase = AttackPhase.None;
        currentPhaseString = CurrentPhase.ToString();
    }
    public bool IsPhase(AttackPhase phase)
    {
        return CurrentPhase == phase;
    }
    //hieenj tại cái này dùng cho lúc niệm phép
    public void CastEffectWhenCharge(SkillDefinition effectCharging)
    {
        SkillContext ctx = new SkillContext
        {
            castPos = GetCastPos(effectCharging.spawnEffectPos),
            damageMultiplier = 1f,
            SkillDefinition = effectCharging
        };
        effectCharging.context = ctx;
        foreach (var effect in effectCharging.effects)
        {
            if (effect is ImpactEffect) continue;
            effect.Excute(ctx);
        }
    }
    public void DestroyEffect(SkillContext ctx)
    {
        foreach (var e in ctx.spawnedEffects)
        {
            if (e == null) continue;

            PooledEffect p = e.GetComponent<PooledEffect>();
            if (p != null)
                p.Despawn();
        }

        ctx.spawnedEffects.Clear();
    }
    public void SetGuard(bool value)
    {
        isGuard = value;
    }
    private Transform GetCastPos(SpawnEffectPos sep)
    {
        switch (sep)
        {
            case SpawnEffectPos.RightArm:
                return weaponTransforms[0];
            case SpawnEffectPos.LeftArm:
                return weaponTransforms[1];
            case SpawnEffectPos.RightWeapon:
                return weaponTransforms[0];
            case SpawnEffectPos.LeftWeapon:
                return weaponTransforms[1];
            case SpawnEffectPos.Feet:
                return this.transform;
            case SpawnEffectPos.CenterBody:
                return this.firePoint;

        }
        return this.transform;
    }
}




