using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleData : ScriptableObject
{
    public enum BubbleType
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

    [SerializeField]
    private BubbleType type;
    public BubbleType Type { get { return type; } }

    [SerializeField]
    private float calWidth = 0;
    public float CalWidth { get { return calWidth; } set { if(calWidth == 0) calWidth = value; } }

    [SerializeField]
    private float calHeight = 0;
    public float CalHeight { get { return calHeight; } set { if(calHeight == 0) calHeight = value; } }
}
