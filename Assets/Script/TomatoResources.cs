using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TomatoResources : BaseInteractionPlayer
{
    public List<Transform> allPointSpawn;
    public List<GameObject> allTomato;
    System.Action actionUpdate = null;
    public float timeSpawn;
    public int amountCurr = 0;
    TypeStatus status = TypeStatus.Wait;
    public int amountMax
    {
        get
        {
            return allPointSpawn.Count;
        }
    }
    public bool IsEmpty
    {
        get
        {
            return amountCurr == 0;
        }
    }
    private void Start()
    {
        StartSpawn();
        GameManager.Instance.gameAction.DelayAction(() =>
        {
            status = TypeStatus.Done;
        },1);
    }
    void StartSpawn()
    {
        float t = timeSpawn;
        actionUpdate += () =>
        {
            if (amountCurr >= amountMax)
                return;
            t -= Time.deltaTime;
            if (t < 0)
            {
                Spawn();
                amountCurr++;
                t = timeSpawn;
            }
        };
    }
    public void Spawn()
    {
        var rsCarry = MyPreb.GetPreb("Tomato");
        rsCarry.transform.parent = allPointSpawn[this.amountCurr];
        rsCarry.transform.localPosition = Vector3.zero;
        rsCarry.transform.rotation = Quaternion.identity;
        rsCarry.transform.localScale = Vector3.zero;
        rsCarry.transform.DOScale(Vector3.one, 0.1f);
        allTomato.Add(rsCarry);
    }
    public GameObject Remove()
    {
        var obj = allTomato[this.amountCurr - 1];
        allTomato.Remove(obj);
        this.amountCurr--;
        return obj;
    }

    public override void DoUpdate()
    {
        if (status == TypeStatus.Wait)
            return;
        base.DoUpdate();
        actionUpdate?.Invoke();
    }

    protected override void Interaction(Person player)
    {
        var obj = this;
        float t = 0.1f;
        System.Action actionProcess = () =>
        {
            if (obj.IsEmpty || player.IsFull)
            {
                return;
            }
            t -= Time.deltaTime;
            if (t < 0)
            {
                if (!obj.IsEmpty)
                {
                    player.AddCarryRs(obj.Remove());
                    t = 0.1f;
                }
            }

        };
        actionUpdateInteraction += actionProcess;
    }
}
