using UnityEngine;

/// <summary>
/// 발사체(화살) 클래스입니다.
/// IDamageable 인터페이스를 가진 모든 대상에게 데미지를 입힙니다.
/// </summary>
public class Arrow : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;       // 투사체 속도
    public float damage = 10f;      // 투사체의 공격력 (PlayerAttack에서 설정)
    public float range = 500f;      // 투사체의 사거리 (PlayerAttack에서 설정)
    public PlayerStats owner;       // 투사체를 발사한 플레이어 정보

    private Vector2 startPosition;  // 발사 시작 위치

    void Start()
    {
        // 발사 시작 시의 위치를 저장
        startPosition = transform.position;
    }

    void Update()
    {
        // 오른쪽 방향으로 이동
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // 시작 위치로부터의 거리가 사거리(range)를 초과하면 파괴
        float distanceTraveled = Vector2.Distance(startPosition, transform.position);
        if (distanceTraveled >= range)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 충돌한 대상이 데미지를 입을 수 있는 객체(IDamageable)인지 확인
        IDamageable target = collision.GetComponent<IDamageable>();
        
        if (target != null)
        {
            if (owner != null)
            {
                // 플레이어의 공격 성공 트리거를 호출 (아이템 효과 적용 후 실제 데미지 전달)
                owner.OnAttackSuccess(target, damage);
            }
            else
            {
                // 소유자 정보가 없는 경우 기본 데미지 처리
                target.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall")) // 벽과 충돌 시 파괴 (태그 필요)
        {
            Destroy(gameObject);
        }
    }
}
