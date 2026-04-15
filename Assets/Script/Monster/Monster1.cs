using UnityEngine;

/// <summary>
/// 가장 기초적인 근접형 몬스터입니다.
/// MonsterBase를 상속받아 플레이어를 추격하고 충돌 시 데미지를 입힙니다.
/// </summary>
public class Monster1 : MonsterBase
{
    private bool isKnockingBack = false;
    private float lastAttackTime = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        // 초기화 완료 및 플레이어 생존 시, 넉백 중이 아닐 때만 이동
        if (isInitialized && playerTransform != null && !isKnockingBack)
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// 플레이어 방향으로 이동 로직
    /// </summary>
    void MoveTowardsPlayer()
    {
        Vector2 direction = (Vector2)playerTransform.position - rb.position;
        direction.Normalize();
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 플레이어와 충돌 시 공격 처리
    /// </summary>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 공격 속도에 따른 쿨타임 체크
            if (Time.time >= lastAttackTime + (1f / attackRate))
            {
                Attack(collision.gameObject);
                lastAttackTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 플레이어에게 데미지를 입히고 쌍방향 넉백 적용
    /// </summary>
    void Attack(GameObject player)
    {
        if (player.TryGetComponent<PlayerStats>(out var stats))
        {
            stats.TakeDamage(damage);
        }

        Vector2 knockbackDir = (player.transform.position - transform.position).normalized;

        if (player.TryGetComponent<PlayerController>(out var controller))
        {
            controller.ApplyKnockback(knockbackDir * knockbackForce, 0.2f);
        }

        StartCoroutine(RecoilCoroutine(-knockbackDir * recoilForce, 0.2f));
    }

    private System.Collections.IEnumerator RecoilCoroutine(Vector2 force, float duration)
    {
        isKnockingBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        isKnockingBack = false;
        rb.linearVelocity = Vector2.zero;
    }
}
