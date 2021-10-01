using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameObject[] bubbles = null;
     
    // 화면 가로에 들어가야할 버블 수
    private const int horizontalBubbleCount = 11;
    public int HorizontalBubbleCount {get{ return horizontalBubbleCount; }}

    // 화면 새로에 들어가야할 버블 수
    private const int verticalBubbleCount = 15;
    public int VerticalBubbleCount { get { return verticalBubbleCount; } }

    // 가로 화면 양 끝에서부터 경계선까지 들어가는 구슬 수
    private const float verticalBaseLineBubbleCount = 1.5f;
    public float VerticalBaseLineBubbleCount { get { return verticalBaseLineBubbleCount; } }

    // 터치영역 아래부터 경계선까지 들어가는 구슬 수
    private const float horizontaBaseLineBubbleCount = 10f;
    public float HorizontalBaseLineBubbleCount { get { return horizontaBaseLineBubbleCount; } }

    public Vector3 windowLeftDownPoint;
    public Vector3 windowRightUpPoint;
    float width;
    float height;
    float calWidth;
    float calHeight;

    // 터치 영역
    public Rect TouchArea;

    // 조준 기준선
    public Rect BaseLine;

    void Awake()
    {
        #region test(나중에 정리하기)
        windowLeftDownPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        windowRightUpPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        width = windowRightUpPoint.x - windowLeftDownPoint.x;
        height = windowRightUpPoint.y - windowLeftDownPoint.y;

        calWidth = width / 11;
        calHeight = height / 23;
        #endregion
        InitBubbleSize();
        InitTouchArea();
    }

    // 터치 영역 초기화
    private void InitTouchArea()
    {
        TouchArea.x = windowLeftDownPoint.x;
        TouchArea.width = windowRightUpPoint.x;
        TouchArea.y = windowLeftDownPoint.y + 400;
        TouchArea.height = windowRightUpPoint.y - 310;

        BaseLine.x = TouchArea.x + calWidth * verticalBaseLineBubbleCount;
        BaseLine.width = TouchArea.width - calWidth * verticalBaseLineBubbleCount;
        BaseLine.y = TouchArea.y;
        BaseLine.height = TouchArea.y + calHeight * horizontaBaseLineBubbleCount;
    }

    // 디바이스 해상도에 따른 버블 프리팹 사이즈 조정
    private void InitBubbleSize()
    {
        foreach (var bubble in bubbles)
        {
            bubble.transform.localScale = new Vector3(1, 1, 1);

            Vector3 size = bubble.GetComponent<SpriteRenderer>().bounds.size;

            bubble.transform.localScale = new Vector3(calWidth / size.x, calHeight / size.y, 1);
        }

        // 모든 버블의 크기가 같아서 임의의 버블의 크기로 지정
        bubbles[0].GetComponent<AllyBubble>().data.CalWidth = calWidth;
        bubbles[0].GetComponent<AllyBubble>().data.CalHeight = calHeight;
    }
}
