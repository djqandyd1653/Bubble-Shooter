using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBubble : Bubble
{
    [SerializeField]
    public AllyBubbleData data;

    [SerializeField]
    private AllyBubbleData.BubbleState state;

    // 나아가야하는 방향벡터
    private Vector3 dir = Vector3.zero;
    private float destinationPositionX = 0;

    protected override void Start()
    {
        base.Start();
        state = AllyBubbleData.BubbleState.WAITING;
    }

    void Update()
    {
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

                transform.position = EventManager.Instance.OnSetBubblePosition(transform.position);
                state = AllyBubbleData.BubbleState.CONTACT;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

                transform.position = EventManager.Instance.OnSetBubblePosition(transform.position);
                state = AllyBubbleData.BubbleState.CONTACT;
            }
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{

    //}

    private void Move()
    {
        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            //transform.position += dir * data.Speed * Time.smoothDeltaTime;
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

    // test
    public void ChangeStateToFire(Vector3 _dir)
    {
        state = AllyBubbleData.BubbleState.FIRE;
        dir = _dir;
    }
}
