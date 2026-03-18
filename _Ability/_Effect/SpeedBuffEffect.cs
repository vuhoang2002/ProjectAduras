using System.Collections;
using UnityEngine;
[CreateAssetMenu(
    fileName = "SpeedBuff",
    menuName = "SpawnEffect/SpeedBuff"
)]
public class SpeedBuffEffect : EffectDefinition
{
    public float speedMultiplier = 1.5f;
    public float duration = 5f;
    public override void Destroy()
    {
    }

    public override void Excute(SkillContext ctx)
    {
        StateMachine character = ctx.caster.GetComponent<StateMachine>();
        if (character == null) return;

        character.StartCoroutine(ApplySpeedBuff(character));
    }
    private IEnumerator ApplySpeedBuff(StateMachine player)
    {
        if (player.Character.isSpeedBuff)
        {
            yield break;
        }
        float originalRun = player.runSpeed;
        //  float originalWalk = player.walkSpeed;
        player.Character.isSpeedBuff = true;

        // Buff
        player.runSpeed *= speedMultiplier;
        // player.walkSpeed *= speedMultiplier;

        yield return new WaitForSeconds(duration);

        // Reset
        player.runSpeed = originalRun;
        //player.walkSpeed = originalWalk;
        player.Character.isSpeedBuff = false;
    }
}
