using UnityEngine;

/// <summary>
/// 플레이어의 애니메이션과 시각적 피드백(이동에 따른 좌우 반전 등)을 처리합니다.
/// 유지보수를 위해 이동 로직과 애니메이션 로직을 분리하여 관리합니다.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    void Awake()
    {
        // 필요한 컴포넌트 참조 캐싱
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();

        if (animator == null) Debug.LogWarning("PlayerAnimation: Animator 컴포넌트가 없습니다.");
        if (spriteRenderer == null) Debug.LogWarning("PlayerAnimation: SpriteRenderer 컴포넌트가 없습니다.");
        if (playerController == null) Debug.LogWarning("PlayerAnimation: PlayerController 컴포넌트가 없습니다.");
    }

    void Update()
    {
        if (playerController == null || animator == null || spriteRenderer == null) return;

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        Vector2 input = playerController.InputVector;

        // 1. 플레이어의 이동 여부 확인 (입력 벡터 크기 기준)
        bool isRunning = input.sqrMagnitude > 0.01f;
        animator.SetBool("isRunning", isRunning);

        // 2. 수평 이동 입력에 따른 좌우 반전 처리
        // 마지막으로 바라보던 방향을 유지하기 위해 입력이 있을 때만 flipX를 갱신합니다.
        if (input.x > 0.1f)
        {
            spriteRenderer.flipX = false; // 오른쪽 바라보기
        }
        else if (input.x < -0.1f)
        {
            spriteRenderer.flipX = true; // 왼쪽 바라보기
        }

        // 3. Animator의 MoveX 파라미터 업데이트
        // 스크립트에서 flipX를 제어하지만, 애니메이션 전이 조건으로 MoveX 값을 활용할 수 있습니다.
        animator.SetFloat("MoveX", input.x);
    }
}
