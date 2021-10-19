using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    // 터치 영역
    public Rect TouchArea;

    // 조준 기준선
    public Rect BaseLine;

    // 가로 화면 양 끝에서부터 경계선까지 들어가는 구슬 수
    private readonly float verticalBaseLineBubbleCount = 1.5f;
    public float VerticalBaseLineBubbleCount { get { return verticalBaseLineBubbleCount; } }

    // 터치영역 아래부터 경계선까지 들어가는 구슬 수
    private readonly float horizontalBaseLineBubbleCount = 10f;
    public float HorizontalBaseLineBubbleCount { get { return horizontalBaseLineBubbleCount; } }

    private void Start()
    {
        InitTouchArea();
    }

    private void InitTouchArea()
    {
        Vector3 windowLeftDownPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 windowRightUpPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float calWidth = (windowRightUpPoint.x - windowLeftDownPoint.x) / 11;
        float calHeight = (windowRightUpPoint.y - windowLeftDownPoint.y - 660) / 14;

        TouchArea.x = windowLeftDownPoint.x;
        TouchArea.width = windowRightUpPoint.x;
        TouchArea.y = windowLeftDownPoint.y + 400;
        TouchArea.height = windowRightUpPoint.y - 310;

        BaseLine.x = TouchArea.x + calWidth * verticalBaseLineBubbleCount;
        BaseLine.width = TouchArea.width - calWidth * verticalBaseLineBubbleCount;
        BaseLine.y = TouchArea.y;
        BaseLine.height = TouchArea.y + calHeight * horizontalBaseLineBubbleCount;
    }

    public bool IsMouseInTouchArea(Vector3 position)
    {
        if (position.x >= TouchArea.x && position.x <= TouchArea.width)
        {
            if (position.y >= TouchArea.y && position.y <= TouchArea.height)
            {
                return true;
            }
        }
        return false;
    }
}
