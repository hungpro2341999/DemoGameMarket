using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Market : BaseInteractionPlayer
{
    public List<GameObject> allRs = new List<GameObject>();
    public List<GameObject> allRsWaint = new List<GameObject>();
    int Column = 3;
    int Row = 3;
    public TypeStatus status = TypeStatus.Wait;
    public Transform posStart;
    public int Count
    {
        get
        {
            return allRs.Count;
        }
    }
    public bool IsFullPerson
    {
        get
        {
            return allClientWait.Count >= posWait.Count;
        }
    }
    public bool IsFull
    {
        get
        {
            return allRs.Count >= (Column * Row);
        }
    }
    public bool IsEmpty
    {
        get
        {
            return allRs.Count==0;
        }
    }
    public Transform GetPosTarget
    {
        get
        {
            foreach(var pos in posWait)
            {
                if (!posWaitSave.Contains(pos))
                    return pos;
            }
            return null;
        }
    }
    public List<Client> allClientWait = new List<Client>();
    public List<Transform> posWait = new List<Transform>();
    public List<Transform> posWaitSave = new List<Transform>();
    System.Action actionUpdate = null;
    private void Start()
    {
        GameManager.Instance.AddMarket(this);
        GameManager.Instance.gameAction.DelayAction(() =>
        {
            status = TypeStatus.Done;
        }, 1);
    }
    public void RemovePerson(Client client)
    {
        allClientWait.Remove(client);
    }
    public void AddPerson(Client client)
    {
        allClientWait.Add(client);
    }
    public void AddSlot(Transform trans)
    {
        posWaitSave.Add(trans);
    }
    public void RemoveSlot(Transform trans)
    {
        posWaitSave.Remove(trans);
    }
    public void AddRs(GameObject rs)
    {
        rs.transform.parent = posStart;
        Vector2 posMatrix = new Vector2(Count%Column,Count/Column);
        Vector3 posTarget = 
            new Vector3(-posMatrix.x, -posMatrix.y*0.5f, 0);
        allRsWaint.Add(rs);
        allRs.Add(rs);
        rs.transform.DOLocalJump(posTarget,1,1,0.25f);
    }


    protected override void Interaction(Person person)
    {
        if(person is Player)
        {
            var player = (Player)person;
            System.Action action = null;
            float t = 0.1f;
            action = () =>
            {
                if (IsFull || player.IsEmpty)
                    return;
                t -= Time.deltaTime;
                if(t<0)
                {
                    var rs = player.RemoveCarryRs();
                    AddRs(rs);
                    t = 0.1f;
                }
            };
            actionUpdateInteraction += action;
        }
        if(person is Client)
        {
            float t = 0;
            var client = (Client)person;
            System.Action action = null;
            action = () =>
            {
                if (client.status == TypeStatus.Wait)
                    return;
                if (IsEmpty || client.IsFull)
                    return;
                t -= Time.deltaTime;
                if (t < 0)
                {
                    client.AddCarryRs(RemoveRs());
                    t = 0.1f;
                    if(client.IsFull)
                    {
                        actionUpdate += action;
                    }    
                }
            };
            actionUpdate += action;
        }    
        

    }
   
    public override void DoUpdate()
    {
        if (status == TypeStatus.Wait)
            return;
        base.DoUpdate();
        actionUpdate?.Invoke();
    }
    public GameObject RemoveRs()
    {
        var rs = allRs[allRs.Count - 1];
        allRs.Remove(rs);
        return rs;
    }    
    bool IsRsDoneMove()
    {
        return allRsWaint.Count == 0;
    }    
}
