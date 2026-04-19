using UnityEngine;

/// <summary>
/// 몬스터가 발사하는 투사체 스크립트입니다.
/// 플레이어와 충돌 시 데미지를 입히고 소멸합니다.
/// </summary>
public class MonsterProjectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private float lifetime = 5f;

    /// <summary>
    /// 발사 직후 몬스터로부터 스탯을 전달받아 초기화합니다.
    /// </summary>
    public void Initialize(float damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
        
        // 일정 시간 후 자동 파괴 (최적화)
        Destroy(gameObject, lifetime);

        // 물리 속도 부여 (Unity 6 기준 linearVelocity 사용)
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = transform.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어 태그 확인
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerStats>(out var stats))
            {
                stats.TakeDamage(damage);
                Debug.Log($"플레이어가 몬스터 투사체에 맞음! 데미지: {damage}");
            }
            
            // 이펙트나 사운드 추가 지점
            Destroy(gameObject); // 충돌 시 제거
        }
    }
}
