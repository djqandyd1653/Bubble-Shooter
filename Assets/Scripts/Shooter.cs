using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shooter : MonoBehaviour
{
    [SerializeField]
    private GameObject currBubble;

    [SerializeField]
    private GameObject readyBubble;

    Vector3 lineVector = Vector3.zero;
    Vector3 lineVector2 = Vector3.zero;
    Vector3 lineDir = Vector3.zero;

    //////////// Test var
    [SerializeField]
    private GameObject[] test_takeBubbleArray = null;

    //[SerializeField]
    //private int test_currCnt = 0;

    //[SerializeField]
    //private int test_maxCnt = 0;

    [SerializeField]
    private Vector3 test_Position = Vector3.zero;
    //////////////

    private void Start()
    {
        EventManager.Instance.OnSetCurveCount(0);
    }

    private void Update()
    {
        Aim();
        Fire();
        ReLoad();
        TestCode();
    }

    // 조준
    private void Aim()
    {
        if(Input.GetMouseButton(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // 마우스 포인터까지 방향벡터
            lineDir = (mousePosition - transform.position).normalized;

            // 터치영역과 발사 기준선 캐싱
            Rect TouchArea = GameManager.Instance.TouchArea;
            Rect baseLine = GameManager.Instance.BaseLine;

            // y값이 고정일 때 x값의 변화량
            float x = (baseLine.height - transform.position.y) * lineDir.x / lineDir.y;

            // 여기서 받아오는게 맞나? (test)
            float bubbleRadius = currBubble.GetComponent<AllyBubble>().data.CalWidth / 2;

            float touchAreaX = mousePosition.x < transform.position.x ? TouchArea.x + bubbleRadius : TouchArea.width - bubbleRadius; // 46.5f => 임시 구슬 가로 반지름
            float baseLineX = mousePosition.x < transform.position.x ? baseLine.x : baseLine.width;
            bool isMousePosXMinus = mousePosition.x < transform.position.x ? x >= touchAreaX : x <= touchAreaX;

            if (isMousePosXMinus)
            {
                lineVector = new Vector3(x, baseLine.height, 0);
                EventManager.Instance.OnSetLinePosition(transform.position, lineVector);
                return;
            }

            lineVector = new Vector3(touchAreaX, lineDir.y * touchAreaX / lineDir.x + transform.position.y, 0);
            lineVector2 = new Vector3(baseLineX, -lineDir.y / lineDir.x * (baseLineX - lineVector.x) + lineVector.y, 0);

            EventManager.Instance.OnSetLinePosition(transform.position, lineVector, lineVector2);
            return;

            //////////////////////////////////////////
            /*
            //슈터 기준으로 왼쪽
            if (mousePosition.x < transform.position.x)
            {
                // 기울기가 발사 기준선 왼쪽위 모서리보다 작을 때
                if (x >= TouchArea.x)
                {
                    lineVector = new Vector3(x, baseLine.height, 0);
                    EventManager.Instance.OnSetLinePosition(transform.position, lineVector);
                    return;
                }

                // 기울기가 발사 기준선 왼쪽위 모서리보다 클 때
                lineVector = new Vector3(TouchArea.x, lineDir.y * TouchArea.x / lineDir.x + transform.position.y, 0);
                lineVector2 = new Vector3(baseLine.x, -lineDir.y / lineDir.x * (baseLine.x - lineVector.x) + lineVector.y, 0);

                EventManager.Instance.OnSetLinePosition(transform.position, lineVector, lineVector2);
                return;
            }

            // 슈터 기준으로 오른쪽

            // 기울기가 발사 기준선 오른쪽 위 모서리보다 클 때
            if (x <= TouchArea.width)
            {
                lineVector = new Vector3(x, baseLine.height, 0);
                EventManager.Instance.OnSetLinePosition(transform.position, lineVector);
                return;
            }

            lineVector = new Vector3(TouchArea.width, lineDir.y * TouchArea.width / lineDir.x + transform.position.y, 0);
            lineVector2 = new Vector3(baseLine.width, -lineDir.y / lineDir.x * (baseLine.width - lineVector.x) + lineVector.y, 0);

            EventManager.Instance.OnSetLinePosition(transform.position, lineVector, lineVector2);
            return;
            */
        }
    }

    // 발사
    private void Fire()
    {
        if(Input.GetMouseButtonUp(0))
        {
            EventManager.Instance.OnSetCurveCount(0);
            currBubble.GetComponent<AllyBubble>().ChangeStateToFire(lineDir);
            currBubble = null;
        }
    }

    // 버블 장전
    private void ReLoad()
    {
        if(currBubble != null)
        {
            return;
        }

        if (readyBubble == null)
        {
            TakeBubble(ref readyBubble);
            // 버블 장전
            // 슈터 이전에 생성된 버블을 슈터로 가져옴
        }

        currBubble = readyBubble;
        currBubble.transform.position = transform.position;
        TakeBubble(ref readyBubble);
    }

    // 버블 가져오기
    private void TakeBubble(ref GameObject gameObject)
    {
        // 남은 방울 중 가장 하단 4칸의 방울 중 랜덤 생성 <= 탐색 후 큐에 담아서 하나씩 판별?
        List<GameObject> bubblelist = new List<GameObject>();

        // 맵 제작 이후 남은 방울 탐색 후 queue에 저장하는 식으로 변경
        SearchBubble();

        // queue에 저장
        foreach(var bubble in test_takeBubbleArray)
        {
            bubblelist.Add(bubble);
        }

        // 랜덤 시스템
        int bubbleCount = bubblelist.Count;
        string name = "";

        int randNum = UnityEngine.Random.Range(0, bubbleCount);
        name = bubblelist[randNum].name;
        gameObject = EventManager.Instance.OnGetBubble(name, test_Position);
    }

    // 남은 버블 중 가장 하단 4칸의 버블 찾기
    private void SearchBubble()
    {

    }

    // test code
    private void TestCode()
    {
        // 버블 풀에서 가져오기
        if (Input.GetKeyDown(KeyCode.P))
        {
            currBubble = EventManager.Instance.OnGetBubble("Red Bubble", transform.position);
        }

        // 버블 풀로 돌려보내기
        if (Input.GetKeyDown(KeyCode.O))
        {
            EventManager.Instance.OnGiveBubble(currBubble);
            currBubble = null;
        }

        // 랜덤시 분포 체크
        //if(currBubble != null)
        //{
        //    if(test_currCnt < test_maxCnt)
        //    {
        //        Debug.Log(currBubble.name);
        //        EventManager.Instance.OnGiveBubble(currBubble);
        //        currBubble = null;
        //        test_currCnt++;
        //    }
        //}
    }
}
