using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shooter : MonoBehaviour
{
    // 현재 버블
    [SerializeField]
    private GameObject currBubble;

    // 대기중인 버블
    [SerializeField]
    private GameObject readyBubble;

    Vector3 lineVector = Vector3.zero;
    Vector3 lineVector2 = Vector3.zero;
    Vector3 lineDir = Vector3.zero;

    //////////// Test var
    [SerializeField]
    private Vector3 test_Position; 
    //////////////

    private void Start()
    {
        EventManager.Instance.OnSetCurveCount(0);
        test_Position = new Vector3(GameManager.Instance.TouchArea.width - 50, transform.position.y, 0);
    }

    private void Update()
    {
        Aim();
        Fire();
        ReLoad();
    }

    // 조준
    private void Aim()
    {
        if(Input.GetMouseButton(0))
        {
            // 게임 진행상태가 AIM이 아니면 예외처리
            if(GameManager.Instance.gameState != GameManager.EnumGameState.AIM)
            {
                return;
            }

            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // 마우스가 터치영역 밖이면 예외처리
            if(!GameManager.Instance.IsMouseInTouchArea(mousePosition))
            {
                return;
            }

            // 마우스 포인터까지 방향벡터
            lineDir = (mousePosition - transform.position).normalized;

            // 터치영역과 발사 기준선 캐싱
            Rect TouchArea = GameManager.Instance.TouchArea;
            Rect baseLine = GameManager.Instance.BaseLine;

            // y값이 고정일 때 x값의 변화량
            float x = (baseLine.height - transform.position.y) * lineDir.x / lineDir.y;

            float bubbleRadius = currBubble.GetComponent<AllyBubble>().data.CalWidth / 2;

            float touchAreaX = mousePosition.x < transform.position.x ? TouchArea.x + bubbleRadius : TouchArea.width - bubbleRadius; 
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
        }
    }

    // 발사
    private void Fire()
    {
        if(Input.GetMouseButtonUp(0))
        {
            if(GameManager.Instance.gameState != GameManager.EnumGameState.AIM)
            {
                return;
            }

            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;

            // 라인렌더러 그리지 않기
            EventManager.Instance.OnSetCurveCount(0);

            if (!GameManager.Instance.IsMouseInTouchArea(mousePosition))
            {
                GameManager.Instance.gameState = GameManager.EnumGameState.AIM;
                return;
            }

            // 버블 상태를 Fire로 변경 및 방향벡터 전달
            currBubble.GetComponent<AllyBubble>().ChangeStateToFire(lineDir);
            currBubble = null;

            GameManager.Instance.gameState = GameManager.EnumGameState.REMOVE;
        }
    }

    // 버블 장전
    private void ReLoad()
    {
        if(GameManager.Instance.gameState != GameManager.EnumGameState.RELOAD)
        {
            return;
        }

        if (currBubble != null)
        {
            return;
        }

        if (readyBubble == null)
        {
            TakeBubble(ref readyBubble);
        }

        currBubble = readyBubble;
        currBubble.transform.position = transform.position;
        TakeBubble(ref readyBubble);

        GameManager.Instance.gameState = GameManager.EnumGameState.AIM;
    }

    // 버블 가져오기
    private void TakeBubble(ref GameObject bubble)
    {
        // 남은 방울 중 가장 하단 4칸의 방울 중 랜덤 생성
        List<string> bubblelist = new List<string>();

        // 맵 제작 이후 남은 방울 탐색 후 queue에 저장하는 식으로 변경
        bubblelist = EventManager.Instance.OnGetButtomLineList();

        // 랜덤 시스템
        int bubbleCount = bubblelist.Count;

        int randNum = UnityEngine.Random.Range(0, bubbleCount);
        string name = bubblelist[randNum];
        bubble = EventManager.Instance.OnGetBubble(name, test_Position);
    }

    // 버블 바꾸기
    public void SwapBubble()
    {
        var tempBubble = currBubble;

        currBubble = readyBubble;
        readyBubble = tempBubble;

        currBubble.transform.position = transform.position;
        readyBubble.transform.position = test_Position;
    }

    public void SwapBubble(string name)
    {
        var item = EventManager.Instance.OnGetBubble(name, transform.position);
        EventManager.Instance.OnGiveBubble(currBubble);
        currBubble = item;
    }

    // test code
    //private void TestCode()
    //{
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
    //}
}
