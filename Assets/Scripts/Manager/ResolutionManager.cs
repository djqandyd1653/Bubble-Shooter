using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    private readonly int baseResolutionWidth = 1080;
    public int BaseResolutionWidth { get { return baseResolutionWidth; } }

    private readonly int baseResolutionHeight = 1920;
    public int BaseResolutionHeight { get { return baseResolutionHeight; } }

    // test Image;
    [SerializeField]
    Image test;
    /// <summary>
    /// https://www.youtube.com/watch?v=mfFQxCeGKMo
    /// </summary>

    private void Start()
    {
#if UNITY_EDITOR_WIN
        Debug.Log("222");
#endif

        //if(baseResolutionWidth != Screen.width)
        //{
        //    test.rectTransform.sizeDelta = new Vector2(Screen.width * test.rectTransform.sizeDelta.x / baseResolutionWidth / test.sprite.bounds.size.x, test.rectTransform.sizeDelta.y);
        //}

        //if(baseResolutionHeight != Screen.height)
        //{
        //    test.rectTransform.sizeDelta = new Vector2(test.rectTransform.sizeDelta.x, Screen.height * test.rectTransform.sizeDelta.y / baseResolutionHeight / test.sprite.bounds.size.y);
        //}

        //Vector3 windowLeftDownPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
        //Vector3 windowRightUpPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //float calWidth = (windowRightUpPoint.x - windowLeftDownPoint.x) / 11;
        //float calHeight = (windowRightUpPoint.y - windowLeftDownPoint.y) / 20;

        test.rectTransform.sizeDelta = new Vector2(test.rectTransform.sizeDelta.x * Screen.width / baseResolutionWidth, test.rectTransform.sizeDelta.y * Screen.height / baseResolutionHeight);

        // test
        //test.transform.localScale = new Vector3(1, 1, 1);
        //Vector3 a = test.sprite.bounds.size;
        //test.rectTransform.sizeDelta = new Vector2(calWidth * 11 / a.x * 0.05f, ((calHeight * 14) + 660) / a.y * 0.05f); //width = calWidth * 11 / a.x;
        //test.transform.localScale = new Vector3(calWidth * 11 / a.x, ((calHeight * 14) + 660) / a.y, 0);

        //float cameraSize = Camera.main.orthographicSize;
    }
}
