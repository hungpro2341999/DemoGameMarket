using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathCreater : MonoBehaviour
{
    public List<Transform> paths;
    private void Start()
    {
        paths = transform.GetComponents<Transform>().ToList();
    }


  
}
