using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    protected Rigidbody rigid;

    [SerializeField]
    protected int row = 0;
    public int Row
    {
        get { return row; }
        set
        {
            if (row == 0)
            {
                row = value;
            }
            else if (value == 0)
            {
                row = 0;
            }
        }
    }

    [SerializeField]
    protected int column = 0;
    public int Column
    {
        get { return column; }
        set
        {
            if(column == 0)
            {
                column = value;
            }
            else if(value == 0)
            {
                column = 0;
            }
        }
    }

    [SerializeField]
    protected bool isCheck = false;
    public bool IsCheck { get { return isCheck; } set { isCheck = value; } }

    protected virtual void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }
}