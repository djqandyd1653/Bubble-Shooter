using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private GameObject[,] mapArray;
    private int maxRow = 0;
    private int maxColumn = 0;
    [SerializeField]
    private float heightDifferent = 0;
    [SerializeField]
    private float widthDifferent = 0;
    [SerializeField]
    private float halfWidth = 0;
    [SerializeField]
    private float halfHeight = 0;

    private float firstIndexX = 0;
    private float firstIndexY = 0;

    // test
    [SerializeField]
    private BubbleData bubbleData = null;

    Queue<GameObject> searchQueue = new Queue<GameObject>();
    Queue<GameObject> removeQueue = new Queue<GameObject>();

    void Start()
    {
        mapArray = new GameObject[GameManager.Instance.VerticalBubbleCount, GameManager.Instance.HorizontalBubbleCount];
        EventManager.Instance.setBubblePosition += SetBubblePosition;
        EventManager.Instance.removeBubble += RemoveBubbles;

        // test
        halfWidth = bubbleData.CalWidth * 0.5f;
        halfHeight = bubbleData.CalHeight * 0.5f;
        widthDifferent = halfWidth * 2;
        heightDifferent = Mathf.Sqrt(3) * halfHeight;

        maxRow = GameManager.Instance.VerticalBubbleCount;
        maxColumn = GameManager.Instance.HorizontalBubbleCount;

        firstIndexX = GameManager.Instance.TouchArea.x;
        firstIndexY = GameManager.Instance.TouchArea.height;
        //PrintArray();
    }

    // 구슬 위치 설정
    private Vector3 SetBubblePosition(GameObject bubble)
    {
        Vector3 bubblePosition = bubble.transform.position;
        // 구슬 위치에 따른 행과 열 계산
        int row = (int)((firstIndexY - bubblePosition.y) / heightDifferent);
        int column;

        if (row % 2 == 0)
        {
            // 0이거나 짝수 행
            column = (int)((bubblePosition.x - firstIndexX) / widthDifferent);
        }
        else
        {
            // 홀수 행
            column = (int)((bubblePosition.x - firstIndexX - halfWidth) / widthDifferent);

            if(column == GameManager.Instance.HorizontalBubbleCount - 1)
            {
                column--;
            }
        }

        // 선택된 행에 색값 입력하기
        Bubble currentbubble = bubble.GetComponent<Bubble>();
        currentbubble.Row = row;
        currentbubble.Column = column;
        mapArray[row, column] = bubble;

        Debug.Log(row + ", " + column);

        return CalculateArrayPosition(row, column);
    }

    // 위쪽 2개 양 옆 2개 탐색
    //private bool SearchAroundIndex(int row, int column, Vector3 bubblePosition)
    //{
    //    if(row > 0 && column > 0 && mapArray[row - 1, column - 1] != 0)
    //    {
    //        return false;
    //    }

    //    if (column > 0 && mapArray[row, column - 1] != 0)
    //    {
    //        return false;
    //    }

    //    if (row > 0 && mapArray[row - 1, column] != 0)
    //    {
    //        return false;
    //    }

    //    if (column < (GameManager.Instance.HorizontalBubbleCount - 1) && mapArray[row, column + 1] != 0)
    //    {
    //        return false;
    //    }

    //    return true;
    //}

    private Vector3 CalculateArrayPosition(int row, int column, int z = 0)
    {
        float x = firstIndexX + (1 + 2 * column) * halfWidth;
        float y = firstIndexY - halfHeight - row * heightDifferent;

        if (row % 2 != 0)
        {
            x += halfWidth;
        }

        return new Vector3(x, y, z);
    }

    // 버블 제거
    private void RemoveBubbles(GameObject bubble)
    {
        // queue 초기화
        searchQueue.Clear();
        removeQueue.Clear();

        searchQueue.Enqueue(bubble);
        removeQueue.Enqueue(bubble);
        bubble.GetComponent<Bubble>().IsCheck = true;

        while(searchQueue.Count != 0)
        {
            GameObject currentSearchBubble = searchQueue.Dequeue();

            if (currentSearchBubble.GetComponent<Bubble>().Row % 2 == 0)
            {
                CalculateEvenRow(currentSearchBubble);
            }
            else
            {
                CalculateOddRow(currentSearchBubble);
            }
        }

        if(removeQueue.Count < 3)
        {
            while (removeQueue.Count != 0)
            {
                GameObject currentRemoveBubble = removeQueue.Dequeue();
                Bubble temp = currentRemoveBubble.GetComponent<Bubble>();
                temp.IsCheck = false;
            }

            removeQueue.Clear();
            searchQueue.Clear();
            bubble.GetComponent<AllyBubble>().ChangeStateToWaiting();
            return;
        }

        //int lastRow = 0;
        //int lastColumn = 0;

        while (removeQueue.Count != 0)
        {
            GameObject currentRemoveBubble = removeQueue.Dequeue();
            Bubble temp = currentRemoveBubble.GetComponent<Bubble>();

            //if(removeQueue.Count == 0)
            //{
            //    lastRow = temp.Row;
            //    lastColumn = temp.Column;
            //}

            mapArray[temp.Row, temp.Column] = null;
            EventManager.Instance.OnGiveBubble(currentRemoveBubble);
        }

        // 버블 제거된 후 구슬 떨어질 구슬 탐색
        //if (lastRow % 2 == 0)
        //{
        //    // row - 1, column - 1
        //    if(lastRow - 1 >= 0 && lastColumn - 1 >= 0)
        //    {
        //        if(mapArray[lastRow - 1, lastColumn - 1] == null)
        //        {
        //            return;
        //        }
        //    }

        //    // row, column - 1
        //    if(lastColumn - 1 >= 0)
        //    {

        //    }
        //}
        //else
        //{

        //}
    }

    private void CalculateEvenRow(GameObject _bubble)
    {
        AllyBubble bubble = _bubble.GetComponent<AllyBubble>();
        int row = bubble.Row;
        int column = bubble.Column;
        int colorNum = (int)bubble.data.BubbleColor;

        // row - 1, column - 1 검사
        if (row - 1 >= 0 && column - 1 >= 0)
        {
            SearchSameColorBubble(row - 1, column - 1, colorNum);
        }

        // row - 1, column - 0 검사
        if (row - 1 >= 0)
        {
            SearchSameColorBubble(row - 1, column, colorNum);
        }

        // row - 0, column - 1 검사
        if (column - 1 >= 0)
        {
            SearchSameColorBubble(row, column - 1, colorNum);
        }

        // row - 0, column + 1 검사
        if (column + 1 < maxColumn)
        {
            SearchSameColorBubble(row, column + 1, colorNum);
        }

        // row + 1, column - 1 검사
        if (row + 1 < maxRow)
        {
            SearchSameColorBubble(row + 1, column, colorNum);
        }

        // row + 1, column - 0 검사
        if (row + 1 < maxRow)
        {
            SearchSameColorBubble(row + 1, column, colorNum);
        }
    }

    private void CalculateOddRow(GameObject _bubble)
    {
        AllyBubble bubble = _bubble.GetComponent<AllyBubble>();
        int row = bubble.Row;
        int column = bubble.Column;
        int colorNum = (int)bubble.data.BubbleColor;

        // row - 1, column - 0 검사
        if (row - 1 >= 0)
        {
            SearchSameColorBubble(row - 1, column, colorNum);
        }

        // row - 1, column + 1 검사
        if (row - 1 >= 0 && column + 1 < maxColumn)
        {
            SearchSameColorBubble(row - 1, column + 1, colorNum);
        }

        // row - 0, column - 1 검사
        if (column - 1 >= 0)
        {
            SearchSameColorBubble(row, column - 1, colorNum);
        }

        // row - 0, column + 1 검사
        if (column + 1 < maxColumn)
        {
            SearchSameColorBubble(row, column + 1, colorNum);
        }

        // row + 1, column - 0 검사
        if (row + 1 < maxRow)
        {
            SearchSameColorBubble(row + 1, column, colorNum);
        }

        // row + 1, column + 1 검사
        if (row + 1 < maxRow && column + 1 < maxColumn)
        {
            SearchSameColorBubble(row + 1, column + 1, colorNum);
        }
    }

    // 같은 색 구슬 탐색
    private void SearchSameColorBubble(int row, int column, int colorNum)
    {
        if(mapArray[row, column] == null)
        {
            return;
        }

        AllyBubble checkBubble = mapArray[row, column].GetComponent<AllyBubble>();

        // 같은 색인지 비교함
        if ((int)checkBubble.data.BubbleColor == colorNum)
        {
            // 체크한적 없으면
            if (!checkBubble.IsCheck)
            {
                checkBubble.IsCheck = true;
                removeQueue.Enqueue(checkBubble.gameObject);
                searchQueue.Enqueue(checkBubble.gameObject);
            }
        }
    }

    // test Function
    private void PrintArray()
    {
        //for(int i = 0; i < GameManager.Instance.VerticalBubbleCount; i++)
        //{
            for(int j = 0; j < GameManager.Instance.HorizontalBubbleCount; j++)
            {
                Debug.Log("[0][" + j + "]: " + mapArray[0, j]);
            }
        //}
    }
}
