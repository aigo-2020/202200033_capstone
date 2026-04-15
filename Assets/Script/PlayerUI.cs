using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class PlayerUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats; // 플레이어 스탯 스크립트 참조

    [Header("UI Elements")]
    public TextMeshProUGUI hpText;       // 체력 표시용 TMP
    public TextMeshProUGUI damageText;   // 공격력 표시용 TMP
    public TextMeshProUGUI fireRateText; // 연사력 표시용 TMP
    public TextMeshProUGUI rangeText;    // 사거리 표시용 TMP
    public TextMeshProUGUI speedText;    // 속도 표시용 TMP
    public TextMeshProUGUI moneyText;    // 재화 표시용 TMP
    public TextMeshProUGUI stageText;    // 스테이지 표시용 TMP (추가)

    void Update()
    {
        if (playerStats == null) return;

        // 매 프레임 스탯 정보를 업데이트 (최적화가 필요할 경우 별도의 UpdateUI 함수로 분리 가능)
        UpdateStatDisplay();
        UpdateStageDisplay(); // 스테이지 정보 업데이트 추가
    }

    void UpdateStatDisplay()
    {
        if (hpText != null)
            hpText.text = $"HP: {playerStats.currentHp:F0} / {playerStats.maxHp.GetValue():F0}";
        
        if (damageText != null)
            damageText.text = $"Damage: {playerStats.damage.GetValue():F1}";

        if (fireRateText != null)
            fireRateText.text = $"Fire Rate: {playerStats.fireRate.GetValue():F2}";
        
        if (rangeText != null)
            rangeText.text = $"Range: {playerStats.range.GetValue():F0}";

        if (speedText != null)
            speedText.text = $"Speed: {playerStats.speed.GetValue():F1}";

        if (moneyText != null)
            moneyText.text = $"Money: {playerStats.money}";
    }

    void UpdateStageDisplay()
    {
        if (stageText != null && StageManager.Instance != null)
        {
            stageText.text = $"Stage: {StageManager.Instance.currentStage}";
        }
    }
}
