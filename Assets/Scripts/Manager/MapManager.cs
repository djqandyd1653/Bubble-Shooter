using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private List<GameObject[,]> mapList = new List<GameObject[,]>();

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

    [SerializeField]
    private BubbleData bubbleData = null;

    // 
    private int reaminFirstSearchCount = -1;

    // 맨위 0, 맨 밑 60 현재 라인 번호
    [SerializeField]
    private int currentLine = 0;

    [SerializeField]
    private int stageNumber = 0;
    [SerializeField]
    private int mapNumber = 1;

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
        InitMap();

        EventManager.Instance.setBubblePosition += SetBubblePosition;
        EventManager.Instance.removeBubble += RemoveBubbles;
        EventManager.Instance.getButtomLineList += GetButtomLineList;

        // test
        halfWidth = bubbleData.CalWidth * 0.5f;
        halfHeight = bubbleData.CalHeight * 0.5f;
        widthDifferent = halfWidth * 2;
        heightDifferent = Mathf.Sqrt(3) * halfHeight;
    }

    private void InitMap()
    {
        maxRow = GameManager.Instance.VerticalBubbleCount;
        maxColumn = GameManager.Instance.HorizontalBubbleCount;

        for(int i = 0; i < GameManager.Instance.OneStageMapCount; i++)
        {
            var currentMap = new GameObject[maxRow, maxColumn];
            GetComponent<MapJson>().LoadMap(maxRow, maxColumn, stageNumber, mapNumber + i, ref currentMap);
            mapList.Add(currentMap);
        }
    }

    // 구슬 위치 설정
    private Vector3 SetBubblePosition(GameObject bubble)
    {
        Vector3 bubblePosition = bubble.transform.position;

        float touchAreaX = GameManager.Instance.TouchArea.x;
        float touchAreaHeigh = GameManager.Instance.TouchArea.height;

        // 구슬 위치에 따른 행과 열 계산
        int row = (int)((touchAreaHeigh - bubblePosition.y) / heightDifferent);
        int column;

        // 한 줄 내려올 때
        if ((row + (currentLine % 2)) % 2 == 0)
        {
            // 0이거나 짝수 행
            column = (int)((bubblePosition.x - touchAreaX) / widthDifferent);
        }
        else
        {
            // 홀수 행
            column = (int)((bubblePosition.x - touchAreaX - halfWidth) / widthDifferent);

            if(column == GameManager.Instance.HorizontalBubbleCount - 1)
            {
                column--;
            }
        }

        // 선택된 행에 색값 입력하기
        Bubble currentbubble = bubble.GetComponent<Bubble>();
        currentbubble.Row = row;
        currentbubble.Column = column;
        mapList[0][row, column] = bubble;

        return CalculateArrayPosition(touchAreaX, touchAreaHeigh, row, column);
    }

    private Vector3 CalculateArrayPosition(float touchAreaX, float touchAreaHeigh, int row, int column, int z = 0)
    {
        float x = touchAreaX + (1 + 2 * column) * halfWidth;
        float y = touchAreaHeigh - halfHeight - row * heightDifferent;

        // 한 줄 내려올 떄
        if ((row + (currentLine % 2)) % 2 != 0)
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

            if ((currentSearchBubble.GetComponent<Bubble>().Row + (currentLine % 2)) % 2 == 0)
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
            DropLine();
            return;
        }

        while (removeQueue.Count != 0)
        {
            var currentRemoveBubble = removeQueue.Dequeue();
            DropBubbles(currentRemoveBubble);
            AllyBubble temp = currentRemoveBubble.GetComponent<AllyBubble>();
            mapList[0][temp.Row, temp.Column] = null;
            temp.ChangeStateToWaiting();
            temp.InitBubble();
            EventManager.Instance.OnGiveBubble(currentRemoveBubble);
        }

        for(int i = 0; i < GameManager.Instance.VerticalBubbleCount; i++)
        {
            for (int j = 0; j < GameManager.Instance.HorizontalBubbleCount; j++)
            {
                if(mapList[0][i, j] == null)
                {
                    continue;
                }

                mapList[0][i, j].GetComponent<Bubble>().IsCheck = false;
            }
        }

        DropLine();
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
                SearchBubble(row + 1, column - 1);
            }
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
                SearchBubble(row + 1, column);
            }
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
                SearchBubble(row + 1, column + 1);
            }
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
        if(mapList[0][row, column] == null)
        {
            return;
        }

        AllyBubble checkBubble = mapList[0][row, column].GetComponent<AllyBubble>();
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
        if (mapList[0][row, column] == null)
        {
            return;
        }

        AllyBubble checkBubble = mapList[0][row, column].GetComponent<AllyBubble>();

        // 체크한적 없으면
        if (!checkBubble.IsCheck)
        {   
            if (row == 0)
            {
                if (reaminFirstSearchCount > 0)
                {
                    reaminFirstSearchCount--;
                }

                while (reaminFirstSearchCount >= 0 && searchStack.Count > reaminFirstSearchCount)
                {
                    searchStack.Pop();
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

    // 끝과 연결되지 않은 버블 떨어뜨리기
    private void DropBubbles(GameObject bubble)
    {
        searchStack.Clear();
        dropStack.Clear();

        // Remove Bubble
        searchStack.Push(bubble);
        GameObject currentSearchBubble = null;

        reaminFirstSearchCount = -1;

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
                if(reaminFirstSearchCount == -1)
                {
                    reaminFirstSearchCount = searchStack.Count;
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
            if((currentSearchBubble.GetComponent<Bubble>().Row + (currentLine % 2)) % 2 == 0)
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
            mapList[0][currentDropBubble.Row, currentDropBubble.Column] = null;
            currentDropBubble.ChangeStateToDrop();
        }
    }

    // 버블 내리기
    private void DropLine()
    {
        bool isAllBubbleNull = true;

        // 구슬 한 칸씩 내리기
        for (int i = maxRow - 1; i >= 0; i--)
        {
            for (int j = 0; j < maxColumn; j++)
            {
                if (mapList[0][i,j] == null)
                {
                    continue;
                }

                if (i >= maxRow - 1)
                {
                    Debug.Log("Game Over");
                    return;
                }

                if(isAllBubbleNull)
                {
                    isAllBubbleNull = !isAllBubbleNull;
                }

                var currentBubble = mapList[0][i, j].GetComponent<Bubble>();
                currentBubble.DropLine(heightDifferent);
                currentBubble.Row = i + 1;
                mapList[0][i + 1, j] = mapList[0][i, j];
                mapList[0][i, j] = null;
            }
        }

        // 아무것도 없으면 8줄 가져오기
        if(isAllBubbleNull)
        {
            int noneBubbleDropCount = GameManager.Instance.NoneBubbleDropCount;

            if(currentLine >= noneBubbleDropCount)
            {
                for(int i = 0; i < noneBubbleDropCount; i++)
                {
                    DropOneLine(noneBubbleDropCount - i - 1);
                }
            }
            else
            {
                for (int i = 0; i < currentLine; i++)
                {
                    DropOneLine(i);
                }

                currentLine = 0;
            }

            GameManager.Instance.gameState = GameManager.EnumGameState.RELOAD;
            return;
        }

        // 다음 맵에서 1줄씩 가져오기
        DropOneLine();

        GameManager.Instance.gameState = GameManager.EnumGameState.RELOAD;
    }

    // 다음 맵에서 1줄씩 가져오기
    private void DropOneLine(int num = 0)
    {
        if (currentLine > 0)
        {
            currentLine--;

            int _maxColumn = currentLine % 2 == 0 ? maxColumn : maxColumn - 1;
            int _maxRow = currentLine % maxRow == 0 ? maxRow - 1 : currentLine % maxRow - 1;

            for (int i = 0; i < _maxColumn; i++)
            {
                int nextMapNumber = GameManager.Instance.OneStageMapCount - (currentLine - 1) / GameManager.Instance.VerticalBubbleCount - 1;

                if(mapList[nextMapNumber][_maxRow, i] == null)
                {
                    continue;
                }

                mapList[0][num, i] = mapList[nextMapNumber][_maxRow, i];
                mapList[nextMapNumber][_maxRow, i] = null;
                var currentBubble = mapList[0][num, i].GetComponent<AllyBubble>();
                currentBubble.Row = num;

                if (num == 0)
                {
                    currentBubble.DropLine((maxRow * nextMapNumber - _maxRow) * heightDifferent + halfHeight * nextMapNumber);
                }
                else
                {
                    currentBubble.DropLine(8 * nextMapNumber * heightDifferent + halfHeight * nextMapNumber);
                }
            }
        }
    }

    // 마지막에서 4번째까지 존재하는 버블 종류 서치
    private List<string> GetButtomLineList()
    {
        List<string> buttomLineBubbleName = new List<string>();

        int buttomRow = -1;

        for (int i = maxRow - 1; i >= 0; i--)
        {
            for (int j = 0; j < maxColumn; j++)
            {
                if (mapList[0][i, j] == null)
                {
                    continue;
                }

                // 마지막에서 4번째까지 존재하는 버블 종류 서치
                if (buttomRow == -1)
                {
                    buttomRow = i;
                }

                if (buttomRow != -1 && i > buttomRow - GameManager.Instance.SearchButtomRow)
                {
                    if (!buttomLineBubbleName.Contains(mapList[0][i, j].name))
                    {
                        buttomLineBubbleName.Add(mapList[0][i, j].name);
                    }
                }
            }
        }

        return buttomLineBubbleName;
    }
}
