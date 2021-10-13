using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoSingleton<EventManager>
{
    // 버블 주고받기
    public event Func<string, Vector3, GameObject> getBubble;
    public event Action<GameObject> giveBubble;

    // 라인 좌표 설정하기
    public event Action<Vector3, Vector3> setLinePosition2;
    public event Action<Vector3, Vector3, Vector3> setLinePosition3;

    // 라인 커브 갯수 설정하기
    public event Action<int> setCurveCount;

    // 버블 위치 설정
    public event Func<GameObject, Vector3> setBubblePosition;

    // 버블 제거
    public event Action<GameObject, bool> removeBubble;

    // 버블 탐색
    public event Action<int, int, int> searchBubble;

    // map 저장
    public event Action<int, int, int, int, GameObject[,]> saveMap;

    // 하단 버블 이름 리스트 전달
    public event Func<List<string>> getBottomLineList;

    // 버블 풀 포기화
    public event Action initBubblePool;

    // 맵 초기화
    public event Action initMap;

    public GameObject OnGetBubble(string key, Vector3 position)
    {
        return getBubble?.Invoke(key, position);
    }

    public void OnGiveBubble(GameObject bubbleObject)
    {
        giveBubble?.Invoke(bubbleObject);
    }

    public void OnSetLinePosition(Vector3 position1, Vector3 position2)
    {
        setLinePosition2?.Invoke(position1, position2);
    }

    public void OnSetLinePosition(Vector3 position1, Vector3 position2, Vector3 position3)
    {
        setLinePosition3?.Invoke(position1, position2, position3);
    }

    public void OnSetCurveCount(int curveCount)
    {
        setCurveCount?.Invoke(curveCount);
    }

    public Vector3 OnSetBubblePosition(GameObject bubble)
    {
        return setBubblePosition.Invoke(bubble);
    }

    public void OnRemoveBubble(GameObject bubble, bool isNormalBubble = true)
    {
        removeBubble?.Invoke(bubble, isNormalBubble);
    }

    public void OnSearchBubble(int row, int column, int colorNum)
    {
        searchBubble?.Invoke(row, column, colorNum);
    }

    public void OnSaveMap(int maxRow, int maxColumn, int stageNumber, int mapNumber, GameObject[,] map)
    {
        saveMap?.Invoke(maxRow, maxColumn, stageNumber, mapNumber, map);
    }

    public List<string> OnGetBottomLineList()
    {
        return getBottomLineList?.Invoke();
    }

    public void OnInitBubblePool()
    {
        initBubblePool?.Invoke();
    }

    public void OnInitMap()
    {
        initMap?.Invoke();
    }
}
