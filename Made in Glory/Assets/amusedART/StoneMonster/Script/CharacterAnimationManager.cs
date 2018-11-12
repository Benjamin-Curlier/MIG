using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Idle() => anim.SetTrigger("StopMoving");
    public void Run() => anim.SetTrigger("Movement");
    public void CastingSpell() => anim.SetTrigger("CastingSpell");
    public void CastingHeal() => anim.SetTrigger("CastingHeal");
    public void TakingDamage() => anim.SetTrigger("TakingDamage");
    public void IsDead(bool parm) => anim.SetBool("IsDead", parm);
}
