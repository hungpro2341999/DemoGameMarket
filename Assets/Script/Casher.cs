using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Casher : BaseInteractionPlayer
{
    public Transform posWaitClient;
    public Transform posMoney;
    public Transform posBox;
    public Queue<Client> allClient = new Queue<Client>();
    public List<GameObject> allMoney = new List<GameObject>();
    System.Action actionUpdate = null;
    public bool IsMoney
    {
        get
        {
            return allMoney.Count != 0;
        }
    }
    public bool IsEmpty
    {
        get
        {
            return allClient.Count == 0;
        }
    }
    public void AddClient(Client client)
    {
        allClient.Enqueue(client);
    }
    public Client RemoveClient()
    {
        var client = allClient.Dequeue();
        return client;
    }
    public Client GetFirstClient()
    {
        return allClient.Peek();
    }
    protected override void Interaction(Person player)
    {
        if (!(player is Player))
            return;
        System.Action action = null;
        action = () =>
        {
            if(IsMoney)
            {
                AddMoneyToPlayer((Player)GameManager.Instance.player);
            }    

            if (IsEmpty)
                return;
            var client = GetFirstClient();
            if(client.status == TypeStatus.Done)
            {
                ProcessClient(client);
            }
        };
        actionUpdateInteraction += action;
    }
    public override void DoUpdate()
    {
        base.DoUpdate();
        actionUpdate?.Invoke();
    }
    void ProcessClient(Client client)
    {
        client.status = TypeStatus.Wait;
        System.Action actionPay = null;
        System.Action actionCloseBox = null; // Boxer
        System.Action actionBox = null; // Boxer
        System.Action actionMoveBoxToPlayer = null; // MoveBoxToClient;
        System.Action actionGoHome = null;
        float t = 0;
        float tDelay = 0.3f;
        int cRsBox = 0;
        var box = MyPreb.GetPreb("Box");
        box.transform.position = posBox.transform.position;
        box.transform.localScale = Vector3.zero;
        box.transform.DOScale(Vector3.one, 0.35f);
     
        actionBox = () =>
        {
            tDelay -= Time.deltaTime;
            if (tDelay > 0)
                return;
            t -= Time.deltaTime;
            if (t < 0)
            {
                if (!client.IsEmpty)
                {
                    var target = box.transform.GetChild(1).GetChild(0).transform.position;
                    target += new Vector3(cRsBox % 2,0,cRsBox / 2)*0.3f;
                    var rs = client.RemoveCarryRs();
                    rs.transform.localScale = Vector3.one;
                    rs.transform.DOMove(target, 0.2f).OnComplete(()=> 
                    {
                        rs.transform.parent = box.transform;
                      
                    });
                    cRsBox++;
                }
                else if(client.IsEmpty)
                {
                    actionCloseBox();
                    actionUpdate -= actionBox;
                    
                }
                t = 0.2f;
            }
        };
       
        actionCloseBox = () =>
        {
            float t1 = 0.5f;
            box.GetComponent<Animator>().Play("Close", 0, 0);
            System.Action action = null;
            action = () =>
            {
                t1 -= Time.deltaTime;
                if(t1<0)
                {
                    actionMoveBoxToPlayer();
                    actionUpdate -= action;
                }    
            };
            actionUpdate += action;
        };
        actionUpdate += actionBox;



        actionMoveBoxToPlayer = () =>
        {
            var target = client.posStartCarry.transform.position;
            box.transform.DOMove(client.posStartCarry.transform.position, 0.2f);
            box.transform.parent = client.posStartCarry.transform;
            System.Action action = null;
            float t = 0.3f;
            action = () =>
            {
                t -= Time.deltaTime;
                if(t<0)
                {
                    actionPay();
                    Debug.Log("Add");
                    actionUpdate -= action;
                    
                }
            };
            actionUpdate += action;

        };

        actionPay = () =>
        {
            int payMoney = client.amountMaxCarry*10;
            System.Action action = null; // action Pay Money
            System.Action action1 = null; // action Done Money
            float t = 0.25f;
            float t1 = 0.3f;
            action1 = () =>
            {
                t1 -= Time.deltaTime;
                if(t1<0)
                {
                    actionUpdate -= action1;
                    var client = RemoveClient();
                    SortPlayer();
                    actionGoHome();
                }    
            };

            action = () =>
            {
                t -= Time.deltaTime;
                if(t<0)
                {
                    int Column = 10;
                    int Row = 10;
                    for(int i=0;i<payMoney;i++)
                    {
                        int index = i;
                        System.Action actionMoveMoney = null;
                        float tMoney = UnityEngine.Random.Range(0f, 0.3f);
                        actionMoveMoney = () =>
                        {
                            tMoney -= Time.deltaTime;
                            if(tMoney<0)
                            {
                                var money = MyPreb.GetPreb("Money");
                                money.transform.parent = posMoney.transform;
                              
                                var pos1 = new Vector3(index % Column * 0.2f, 0, index / Column * 0.4f);
                                var target = posMoney.transform.position+pos1;
                                money.transform.position = client.posStartCarry.transform.position;
                                money.transform.DOJump(target,2,1,0.8f).OnComplete(()=> 
                                {
                                    allMoney.Add(money);
                                });
                          
                                actionUpdate -= actionMoveMoney;
                            }    
                        };
                        actionUpdate += actionMoveMoney;
                    }
                    actionUpdate -= action;
                }    
            };
            actionUpdate += action;
            actionUpdate += action1;
        };

        actionGoHome = () =>
        {
            client.EnableEmoji(TypeEmoji.Smile);
            client.PlayAnim(TypeAnim.CarryMove);
            System.Action action = null;
            float t = 3;
            action = () =>
            {
                t -= Time.deltaTime;
                if(t<0)
                {
                    client.DestroySelf();
                    actionUpdate -= action;
                    return;
                }    
              
                client.agent.destination = GameManager.Instance.posStartClient.transform.position;
            };
            actionUpdate += action;
        };
    }
    void SortPlayer()
    {
        int index = 0;
        foreach(var client in allClient)
        {
            client.MoveQueueCasher(index);
            index++;
        }
    }
    void AddMoneyToPlayer(Player player)
    {
        foreach(var money in allMoney)
        {
            System.Action action = null;
            float t = UnityEngine.Random.Range(0f, 0.2f);
            action = () =>
            {
                t -= Time.deltaTime;
                if(t<0)
                {
                    System.Action actionMove = null;
                    float t = 0;
                    Vector3 posStart = money.transform.position;
                    actionMove = () =>
                    {
                        t += Time.deltaTime * 2;

                        money.transform.position = Vector3.Lerp(posStart, player.transform.position+Vector3.up*1.2f, t);
                       
                        if(t>1)
                        {
                            GameManager.Instance.AddMoney(1);
                            actionUpdate -= actionMove;
                            money.gameObject.SetActive(false);
                        }    
                    };
                    actionUpdate += actionMove;
                    actionUpdate -= action;
                }    
            };
            actionUpdate += action;
        }
        allMoney.Clear();
        
    }    
    

}
