using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    // 게임 상태
    public enum EnumGameState
    {
        AIM,
        REMOVE,
        DROPLINE,
        RELOAD,
        STOP,
        CREATE_MAP
    }

    [SerializeField]
    private Canvas menuCanvas = null;

    // 게임 상태
    [SerializeField]
    public EnumGameState gameState;

    // 이전 게임 상태
    private EnumGameState lastGameState;

    public bool isCreateMapMode = false;

    void Awake()
    { 
        gameState = EnumGameState.RELOAD;
    }

    // pause 버튼 눌렀을때
    public void ClickPauseButton()
    {
        menuCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
        lastGameState = gameState;
        gameState = EnumGameState.STOP;
    }

    // 되돌아가기
    public void ClickBackButton()
    {
        menuCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
        Invoke("ChangeGameStage", 0.2f);
    }

    private void ChangeGameStage()
    {
        gameState = lastGameState;
    }

    // 다시시작
    public void Retry()
    {
        //EventManager.Instance.OnInitMap();
        //EventManager.Instance.OnInitBubblePool();
        menuCanvas.gameObject.SetActive(false);
        Time.timeScale = 1;
        gameState = EnumGameState.AIM;
    }
}
