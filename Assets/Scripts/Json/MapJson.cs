﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MapJson : MonoBehaviour
{
    // Vector3 커스텀
    [Serializable]
    public struct CustomVector3
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public CustomVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
            w = 0;
        }

        public CustomVector3(Quaternion quaternion)
        {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
    }

    // List 커스텀
    [Serializable]
    public class CustomList<T>
    {
        [SerializeField]
        List<T> list;

        public List<T> ToList()
        {
            return list;
        }

        public CustomList(List<T> _list)
        {
            list = _list;
        }
    }

    [Serializable]
    public struct BubbleJsonData
    {
        [SerializeField]
        public string bubbleName;

        [SerializeField]
        public CustomVector3 position;

        [SerializeField]
        public int row;

        [SerializeField]
        public int column;
    }

    // 버블 데이터 저장
    public void SaveMap(int maxRow, int maxColumn, int stageNumber, int mapNumber, GameObject[,] map)
    {
        List<BubbleJsonData> bubbles = new List<BubbleJsonData>();

        for (int i = 0; i < maxRow; i++)
        {
            for (int j = 0; j < maxColumn; j++)
            {
                if (map[i, j] == null)
                {
                    continue;
                }

                var bubbleJsonData = new BubbleJsonData();
                var currentBubble = map[i, j];

                bubbleJsonData.bubbleName = currentBubble.name;
                bubbleJsonData.position = new CustomVector3(currentBubble.transform.position);
                bubbleJsonData.row = i;
                bubbleJsonData.column = j;

                bubbles.Add(bubbleJsonData);
            }
        }

        CustomList<BubbleJsonData> bubbleList = new CustomList<BubbleJsonData>(bubbles);
        string mapJson = JsonUtility.ToJson(bubbleList, true);
        string path = $"{Application.dataPath}/Resources/Datas/Map Json Datas/stage{stageNumber}_map{mapNumber}.json";
        File.WriteAllText(path, mapJson);
    }

    // 맵 불러오기
    public void LoadMap(int maxRow, int maxColumn, int stageNumber, int mapNumber, GameObject[,] map)
    {
        if(map == null)
        {
            return;
        }

        MapInit(map, maxRow, maxColumn);

        string path = $"{Application.dataPath}/Resources/Datas/Map Json Datas/stage{stageNumber}_map{mapNumber}.json";
        string mapJson = File.ReadAllText(path);
        List<BubbleJsonData> bubbles = JsonUtility.FromJson<CustomList<BubbleJsonData>>(mapJson).ToList();

        for(int i = 0; i < bubbles.Count; i++)
        {
            int row = bubbles[i].row;
            int column = bubbles[i].column;

            map[row, column] = EventManager.Instance.OnGetBubble(bubbles[i].bubbleName, bubbles[i].position.ToVector3());

            map[row, column].GetComponent<Bubble>().Row = row;
            map[row, column].GetComponent<Bubble>().Column = column;
        }
    }

    // 맵 초기화
    private void MapInit(GameObject[,] map, int maxRow, int maxColumn)
    {
        for (int i = 0; i < maxRow; i++)
        {
            for (int j = 0; j < maxColumn; j++)
            {
                if (map[i, j] == null)
                {
                    continue;
                }

                var bubble = map[i, j].GetComponent<AllyBubble>();
                bubble.Row = 0;
                bubble.Column = 0;
                bubble.ChangeStateToWaiting();
                EventManager.Instance.OnGiveBubble(map[i, j]);
                map[i, j] = null;
            }
        }
    }
}
