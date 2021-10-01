using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSetter : MonoBehaviour
{
    LineRenderer lr;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startColor = Color.yellow; 
        lr.startWidth = 8;
        lr.endWidth = 8;

        EventManager.Instance.setLinePosition2 += SetLinePosition;
        EventManager.Instance.setLinePosition3 += SetLinePosition;
        EventManager.Instance.setCurveCount += SetCurveCnt;
    }

    private void SetCurveCnt(int curveCnt = 1)
    {
        lr.positionCount = curveCnt;
    }

    private void SetLinePosition(Vector3 position1, Vector3 position2)
    {
        lr.positionCount = 2;
        lr.SetPosition(0, position1);
        lr.SetPosition(1, position2);
    }

    private void SetLinePosition(Vector3 position1, Vector3 position2, Vector3 position3)
    {
        lr.positionCount = 3;
        lr.SetPosition(0, position1);
        lr.SetPosition(1, position2);
        lr.SetPosition(2, position3);
    }
}
