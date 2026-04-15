using UnityEngine;

// Ensure the component is present on the gameobject the script is attached to
// Uncomment this if you want to enforce the object to require the RB2D component to be already attached
// [RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 50f;    // 정지 상태에서 최고 속도까지 가속되는 정도
    [SerializeField] private float deceleration = 60f;    // 입력이 없을 때 멈추는 정도

    private new Rigidbody2D rigidbody2D;
    private Vector2 inputVector = Vector2.zero;
    private PlayerStats playerStats;
    private bool isKnockedBack = false;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats component is missing!");
        }

        if (!TryGetComponent<Rigidbody2D>(out rigidbody2D))
        {
            rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // 유니티 6 탑다운 이동을 위한 물리 설정 최적화
        rigidbody2D.angularDamping = 0.0f;
        rigidbody2D.gravityScale = 0.0f;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate; // 떨림 방지
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // 벽 뚫기 방지
        rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 방지
    }

    void Update()
    {
        if (!isKnockedBack)
        {
            // 입력값을 Raw하게 가져와서 코드에서 가속/감속을 직접 제어합니다.
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            inputVector = new Vector2(moveX, moveY).normalized;
        }
        else
        {
            inputVector = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (playerStats != null && !isKnockedBack)
        {
            ApplyMovement();
        }
    }

    private void ApplyMovement()
    {
        // 목표 속도 계산 (스탯 기반)
        Vector2 targetVelocity = inputVector * playerStats.speed.GetValue();
        
        // 현재 이동 상태에 따라 가속 또는 감속 속도 선택
        float lerpSpeed = inputVector.magnitude > 0 ? acceleration : deceleration;
        
        // 현재 속도에서 목표 속도까지 부드럽게 변화
        rigidbody2D.linearVelocity = Vector2.MoveTowards(
            rigidbody2D.linearVelocity, 
            targetVelocity, 
            lerpSpeed * Time.fixedDeltaTime
        );
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackCoroutine(force, duration));
    }

    private System.Collections.IEnumerator KnockbackCoroutine(Vector2 force, float duration)
    {
        isKnockedBack = true;
        rigidbody2D.linearVelocity = Vector2.zero;
        rigidbody2D.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
        rigidbody2D.linearVelocity = Vector2.zero;
    }
}