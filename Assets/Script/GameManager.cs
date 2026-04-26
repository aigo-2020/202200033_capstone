using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 전역 상태(로비, 플레이, 게임오버) 및 씬 전환을 관리하는 최상위 관리자입니다.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Lobby, Playing, Paused, GameOver }
    
    [Header("Game State")]
    public GameState currentState = GameState.Playing;

    [Header("UI References")]
    public GameObject gameOverUI; // 게임 오버 UI 패널 (GameOverUI 스크립트가 붙은 오브젝트)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 플레이어 사망 시 호출되어 게임 오버 상태로 전환합니다.
    /// </summary>
    public void GameOver()
    {
        if (currentState == GameState.GameOver) return;

        currentState = GameState.GameOver;
        Time.timeScale = 0f; // 게임 정지
        
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        
        Debug.Log("GameManager: 게임 오버 상태 진입");
    }

    /// <summary>
    /// 현재 게임 세션을 리셋하고 처음부터 다시 시작합니다.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;
        
        if (StageManager.Instance != null)
        {
            StageManager.Instance.currentStage = 1;
            // 필요한 경우 추가적인 데이터 초기화 로직 수행
        }

        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// 로비 씬으로 이동합니다.
    /// </summary>
    public void GoToLobby()
    {
        Time.timeScale = 1f;
        currentState = GameState.Lobby;
        SceneManager.LoadScene("LobbyScene");
    }
}
