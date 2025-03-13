using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Config
{
    public static float speed;
    public static int MaxCarry = 6;
    public static float spaceCarry = 0.35f;
}
public class Player: Person
{
    #region Move
    float speed = 5f;
    public FixedJoystick joyStick;
    public Vector3 directCurr;
    System.Action actionMove;
    System.Action actionWaitCarry;
    System.Action actionUnCarry;
    #endregion

    protected override void Start()
    {
        base.Start();
        this.amountMaxCarry = Config.MaxCarry;
        System.Action actionMove = () =>
        {
            var direct = new Vector3(joyStick.Direction.x, 0, joyStick.Direction.y);
            direct = direct.normalized;
            directCurr = direct.normalized;
            if(direct.magnitude!=0)
            {
                transform.rotation = Quaternion.LookRotation(direct);
                // transform.position += direct * speed;
            }
            body.velocity = direct * speed;
            Run();
        };
       
        joyStick.actionPointUp += () =>
        {
            body.velocity = Vector3.zero;
            Idle();
            this.actionMove -= actionMove;
        };
        joyStick.actionPointDown += () =>
        {
            this.actionMove += actionMove;
        };
    }
    void Run()
    {
        if (amountCurrentCarry != 0)
        {
            PlayAnim(TypeAnim.CarryMove);
        }
        else
        {
            PlayAnim(TypeAnim.Run);
        }
    }
    void Idle()
    {
        if (amountCurrentCarry != 0)
        {
            PlayAnim(TypeAnim.CarryIdle);
        }
        else
        {
            PlayAnim(TypeAnim.Idle);
        }
    }
    private void Update()
    {
        actionMove?.Invoke();
        actionCarry?.Invoke();
    }
}
