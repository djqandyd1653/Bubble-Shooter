using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private int[,] mapArray;
    [SerializeField]
    private float heightDifferent = 0;
    [SerializeField]
    private float widthDifferent = 0;

    private float firstIndexX = 0;
    private float firstIndexY = 0;

    // test
    [SerializeField]
    private AllyBubbleData test = null;

    void Start()
    {
        mapArray = new int[GameManager.Instance.VerticalBubbleCount, GameManager.Instance.HorizontalBubbleCount];
        EventManager.Instance.setBubblePosition += SetBubblePosition;

        // test
        widthDifferent = test.CalWidth;
        heightDifferent = Mathf.Sqrt(3) * widthDifferent * 0.5f;

        firstIndexX = GameManager.Instance.TouchArea.x;
        firstIndexY = GameManager.Instance.TouchArea.height;
        //PrintArray();
    }

    private Vector3 SetBubblePosition(Vector3 bubblePosition)
    {
        // 천장에 도달하거나 충돌한 인덱스가 0이 아닐경우

        int row = (int)((firstIndexY - bubblePosition.y) / heightDifferent);
        int column = (int)((bubblePosition.x - firstIndexX) / widthDifferent);

        //if (bubblePosition.y + test.CalHeight * 0.5f < firstIndexY &&
        //    SearchAroundIndex(row, column, bubblePosition))
        //{
        //    return bubblePosition;
        //}

        mapArray[row, column] += 1;

        return CalculateArrayPosition(row, column);
    }

    // 위쪽 2개 양 옆 2개 탐색
    private bool SearchAroundIndex(int row, int column, Vector3 bubblePosition)
    {
        if(row > 0 && column > 0 && mapArray[row - 1, column - 1] != 0)
        {
            return false;
        }

        if (column > 0 && mapArray[row, column - 1] != 0)
        {
            return false;
        }

        if (row > 0 && mapArray[row - 1, column] != 0)
        {
            return false;
        }

        if (column < (GameManager.Instance.HorizontalBubbleCount - 1) && mapArray[row, column + 1] != 0)
        {
            return false;
        }

        return true;
    }

    private Vector3 CalculateArrayPosition(int row, int column, int z = 0)
    {
        float x = firstIndexX + (1 + 2 * column) * widthDifferent *0.5f;
        float y = firstIndexY - widthDifferent * 0.5f - row * heightDifferent;

        if (row % 2 != 0)
        {
            x += widthDifferent * 0.5f;
        }

        return new Vector3(x, y, z);
    }

    private void CalculateEvenRow()
    {

    }

    private void CalculateOddRow()
    {

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
