using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleData : ScriptableObject
{
    public enum EnumBubbleType
    {
        NORMAL,             // 기본
        INVISIBLE,          // 투명
        DISTURB,            // 방해
        ESCAPE,             // 탈출
        RANDOM,             // 랜덤
        INCREASE,           // 증식
        BASELINE,           // 기준선 증가
        FIVE_HORIZONTAL,    // 가로 5칸 제거
        SAME,               // 같은 구슬 제거
        TWO_RADIUS,         // 360도 2칸 제거
        ALL_HORIZONTAL      // 가로 한줄 제거
    }

    public enum EnumBubbleColor
    {
        RED,
        ORANGE,
        YELLOW,
        GREEN,
        BLUE,
        SKY,
        PURPLE,
        PINK,
        SPECIAL,   
    }

    [SerializeField]
    protected string bubbleName;
    public string BubbleName { get { return BubbleName; } }

    [SerializeField]
    protected EnumBubbleType type;
    public EnumBubbleType Type { get { return type; } }

    [SerializeField]
    protected float calWidth = 0;
    public float CalWidth { get { return calWidth; } set { calWidth = value; } }

    [SerializeField]
    protected float calHeight = 0;
    public float CalHeight { get { return calHeight; } set { calHeight = value; } }

    [SerializeField]
    protected EnumBubbleColor bubbleColor;
    public EnumBubbleColor BubbleColor { get { return bubbleColor; } }

    [SerializeField]
    protected float dropSpeed = 0;
    public float DropSpeed { get { return dropSpeed; } }
}
