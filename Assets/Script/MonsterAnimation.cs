using UnityEngine;

/// <summary>
/// 몬스터의 애니메이션과 시각적 상태(좌우 반전 등)를 제어하는 스크립트입니다.
/// 모든 몬스터 프리팹에 공통으로 부착하여 사용할 수 있습니다.
/// </summary>
public class MonsterAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MonsterBase monsterBase;

    void Awake()
    {
        // 필요한 컴포넌트 참조 캐싱
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        monsterBase = GetComponent<MonsterBase>();

        if (animator == null) Debug.LogWarning($"{gameObject.name}: Animator 컴포넌트가 없습니다.");
        if (spriteRenderer == null) Debug.LogWarning($"{gameObject.name}: SpriteRenderer 컴포넌트가 없습니다.");
        if (monsterBase == null) Debug.LogWarning($"{gameObject.name}: MonsterBase 기반 스크립트가 없습니다.");
    }

    void Update()
    {
        if (monsterBase == null || animator == null || spriteRenderer == null) return;

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        // 1. 이동 상태 업데이트 (isRunning 파라미터)
        bool isRunning = monsterBase.IsMoving;
        animator.SetBool("isRunning", isRunning);

        // 2. 이동 방향에 따른 좌우 반전 (flipX)
        Vector2 direction = monsterBase.MoveDirection;

        // X축 이동 속도가 일정 수치 이상일 때만 반전 처리 (떨림 방지)
        if (direction.x > 0.1f)
        {
            spriteRenderer.flipX = false; // 오른쪽 보기
        }
        else if (direction.x < -0.1f)
        {
            spriteRenderer.flipX = true; // 왼쪽 보기
        }

        // 3. MoveX 파라미터 업데이트 (필요 시 블렌드 트리에서 사용 가능)
        animator.SetFloat("MoveX", direction.x);
    }
}
