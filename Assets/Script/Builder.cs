using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Builder : BaseInteractionPlayer
{
    public Text txtBuilder;
    public string nameBuilder;
    public int cost = 20;
    System.Action actionUpdate = null;
    private void Start()
    {
        txtBuilder.text = cost.ToString();
    }
    public override void DoUpdate()
    {
        base.DoUpdate();
        actionUpdate?.Invoke();
    }
    protected override void Interaction(Person player)
    {
        if(player is Player)
        {
            System.Action action = null;
            float t = 0;
            float tDelayStart = 1.5f;
            action = () =>
            {
                tDelayStart -= Time.deltaTime;
                if (tDelayStart > 0)
                    return;
                t -= Time.deltaTime;
                if(t<0)
                {
                    if (GameManager.Instance.IsEmptyMoney || cost ==0)
                        return;
                    GameManager.Instance.AddMoney(-1);
                    cost--;
                    txtBuilder.text = cost.ToString();
                    var money = MyPreb.GetPreb("Money");
                    money.transform.localScale = Vector3.one * 2;
                    Vector3 posStart = player.transform.position;
                    Vector3 posTarget = transform.position;
                    float tDelay = 0.3f;
                    System.Action action = null;
                    money.transform.position = posStart;
                    money.transform.DOJump(posTarget, 2.5f, 1, tDelay);
                    action = () =>
                    {
                        tDelay -= Time.deltaTime;
                        if(tDelay<0)
                        {
                            GameObject.Destroy(money.gameObject);
                            actionUpdate -= action;
                            if(cost==0)
                            {
                                
                                Spawn();
                            }    
                        }    
                    };
                    actionUpdate += action;
                    t = 0.1f;
                }    
            };
            actionUpdateInteraction += action;
        }    
    }
    void Spawn()
    {
        var obj = MyPreb.GetPreb(nameBuilder);
        obj.transform.position = new Vector3(transform.position.x,obj.transform.position.y,transform.position.z);
        gameObject.SetActive(false);
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(Vector3.one, 0.5f);
    }    
}
