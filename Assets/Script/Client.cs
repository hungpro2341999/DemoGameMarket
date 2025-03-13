using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.AI;
public enum TypeStatus { Wait,Done}
public enum TypeEmoji { None,Cash,Smile}
public class Client : Person
{
    public NavMeshAgent agent;
    public TypeStatus status;
    public Text txtAmount;
    public bool IsComplete =false;
    System.Action actionUpdate = null;
    System.Action actionMoveCasher = null;
    GameObject uiClient;
    Market market;
    public void StartShopping()
    {
        uiClient = MyPreb.GetPreb("UIClient");
        uiClient.transform.position = transform.position;
        txtAmount = uiClient.transform.GetChild(0).GetChild(2).GetComponent<Text>();
        agent = GetComponent<NavMeshAgent>();
        base.Start();
        MoveToMarket();
        amountCurrentCarry = 0;
        amountMaxCarry = UnityEngine.Random.Range(1, 5);
        actionUpdate += UpdateUI;
    }
    private void Update()
    {
        actionUpdate?.Invoke();
        actionMoveCasher?.Invoke();
    }
    void UpdateUI()
    {
        uiClient.transform.position = transform.position;
        txtAmount.text = amountCurrentCarry + "/" + amountMaxCarry;
    }    
    public void MoveToMarket()
    {
        status = TypeStatus.Wait;
        PlayAnim(TypeAnim.Run);
        market = GameManager.Instance.GetMarket();
        var transTargetPos = market.GetPosTarget;
        var targetPos = transTargetPos.transform.position;
        market.AddPerson(this);
        market.AddSlot(transTargetPos);
        System.Action action = null;
      
        action = () =>
        {
            var target = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            // transform.position = Vector3.MoveTowards(transform.position,target,Time.deltaTime*5);
            agent.destination = target ;
            if (Vector2.Distance(transform.position,target)<0.1f)
            {
                status = TypeStatus.Done;
                PlayAnim(TypeAnim.Idle);
                actionUpdate -= action;
                Vector3 direction = (market.transform.position - transform.position).normalized;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.DORotateQuaternion(targetRotation, 0.35f)
                .SetEase(Ease.Linear);
                WaitBuy(()=> 
                {
                    market.RemoveSlot(transTargetPos);
                });
            }
        };
        actionUpdate += action;
    }

    public void WaitBuy(System.Action actionDone)
    {
        System.Action action = null;
        action = () =>
        {
            if(amountCurrentCarry!=0)
            {
                PlayAnim(TypeAnim.CarryIdle);
            }    
            if (IsFull)
            {
                uiClient.transform.GetChild(0).gameObject.SetActive(false);
                EnableEmoji(TypeEmoji.Cash);
                market.RemovePerson(this);
                actionDone?.Invoke();
                actionUpdate -= action;
                MoveToCasher();
            }
        };
        actionUpdate += action;
    }
    public void MoveToCasher()
    {
        status = TypeStatus.Wait;
        PlayAnim(TypeAnim.CarryMove);
        var casher = GameManager.Instance.casher;
        System.Action action = null;
        var target = casher.posWaitClient.transform.position + Vector3.right * casher.allClient.Count;
        casher.AddClient(this);
        target = new Vector3(target.x, transform.position.y, target.z);
        action = () =>
        {
            agent.destination = target;
          //  transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);
            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                ResetAgent();
                status = TypeStatus.Done;
                PlayAnim(TypeAnim.CarryIdle);
                actionMoveCasher -= action;
            }
        };
        actionMoveCasher += action;
    }

    public void MoveQueueCasher(int queue)
    {
        actionMoveCasher = null;
        status = TypeStatus.Wait;
        PlayAnim(TypeAnim.CarryMove);
        var casher = GameManager.Instance.casher;
        System.Action action = null;
        var target = casher.posWaitClient.transform.position + Vector3.right * queue;
        target = new Vector3(target.x, transform.position.y, target.z);
        action = () =>
        {
            agent.destination = target;
           // transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * 2);
            if (Vector2.Distance(transform.position, target) < 0.1f)
            {
                ResetAgent();
                status = TypeStatus.Done;
                PlayAnim(TypeAnim.CarryIdle);
                actionMoveCasher -= action;
            }
        };
        actionMoveCasher += action;

    }
    void ResetAgent()
    {
        agent.velocity = Vector3.zero;
    }    
    public void EnableEmoji(TypeEmoji typeEmoji)
    {
        var cash = uiClient.transform.GetChild(1).GetChild(0);
        var smile = uiClient.transform.GetChild(1).GetChild(1);
        switch (typeEmoji)
        {
            case TypeEmoji.Cash:
                cash.gameObject.SetActive(true);
                smile.gameObject.SetActive(false);
                cash.transform.localScale = Vector3.zero;
                cash.transform.DOScale(Vector3.one, 0.5f);
                break;
            case TypeEmoji.Smile:
                cash.gameObject.SetActive(false);
                smile.gameObject.SetActive(true);

                smile.transform.localScale = Vector3.zero;
                smile.transform.DOScale(Vector3.one,1).OnComplete(()=> 
                {
                    smile.gameObject.SetActive(false);
                });
                break;
            case TypeEmoji.None:
                cash.gameObject.SetActive(false);
                smile.gameObject.SetActive(false);
                break;
        }    
    }    
    public void DestroySelf()
    {
        GameObject.Destroy(uiClient.gameObject);
        GameObject.Destroy(gameObject);
    }    
    
}
