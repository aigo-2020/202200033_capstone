using UnityEngine;
using TMPro;

/// <summary>
/// Tab 키를 눌러 열 수 있는 상세 상태창(인벤토리/스탯)을 관리합니다.
/// 열려 있는 동안 게임을 일시정지하고 플레이어의 상세 스탯을 표시합니다.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI Structure")]
    public GameObject uiContent; // 실제 내용이 담긴 패널 오브젝트 (Tab으로 끄고 켤 대상)
    private bool isOpened = false;

    [Header("Player Stats Reference")]
    public PlayerStats playerStats;

    [Header("Stat Text Elements")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI defenseText;

    [Header("Level Text Elements")]
    public TextMeshProUGUI attackLevelText;
    public TextMeshProUGUI defenseLevelText;
    public TextMeshProUGUI agilityLevelText;

    void Start()
    {
        // 시작 시 창을 닫아둡니다.
        if (uiContent != null)
        {
            uiContent.SetActive(false);
            isOpened = false;
        }

        if (playerStats == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) playerStats = player.GetComponent<PlayerStats>();
        }
    }

    void Update()
    {
        // Tab 키 입력 감지
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStatusWindow();
        }

        // 창이 열려 있을 때만 실시간으로 스탯 정보를 업데이트합니다.
        if (isOpened && playerStats != null)
        {
            UpdateStatTexts();
        }
    }

    /// <summary>
    /// 상태창을 열거나 닫으며 게임의 일시정지 상태를 제어합니다.
    /// </summary>
    public void ToggleStatusWindow()
    {
        if (uiContent == null) return;

        isOpened = !isOpened;
        uiContent.SetActive(isOpened);

        // 게임 일시정지 제어
        Time.timeScale = isOpened ? 0f : 1f;

        if (isOpened)
        {
            UpdateStatTexts();
        }
    }

    /// <summary>
    /// 플레이어의 상세 스탯 및 카테고리 레벨 정보를 텍스트에 출력합니다.
    /// </summary>
    void UpdateStatTexts()
    {
        if (playerStats == null) return;

        // 상세 스탯 정보 (소수점 자릿수 조절 가능)
        if (damageText != null) damageText.text = $"Damage: {playerStats.damage.GetValue():F1}";
        if (fireRateText != null) fireRateText.text = $"Fire Rate: {playerStats.fireRate.GetValue():F2}";
        if (rangeText != null) rangeText.text = $"Range: {playerStats.range.GetValue():F1}";
        if (speedText != null) speedText.text = $"Speed: {playerStats.speed.GetValue():F1}";
        if (defenseText != null) defenseText.text = $"Defense: {playerStats.defense.GetValue():F1}";

        // 카테고리 레벨 정보 (SpecUp)
        if (attackLevelText != null) attackLevelText.text = $"Attack Lv: {playerStats.attackLevel}";
        if (defenseLevelText != null) defenseLevelText.text = $"Defense Lv: {playerStats.defenseLevel}";
        if (agilityLevelText != null) agilityLevelText.text = $"Agility Lv: {playerStats.agilityLevel}";
    }
}
