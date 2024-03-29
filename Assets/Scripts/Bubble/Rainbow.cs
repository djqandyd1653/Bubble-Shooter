﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rainbow : AllyBubble
{
    // 충돌한 버블
    private int colorNumber;

    protected override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        colorNumber = -1;
    }

    protected override void Update()
    {
        base.Update();
        CheackHighestPosition();
        RemoveBubble();
    }

    protected override void RemoveBubble()
    {
        if(state != AllyBubbleData.BubbleState.CONTACT)
        {
            return;
        }

        for (int i = 0; i < MapManager.Instance.VerticalBubbleCount; i++)
        {
            for(int j = 0; j < MapManager.Instance.HorizontalBubbleCount; j++)
            {
                MapManager.Instance.SearchBubble(i, j, colorNumber);
            }
        }

        state = AllyBubbleData.BubbleState.WAITING;
        MapManager.Instance.RemoveBubbles(this.gameObject, false);
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        // 터치영역 높이값과 비교해서 CONTACT상태로 변경
        if (state == AllyBubbleData.BubbleState.FIRE)
        {
            if (collision.gameObject.CompareTag("Bubble"))
            {
                if (transform.position.y <= InputManager.Instance.TouchArea.y)
                {
                    return;
                }

                state = AllyBubbleData.BubbleState.CONTACT;
                colorNumber = (int)collision.GetComponent<AllyBubble>().data.BubbleColor;
            }
        }
    }
}
