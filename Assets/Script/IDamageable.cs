/// <summary>
/// 데미지를 입을 수 있는 모든 객체(몬스터, 부술 수 있는 장애물 등)가 구현해야 하는 인터페이스입니다.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 데미지를 입었을 때 실행되는 함수입니다.
    /// </summary>
    /// <param name="damage">입히려는 데미지 수치</param>
    void TakeDamage(float damage);
}
