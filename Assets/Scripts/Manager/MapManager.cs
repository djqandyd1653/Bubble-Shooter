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

    private int num = -1;

    // 탐색 큐
    Queue<GameObject> searchQueue = new Queue<GameObject>();
    // 버블 제거 큐
    Queue<GameObject> removeQueue = new Queue<GameObject>();
    // 탐색 스택
    Stack<GameObject> searchStack = new Stack<GameObject>();
    // 버블 낙하 스택
    Stack<GameObject> dropStack = new Stack<GameObject>();

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
                CalculateEvenRow(currentSearchBubble, true);
            }
            else
            {
                CalculateOddRow(currentSearchBubble, true);
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

        while (removeQueue.Count != 0)
        {
            var currentRemoveBubble = removeQueue.Dequeue();
            DropBubbles(currentRemoveBubble);
            AllyBubble temp = currentRemoveBubble.GetComponent<AllyBubble>();
            mapArray[temp.Row, temp.Column] = null;
            temp.ChangeStateToWaiting();
            temp.InitBubble();
            EventManager.Instance.OnGiveBubble(currentRemoveBubble);
        }

        for(int i = 0; i < GameManager.Instance.VerticalBubbleCount; i++)
        {
            for (int j = 0; j < GameManager.Instance.HorizontalBubbleCount; j++)
            {
                if(mapArray[i, j] == null)
                {
                    continue;
                }

                mapArray[i, j].GetComponent<Bubble>().IsCheck = false;
            }
        }
    }

    private void CalculateEvenRow(GameObject _bubble, bool isSearchRemoveBubble)
    {
        AllyBubble bubble = _bubble.GetComponent<AllyBubble>();
        int row = bubble.Row;
        int column = bubble.Column;
        int colorNum = (int)bubble.data.BubbleColor;

        // row + 1, column - 1 검사
        if (row + 1 < maxRow && column - 1 >= 0)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row + 1, column - 1, colorNum);
            }
            else
            {
                SearchBubble(row + 1, column - 1);            }
        }

        // row + 1, column - 0 검사
        if (row + 1 < maxRow)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row + 1, column, colorNum);
            }
            else
            {
                SearchBubble(row + 1, column);            }
        }

        // row - 0, column - 1 검사
        if (column - 1 >= 0)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row, column - 1, colorNum);
            }
            else
            {
                SearchBubble(row, column - 1);            }
        }

        // row - 0, column + 1 검사
        if (column + 1 < maxColumn)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row, column + 1, colorNum);
            }
            else
            {
                SearchBubble(row, column + 1);            }
        }

        // row - 1, column - 1 검사
        if (row - 1 >= 0 && column - 1 >= 0)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row - 1, column - 1, colorNum);
            }
            else
            {
                SearchBubble(row - 1, column - 1);
            }
        }

        // row - 1, column - 0 검사
        if (row - 1 >= 0)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row - 1, column, colorNum);
            }
            else
            {
                SearchBubble(row - 1, column);
            }
        }
    }

    private void CalculateOddRow(GameObject _bubble, bool isSearchRemoveBubble)
    {
        AllyBubble bubble = _bubble.GetComponent<AllyBubble>();
        int row = bubble.Row;
        int column = bubble.Column;

        int colorNum = (int)bubble.data.BubbleColor;

        // row + 1, column - 0 검사
        if (row + 1 < maxRow)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row + 1, column, colorNum);
            }
            else
            {
                SearchBubble(row + 1, column);
            }
        }

        // row + 1, column + 1 검사
        if (row + 1 < maxRow && column + 1 < maxColumn)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row + 1, column + 1, colorNum);
            }
            else
            {
                SearchBubble(row + 1, column + 1);            }
        }

        // row - 0, column - 1 검사
        if (column - 1 >= 0)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row, column - 1, colorNum);
            }
            else
            {
                SearchBubble(row, column - 1);
            }
        }

        // row - 0, column + 1 검사
        if (column + 1 < maxColumn)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row, column + 1, colorNum);
            }
            else
            {
                SearchBubble(row, column + 1);
            }
        }

        // row - 1, column - 0 검사
        if (row - 1 >= 0)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row - 1, column, colorNum);
            }
            else
            {
                SearchBubble(row - 1, column);
            }
        }

        // row - 1, column + 1 검사
        if (row - 1 >= 0 && column + 1 < maxColumn)
        {
            if (isSearchRemoveBubble)
            {
                SearchBubble(row - 1, column + 1, colorNum);
            }
            else
            {
                SearchBubble(row - 1, column + 1);
            }
        }
    }

    // 같은 색 구슬 탐색
    private void SearchBubble(int row, int column, int colorNum)
    {
        if(mapArray[row, column] == null)
        {
            return;
        }

        AllyBubble checkBubble = mapArray[row, column].GetComponent<AllyBubble>();
        int _colorNum = (int)checkBubble.data.BubbleColor;

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

    private void SearchBubble(int row, int column)
    {
        if (mapArray[row, column] == null)
        {
            return;
        }

        AllyBubble checkBubble = mapArray[row, column].GetComponent<AllyBubble>();

        // 체크한적 없으면
        if (!checkBubble.IsCheck)
        {   
            if (row == 0)
            {
                while(num >= 0 && searchStack.Count > num)
                {
                    searchStack.Pop();
                }

                if(num > 0)
                {
                    num--;
                }

                while (dropStack.Count != 0)
                {
                    dropStack.Pop().GetComponent<Bubble>().IsCheck = false;
                }

                return;
            }

            searchStack.Push(checkBubble.gameObject);
        }
    }

    private void DropBubbles(GameObject bubble)
    {
        searchStack.Clear();
        dropStack.Clear();

        // Remove Bubble
        searchStack.Push(bubble);
        GameObject currentSearchBubble = null;

        num = -1;

        // 탐색 및 dropStack에 저장
        while (searchStack.Count != 0)
        {
            if(currentSearchBubble == null)
            {
                currentSearchBubble = bubble;
                searchStack.Pop();
            }
            else
            {
                if(num == -1)
                {
                    num = searchStack.Count;
                }

                while(searchStack.Count != 0 && searchStack.Peek().GetComponent<Bubble>().IsCheck)
                {
                    searchStack.Pop();
                }

                if(searchStack.Count == 0)
                {
                    break;
                }

                currentSearchBubble = searchStack.Pop();
                currentSearchBubble.GetComponent<Bubble>().IsCheck = true;
                dropStack.Push(currentSearchBubble);
            }
            
            // 주변 버블 탐색
            if(currentSearchBubble.GetComponent<Bubble>().Row % 2 == 0)
            {
                CalculateEvenRow(currentSearchBubble, false);
            }
            else
            {
                CalculateOddRow(currentSearchBubble, false);
            }
        }

        if(dropStack.Count == 0)
        {
            return;
        }

        while(dropStack.Count != 0)
        {
            var currentDropBubble = dropStack.Pop().GetComponent<AllyBubble>();
            mapArray[currentDropBubble.Row, currentDropBubble.Column] = null;
            currentDropBubble.ChangeStateToDrop();
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
