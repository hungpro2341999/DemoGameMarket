using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MyPreb
{
    static Dictionary<string, GameObject> allPreb = new Dictionary<string, GameObject>();
    public static GameObject GetPreb(string name)
    {
        GameObject entity = null;
        if (allPreb.ContainsKey(name))
        {
            entity = GameObject.Instantiate(allPreb[name]);
            entity.name = name;
            return entity;
        }
        var obj = Resources.Load<GameObject>(name);
        allPreb.Add(name, obj);
        entity = GameObject.Instantiate(allPreb[name]);
        entity.name = name;
        return entity;

    }
}
