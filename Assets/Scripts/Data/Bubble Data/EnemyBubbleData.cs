using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Bubble Data", menuName = "Create Enemy Bubble Data")]
public class EnemyBubbleData : BubbleData
{
    public enum BubbleState
    {
        CONTACT,            // 접촉
        REMOVE              // 제거
    }
}
