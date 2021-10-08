using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private GameObject[,] mapArray = null;
    private int maxRow = 0;
    private int maxColumn = 0;

    private void Init()
    {
        maxRow = GameManager.Instance.VerticalBubbleCount;
        maxColumn = GameManager.Instance.HorizontalBubbleCount;

        mapArray = new GameObject[maxRow, maxColumn];
    }
}
