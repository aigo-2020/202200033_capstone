using UnityEngine;

public class Portal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 물체의 태그가 "Player"인지 확인
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어가 포탈에 진입했습니다! 이벤트를 발생시킵니다.");
            
            // StageEventManager를 통해 스테이지에 맞는 이벤트를 발생시킵니다.
            if (StageEventManager.Instance != null)
            {
                StageEventManager.Instance.CheckAndTriggerEvent();
            }
            
            // 포탈의 목적을 달성했으므로 포탈 파괴
            Destroy(gameObject);
        }
    }
}
