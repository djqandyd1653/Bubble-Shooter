using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    private GameObject[,] map = null;
    private int maxRow = 0;
    private int maxColumn = 0;

    // 구슬 프리팹들 저장
    [SerializeField]
    private GameObject[] bubbles = null;
    public GameObject[] Bubbles { get { return bubbles; } }

    // 구슬들 이름저장 배열
    private string[] bubbleNames = null;
    public string[] BubbleNames { get { return bubbleNames; } set { bubbleNames = value; } }

    // 선택된 구슬 번호
    private int selectedBubbleNum = 0;
    public int SelectedBubbleNum { get { return selectedBubbleNum; } set { selectedBubbleNum = value; } }

    // 맵 번호(1~4번)
    [SerializeField]
    [Range(1,4)]
    private int mapNumber = 0;
    public int MapNumber { get { return mapNumber; } }

    // 스테이지 번호
    [SerializeField]
    private int stageNumber = 0;
    public int StageNumber { get { return stageNumber; } }

    // 구슬사이의 높이 차
    private float heightDifferent = 0;
    // 구슬 사이의 너비 차
    private float widthDifferent = 0;
    // 구슬 너비 절반
    private float halfWidth = 0;
    // 구슬 높이 절반
    private float halfHeight = 0;
    // 0행 0열 인덱스의 x값과 y값
    private float touchAreaX = 0;
    private float touchAreaHeigh = 0;

    void Start()
    {
        maxRow = GameManager.Instance.VerticalBubbleCount;
        maxColumn = GameManager.Instance.HorizontalBubbleCount;

        map = new GameObject[maxRow, maxColumn];

        // test
        halfWidth = bubbles[0].GetComponent<AllyBubble>().data.CalWidth * 0.5f;
        halfHeight = bubbles[0].GetComponent<AllyBubble>().data.CalHeight * 0.5f;
        widthDifferent = halfWidth * 2;
        heightDifferent = Mathf.Sqrt(3) * halfHeight;
        touchAreaX = GameManager.Instance.TouchArea.x;
        touchAreaHeigh = GameManager.Instance.TouchArea.height;
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            SetBubble();
        }
    }

    // 버블을 마우스 위치로 세팅
    private void SetBubble()
    {
        // 마우스 위치 스크린 좌표에서 월드 좌표로 변환 및 z축 값 변경
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        int row = 0;
        int column = 0;

        SetMatrix(out row, out column, mousePosition);

        if (row < 0 || row >= maxRow || column < 0 || column >= maxColumn)
        {
            return;
        }

        if(map[row, column] != null)
        {
            return;
        }

        var bubblePosition = SetPosition(row, column);

        // 버블 생성
        var bubble = Instantiate(bubbles[selectedBubbleNum], bubblePosition, Quaternion.identity);
        bubble.name = bubbleNames[selectedBubbleNum];
        map[row, column] = bubble;
    }


    // 위치값으로 행렬 결정
    private void SetMatrix(out int row, out int column, Vector3 mousePosition)
    {
        row = (int)((touchAreaHeigh - mousePosition.y) / heightDifferent);

        if (row % 2 == 0)
        {
            column = (int)((mousePosition.x - touchAreaX) / widthDifferent);
        }
        else
        {
            column = (int)((mousePosition.x - touchAreaX - halfWidth) / widthDifferent);

            if (column == GameManager.Instance.HorizontalBubbleCount - 1)
            {
                column--;
            }
        }
    }

    // 행렬값으로 위치 반환
    private Vector3 SetPosition(int row, int column)
    {
        float x = touchAreaX + (1 + 2 * column) * halfWidth;
        float y = touchAreaHeigh - halfHeight - row * heightDifferent;

        if (row % 2 != 0)
        {
            x += halfWidth;
        }

        return new Vector3(x, y, 0);
    }

    public void SaveMap()
    {
        GetComponent<MapJson>().SaveMap(maxRow, maxColumn, stageNumber, mapNumber, map);
    }
}
