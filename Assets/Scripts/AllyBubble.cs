using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AllyBubble : Bubble
{
    [SerializeField]
    public AllyBubbleData data;

    [SerializeField]
    private AllyBubbleData.BubbleState state;

    // 나아가야하는 방향벡터
    private Vector3 dir = Vector3.zero;
    private float destinationPositionX = 0;

    //private event Action bubbleStateFunction;

    protected override void Start()
    {
        base.Start();
        state = AllyBubbleData.BubbleState.WAITING;
    }

    void Update()
    {
        //bubbleStateFunction?.Invoke();
        Move();
        CheckCollisionToWall();

        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            if (transform.position.y + data.CalHeight * 0.5f >= GameManager.Instance.TouchArea.height)
            {
                if (transform.position.y <= GameManager.Instance.TouchArea.y)
                {
                    return;
                }

                transform.position = EventManager.Instance.OnSetBubblePosition(this.gameObject);
                //ChangeState(AllyBubbleData.BubbleState.CONTACT);
                state = AllyBubbleData.BubbleState.CONTACT;
            }
        }

        if(state == AllyBubbleData.BubbleState.CONTACT)
        {
            EventManager.Instance.OnRemoveBubble(this.gameObject);
        }

        if(state == AllyBubbleData.BubbleState.DROP)
        {
            rigid.MovePosition(transform.position + Vector3.down * data.DropSpeed * Time.smoothDeltaTime);

            if (transform.position.y < GameManager.Instance.TouchArea.y)
            {
                state = AllyBubbleData.BubbleState.WAITING;
                EventManager.Instance.OnGiveBubble(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // 터치영역 높이값과 비교해서 CONTACT상태로 변경
        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            if (collision.gameObject.CompareTag("Bubble"))
            {
                if (transform.position.y <= GameManager.Instance.TouchArea.y)
                {
                    return;
                }

                transform.position = EventManager.Instance.OnSetBubblePosition(this.gameObject);
                //ChangeState(AllyBubbleData.BubbleState.CONTACT);
                state = AllyBubbleData.BubbleState.CONTACT;
            }
        }
    }

    private void Move()
    {
        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            rigid.MovePosition(transform.position + dir * data.Speed * Time.smoothDeltaTime);
        }
    }

    // 벽과 충돌 체크
    private void CheckCollisionToWall()
    {
        if(state == AllyBubbleData.BubbleState.FIRE)
        {
            if(dir.x < 0)
            {
                destinationPositionX = GameManager.Instance.TouchArea.x + data.CalWidth / 2;

                if(transform.position.x <= destinationPositionX)
                {
                    ChangeDir();
                }

                return;
            }

            destinationPositionX = GameManager.Instance.TouchArea.width - data.CalWidth / 2;

            if (transform.position.x >= destinationPositionX)
            {
                ChangeDir();
            }
        }
    }

    // 방향 전환 (전환 조건이 충족되면)
    private void ChangeDir()
    {
        Vector3 tempVector = new Vector3(-dir.x, dir.y, dir.z);
        dir = tempVector;
    }

    private void RemoveBubble()
    {
        EventManager.Instance.OnRemoveBubble(this.gameObject);
    }


    // 버블이 오브젝트 풀로 돌아갈때 값 초기화
    public void InitBubble()
    {
        row = 0;
        column = 0;
        isCheck = false;
    }

    //private void ChangeState(AllyBubbleData.BubbleState newStateNumber)
    //{
    //    switch(newStateNumber)
    //    {
    //        case AllyBubbleData.BubbleState.CONTACT:
    //            bubbleStateFunction -= Move;
    //            bubbleStateFunction -= CheckCollisionToWall;
    //            bubbleStateFunction += RemoveBubble;
    //            break;
    //        case AllyBubbleData.BubbleState.FIRE:
    //            //bubbleStateFunction
    //            bubbleStateFunction += Move;
    //            bubbleStateFunction += CheckCollisionToWall;
    //            break;
    //        case AllyBubbleData.BubbleState.REMOVE:
    //            bubbleStateFunction -= RemoveBubble;
    //            bubbleStateFunction += InitBubble;
    //            break;
    //        case AllyBubbleData.BubbleState.WAITING:
    //            bubbleStateFunction -= InitBubble;
    //            break;
    //        default:
    //            Debug.LogError("Switch Case Index Error(AllyBubble.cs, Line: 130)");
    //            break;
    //    }
    //}

    // test
    
    public void ChangeStateToFire(Vector3 _dir)
    {
        state = AllyBubbleData.BubbleState.FIRE;
        //ChangeState(AllyBubbleData.BubbleState.FIRE);
        dir = _dir;
    }

    public void ChangeStateToWaiting()
    {
        state = AllyBubbleData.BubbleState.WAITING;
    }

    public void ChangeStateToDrop()
    {
        state = AllyBubbleData.BubbleState.DROP;
        InitBubble();
    }

    public override void DropLine(float heightDifferent)
    {
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y - heightDifferent,
                                         0);
    }
}
