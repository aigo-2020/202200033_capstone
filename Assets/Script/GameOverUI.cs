using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 오버 화면의 UI 요소와 버튼 기능을 관리합니다.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button restartButton;
    public Button lobbyButton;

    void Start()
    {
        // 버튼에 GameManager의 기능 연결
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() => {
                if (GameManager.Instance != null) GameManager.Instance.RestartGame();
            });
        }

        if (lobbyButton != null)
        {
            lobbyButton.onClick.AddListener(() => {
                if (GameManager.Instance != null) GameManager.Instance.GoToLobby();
            });
        }
    }
}
