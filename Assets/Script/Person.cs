using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Person : MonoBehaviour
{
    public List<GameObject> allCarry = new List<GameObject>();
    protected TypeAnim animCurr = TypeAnim.None;
    protected Animator animator;
    protected Rigidbody body;

    #region Carry
    public int amountCurrentCarry;
    public int amountMaxCarry;
    public Transform posStartCarry;
    protected System.Action actionCarry;
    protected System.Action actionAddCarry;
    public bool IsFull
    {
        get
        {
            return amountCurrentCarry >= amountMaxCarry;
        }
    }
    public bool IsEmpty
    {
        get
        {
            return amountCurrentCarry ==0;
        }
    }
    #endregion

    protected virtual void  Start()
    {
        animator = transform.GetComponentInChildren<Animator>();
        body = transform.GetComponentInChildren<Rigidbody>();
       
    }

    protected void Move()
    {

    }
    public void PlayAnim(TypeAnim anim)
    {
        if (animCurr == anim)
        {
            return;
        }
        animCurr = anim;
        switch (anim)
        {
            case TypeAnim.Run:
                animator.SetBool("IsMove", true);
                animator.SetBool("IsEmpty", true);
                break;
            case TypeAnim.Idle:
                animator.SetBool("IsMove", false);
                animator.SetBool("IsEmpty", true);
                break;
            case TypeAnim.CarryIdle:
               
                animator.SetBool("IsCarryMove", false);
                animator.SetBool("IsEmpty", false);
                break;
            case TypeAnim.CarryMove:
                animator.SetBool("IsCarryMove", true);
                animator.SetBool("IsEmpty", false);
                break;
        }
    }
   
    public void AddCarryRs(GameObject obj)
    {
       
        allCarry.Add(obj);
        var rsCarry = obj;
        rsCarry.transform.rotation = Quaternion.identity;
        rsCarry.transform.localScale = Vector3.zero;
        rsCarry.transform.DOScale(Vector3.one, 0.1f);
        var target = this.amountCurrentCarry * Vector3.up * 0.35f;
        rsCarry.transform.parent = posStartCarry;
        rsCarry.transform.DOLocalJump(target,2,1,0.25f);
        this.amountCurrentCarry += 1;
        if(animCurr == TypeAnim.Idle)
        {
            PlayAnim(TypeAnim.CarryIdle);
        }
        if (animCurr == TypeAnim.Run)
        {
            PlayAnim(TypeAnim.CarryMove);
        }

    }
    public GameObject RemoveCarryRs()
    {
        var obj = allCarry[this.amountCurrentCarry- 1];
        allCarry.Remove(obj);
        this.amountCurrentCarry--;
        if(amountCurrentCarry==0)
        {
            if (animCurr == TypeAnim.CarryMove)
            {
                PlayAnim(TypeAnim.Run);
            }
            if (animCurr == TypeAnim.CarryIdle)
            {
                PlayAnim(TypeAnim.Idle);
            }

        }




        return obj;
    }

}
