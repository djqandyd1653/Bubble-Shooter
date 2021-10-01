using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bubblePrefabs = null;

    Dictionary<string, Queue<GameObject>> poolManager = new Dictionary<string, Queue<GameObject>>();

    void Start()
    {
        InitPoolManager();
        EventManager.Instance.getBubble += GiveBubble;
        EventManager.Instance.giveBubble += TakeBubble;
    }

    // 버블 풀 관리자 초기화
    private void InitPoolManager()
    {
        foreach (var bubblePrefab in bubblePrefabs)
        {
            string key = bubblePrefab.name;

            if (!poolManager.Equals(key))
            {
                poolManager.Add(key, new Queue<GameObject>());
            }

            CreateBubble(key, bubblePrefab, 10);
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
    private GameObject GiveBubble(string key, Vector3 position)
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
    private void TakeBubble(GameObject bubbleObject)
    {
        string key = bubbleObject.name;
        bubbleObject.transform.position = transform.position;
        bubbleObject.SetActive(false);
        poolManager[key].Enqueue(bubbleObject);
    }
}
