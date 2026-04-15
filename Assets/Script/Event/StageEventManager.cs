using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 스테이지 번호에 따라 어떤 이벤트(보상/상점)를 띄울지 결정하는 중앙 관리자입니다.
/// </summary>
public class StageEventManager : MonoBehaviour
{
    public static StageEventManager Instance { get; private set; }

    [Header("Stage Settings (수정하여 테스트 가능)")]
    public List<int> rewardEventStages = new List<int> { 1, 2, 3 };
    public List<int> shopEventStages = new List<int> { 4, 5 };

    [Header("UI Managers")]
    public RewardUIManager rewardUIManager;
    public ShopUIManager shopUIManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 포탈 진입 시 호출되는 메인 함수입니다.
    /// </summary>
    public void CheckAndTriggerEvent()
    {
        if (StageManager.Instance == null) return;

        int currentStage = StageManager.Instance.currentStage;

        if (rewardEventStages.Contains(currentStage))
        {
            Debug.Log($"스테이지 {currentStage}: 일반 보상 이벤트를 시작합니다.");
            if (rewardUIManager != null) rewardUIManager.ShowRewardUI();
        }
        else if (shopEventStages.Contains(currentStage))
        {
            Debug.Log($"스테이지 {currentStage}: 상점 이벤트를 시작합니다.");
            if (shopUIManager != null) shopUIManager.ShowShopUI();
        }
        else
        {
            // 지정되지 않은 스테이지는 그냥 다음 스테이지로 진행
            Debug.Log($"스테이지 {currentStage}: 이벤트가 지정되지 않았습니다. 바로 다음 스테이지로 넘어갑니다.");
            StageManager.Instance.NextStage();
        }
    }
}
