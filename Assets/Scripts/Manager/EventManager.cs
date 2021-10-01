﻿using System.Collections;
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

    // 러안 커브 갯수 설정하기
    public event Action<int> setCurveCount;

    public event Func<Vector3, Vector3> setBubblePosition;

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

    public Vector3 OnSetBubblePosition(Vector3 oldPosition)
    {
        return setBubblePosition.Invoke(oldPosition);
    }
}