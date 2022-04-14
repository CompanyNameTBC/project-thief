using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    private void Awake()
    {
        transform.SetParent(null);
    }
}
