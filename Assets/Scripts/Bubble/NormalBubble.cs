using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBubble : AllyBubble
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        CheackHighestPosition();
        RemoveBubble();
        Drop();
    }

    protected override void CheackHighestPosition()
    {
        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            if (transform.position.y + data.CalHeight * 0.5f >= GameManager.Instance.TouchArea.height)
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

    // 버블 떨어짐
    private void Drop()
    {
        if (state == AllyBubbleData.BubbleState.DROP)
        {
            rigid.MovePosition(transform.position + Vector3.down * data.DropSpeed * Time.smoothDeltaTime);

            if (transform.position.y < GameManager.Instance.TouchArea.y)
            {
                state = AllyBubbleData.BubbleState.WAITING;
                EventManager.Instance.OnGiveBubble(this.gameObject);
            }
        }
    }

    //public int GetCount(int colorIndex)
    //{
    //    if (myColor != colorIndex)
    //        return 0;

    //    List<NormalBubble> bubbleList = new List<NormalBubble>();

    //    int ret= 1;
    //    for (int i = 0; i < bubbleList.Count; i++)
    //    {
    //        ret += bubbleList[i].GetCount();
    //    }
    //    return ret;
    //}
}
