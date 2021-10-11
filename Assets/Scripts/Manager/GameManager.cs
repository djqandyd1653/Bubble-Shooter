using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    // 게임 상태
    public enum EnumGameState
    {
        AIM,
        REMOVE,
        DROPLINE,
        RELOAD
    }

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

    // 맵에 구슬이 없을 때 한번에 가져오는 열 수
    private const int noneBubbleDropCount = 8;
    public int NoneBubbleDropCount { get { return noneBubbleDropCount; } }

    // 구슬 생성 시 탐색할 열 수
    private const int searchButtomRow = 4;
    public int SearchButtomRow { get { return searchButtomRow; } }

    // 1 스테이지당 맵 갯수
    private const int oneStageMapCount = 4;
    public int OneStageMapCount { get { return oneStageMapCount; } }

    private float calWidth;
    private float calHeight;

    // 터치 영역
    public Rect TouchArea;

    // 조준 기준선
    public Rect BaseLine;

    // 게임 상태
    [SerializeField]
    public EnumGameState gameState;

    void Awake()
    {
        gameState = EnumGameState.RELOAD;

        Vector3 windowLeftDownPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 windowRightUpPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        calWidth = (windowRightUpPoint.x - windowLeftDownPoint.x) / 11;
        calHeight = (windowRightUpPoint.y - windowLeftDownPoint.y - 660) / 14;  

        InitTouchArea(windowLeftDownPoint.x, windowLeftDownPoint.y, windowRightUpPoint.x, windowRightUpPoint.y);
        InitBubbleSize();
    }

    // 터치 영역 초기화
    private void InitTouchArea(float x, float y, float width, float height)
    {
        TouchArea.x = x;
        TouchArea.width = width;
        TouchArea.y = y + 400;
        TouchArea.height = height - 310;

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

            // 버블의 크기 AllyBubbleData에 지정
            AllyBubbleData allyBubbleData = bubble.GetComponent<AllyBubble>().data;
            allyBubbleData.CalWidth = calWidth;
            allyBubbleData.CalHeight = calHeight;
        }
    }

    public bool IsMouseInTouchArea(Vector3 position)
    {
        if(position.x >= TouchArea.x && position.x <= TouchArea.width)
        {
            if(position.y >= TouchArea.y && position.y <= TouchArea.height)
            {
                return true;
            }
        }
        return false;
    }
}
