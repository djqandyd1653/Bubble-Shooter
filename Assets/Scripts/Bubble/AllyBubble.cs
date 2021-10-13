using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AllyBubble : Bubble
{
    [SerializeField]
    public AllyBubbleData data;

    [SerializeField]
    protected AllyBubbleData.BubbleState state;

    // 나아가야하는 방향벡터
    private Vector3 dir = Vector3.zero;
    private float destinationPositionX = 0;


    protected override void Start()
    {
        base.Start();
        state = AllyBubbleData.BubbleState.WAITING;
    }

    protected virtual void Update()
    {
        Move();
        CheckCollisionToWall();
    }

    // 구슬과 충돌 시 
    protected virtual void OnTriggerEnter(Collider collision)
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
                state = AllyBubbleData.BubbleState.CONTACT;
            }
        }
    }

    // 움직임
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

    // 버블이 터치영역 최대높이에 도달했는가
    protected virtual void CheackHighestPosition()
    {
        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            if (transform.position.y - data.CalHeight * 0.5f >= GameManager.Instance.TouchArea.height)
            {
                if (transform.position.y <= GameManager.Instance.TouchArea.y)
                {
                    return;
                }

                state = AllyBubbleData.BubbleState.WAITING;
                EventManager.Instance.OnGiveBubble(this.gameObject);
            }
        }
    }

    // 버블 제거
    protected virtual void RemoveBubble()
    {
        if (state == AllyBubbleData.BubbleState.CONTACT)
        {
            EventManager.Instance.OnRemoveBubble(this.gameObject);
        }
    }

    // 방향 전환 (전환 조건이 충족되면)
    private void ChangeDir()
    {
        Vector3 tempVector = new Vector3(-dir.x, dir.y, dir.z);
        dir = tempVector;
    }

    // 버블이 오브젝트 풀로 돌아갈때 값 초기화
    public void InitBubble()
    {
        row = 0;
        column = 0;
        isCheck = false;
    }

    // test
    public void ChangeStateToFire(Vector3 _dir)
    {
        state = AllyBubbleData.BubbleState.FIRE;
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
