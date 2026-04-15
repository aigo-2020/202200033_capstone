using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab; // 발사할 투사체 프리팹 (Arrow 태그 설정 필요)

    private float nextFireTime = 0f;    // 다음 발사 가능 시간
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component is missing!");
        }

        // 게임 시작 시 의도치 않은 첫 프레임 클릭을 무시하기 위해 0.1초 유예
        nextFireTime = Time.time + 0.1f;
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 누르고 있고, 연사 쿨타임이 지났을 때 발사
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && playerStats != null)
        {
            Fire();
            // 1초를 연사력(fireRate)으로 나누면 한 번 발사 시 대기해야 할 '간격'이 나옵니다.
            float fireInterval = 1f / Mathf.Max(0.01f, playerStats.fireRate.GetValue());
            nextFireTime = Time.time + fireInterval;
        }
    }

    void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("PlayerAttack: Projectile Prefab이 할당되지 않았습니다!");
            return;
        }

        // 1. 마우스 위치를 월드 좌표로 변환하여 조준 방향 계산
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector2 fireDirection = ((Vector2)mousePosition - (Vector2)transform.position).normalized;

        if (fireDirection == Vector2.zero) return;

        // 2. 투사체가 날아갈 각도 계산 (오른쪽 방향 기준)
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;

        // 3. 투사체 생성
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
        
        // 생성된 투사체에 플레이어의 공격력과 사거리 전달
        Arrow arrow = projectile.GetComponent<Arrow>();
        if (arrow != null)
        {
            arrow.damage = playerStats.damage.GetValue();
            arrow.range = playerStats.range.GetValue();
        }
    }
}
