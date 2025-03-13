using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BaseInteractionPlayer : MonoBehaviour
{
    protected System.Action actionUpdateInteraction;
    protected virtual void Interaction(Person player)
    {
      
    }
    protected virtual void Uninteraction(Person player)
    {
        actionUpdateInteraction = null;
    }
    private void Update()
    {
        DoUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Interaction(other.GetComponent<Person>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            Uninteraction(other.GetComponent<Person>());
        }
    }
    public virtual void DoUpdate()
    {
        actionUpdateInteraction?.Invoke();
    }
}
