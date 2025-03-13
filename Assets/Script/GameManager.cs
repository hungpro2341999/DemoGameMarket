using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : Singleton<GameManager>
{
    public Transform posStartClient;
    public Person player;
    public List<Market> allMarkets;
    public Casher casher;
    public int money;
    public Text txtMoney;
    System.Action actionUpdate = null;
    public GameAction gameAction = new GameAction();
    public bool IsEmptyMoney
    {
        get
        {
            return money == 0;
        }
    }
    private void Start()
    {
        StartGame();
    }
    public void AddMarket(Market market)
    {
        allMarkets.Add(market);
    }    
    public Market GetMarket()
    {
        if (IsFull())
            return null;
        foreach(var market in allMarkets)
        {
            if (market.IsFullPerson || market.status == TypeStatus.Wait)
                continue;
            return market;
        }
        return null;
    }    
    bool IsFull()
    {
        foreach(var market in allMarkets)
        {
            if (!market.IsFullPerson && market.status == TypeStatus.Done)
                return false;
        }
        return true;
    }    
    void StartGame()
    {
        gameAction.DelayAction(() =>
        {
            StartSpawnClient();
        }, 1.2f);
     
        StartUpdateUI();
    }
    void StartUpdateUI()
    {
        actionUpdate += () =>
        {
            txtMoney.text = money.ToString();
        };
    }    
    void StartSpawnClient()
    {
        for(int i=0;i<3;i++)
        {
            SpawnClient();
        }    
        float t = 0;
        actionUpdate += () =>
        {
            t -= Time.deltaTime;
            if(t<0)
            {
                if (IsFull())
                {
                    return;
                }
                t = 3.5f;
                Debug.Log("Spawn");
                SpawnClient();
            }    
        };
    }    
    private void Update()
    {
        actionUpdate?.Invoke();
        gameAction.RunAction();
    }
    void SpawnClient()
    {
        var client = MyPreb.GetPreb("Client");
        client.GetComponent<Client>().StartShopping();
        client.transform.position = posStartClient.transform.position;
    }    
    public void AddMoney(int money)
    {
        this.money += money;
    }    
}

public class GameAction
{
    public System.Action actionThread1;
    public void DelayAction(System.Action actionDelay,float t)
    {
        System.Action action = null;
        action = () =>
        {
            t -= Time.deltaTime;
            if(t<0)
            {
                actionDelay?.Invoke();
                actionThread1 -= action;
            }
        };
        actionThread1 += action;
    }
    public void RunAction()
    {
        actionThread1?.Invoke();
    }
}
