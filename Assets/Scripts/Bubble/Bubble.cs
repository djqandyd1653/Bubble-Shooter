using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    protected Rigidbody rigid;

    [SerializeField]
    protected int row = 0;
    public int Row { get { return row; } set { row = value; } }

    [SerializeField]
    protected int column = 0;
    public int Column { get { return column; } set { column = value; } }

    [SerializeField]
    protected bool isCheck = false;
    public bool IsCheck { get { return isCheck; } set { isCheck = value; } }

    protected virtual void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    public virtual void DropLine(float heightDifferent)
    {
        Debug.Log(1);
    }
}