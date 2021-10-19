using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoSingleton<BubbleManager>
{
    [SerializeField]
    private GameObject[] bubblePrefabs = null;

    Dictionary<string, Queue<GameObject>> poolManager = null;

    void Start()
    {
        InitBubbleSize();
        InitPoolManager();
    }

    private void InitBubbleSize()
    {
        Vector3 windowLeftDownPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 windowRightUpPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        float calWidth = (windowRightUpPoint.x - windowLeftDownPoint.x) / 11;
        float calHeight = (windowRightUpPoint.y - windowLeftDownPoint.y - 660) / 14;

        foreach (var bubble in bubblePrefabs)
        {
            bubble.transform.localScale = new Vector3(1, 1, 1);

            // 월드에 그려진 스프라이트 사이즈
            Vector3 size = bubble.GetComponent<SpriteRenderer>().bounds.size;

            bubble.transform.localScale = new Vector3(calWidth / size.x, calHeight / size.y, 1);

            // 버블의 크기 AllyBubbleData에 지정
            AllyBubbleData allyBubbleData = bubble.GetComponent<AllyBubble>().data;
            allyBubbleData.CalWidth = calWidth;
            allyBubbleData.CalHeight = calHeight;
        }
    }

    // 버블 풀 관리자 초기화
    private void InitPoolManager()
    {
        if(poolManager != null)
        {
            foreach(var queue in poolManager.Values)
            {
                queue.Clear();
            }

            poolManager.Clear();
        }

        poolManager = new Dictionary<string, Queue<GameObject>>();

        foreach (var bubblePrefab in bubblePrefabs)
        {
            string key = bubblePrefab.name;

            if (!poolManager.Equals(key))
            {
                poolManager.Add(key, new Queue<GameObject>());
            }

            CreateBubble(key, bubblePrefab, 5);
        }
    }

    // 버블 생성
    private void CreateBubble(string key, GameObject bubbleObject, int cnt = 1)
    {
        for(int i = 0; i < cnt; i++)
        {
            var bubble = Instantiate(bubbleObject, transform.position, Quaternion.identity, this.transform);
            bubble.name = bubbleObject.name;
            bubble.SetActive(false);
            poolManager[key].Enqueue(bubble);
        }
    }

    // 버블 풀에서 내보내기
    public GameObject GetBubble(string key, Vector3 position)
    {
        var bubble = poolManager[key].Dequeue();
        bubble.SetActive(true);
        bubble.transform.position = position;

        if (poolManager[key].Count == 0)
        {
            CreateBubble(key, bubble, 10);
        }

        return bubble;
    }

    // 버블 풀로 가져오기
    public void ReturnBubble(GameObject bubbleObject)
    {
        string key = bubbleObject.name;
        bubbleObject.transform.position = transform.position;
        bubbleObject.SetActive(false);
        poolManager[key].Enqueue(bubbleObject);
    }
}
