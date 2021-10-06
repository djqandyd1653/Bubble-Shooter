using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ally Bubble Data", menuName = "Create Ally Bubble Data")]
public class AllyBubbleData : BubbleData
{
    public enum BubbleState
    {
        WAITING,            // 대기
        FIRE,               // 발사
        CONTACT,            // 접촉
        REMOVE,             // 제거
        DROP                // 낙하
    }
    
    [SerializeField]
    private float speed;
    public float Speed { get { return speed; } }
}
