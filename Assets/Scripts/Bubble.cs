using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    protected Rigidbody rigid;
    protected virtual void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }
}