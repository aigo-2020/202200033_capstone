namespace StatSystem
{
    // 플레이어가 가질 수 있는 모든 스탯 종류 정의
    public enum StatType
    {
        MaxHp,      // 최대 체력
        Damage,     // 공격력
        Speed,      // 이동 속도
        FireRate,   // 연사력
        Range,      // 사거리
        Defense     // 방어력
    }

    // 스탯 계산 방식 정의
    public enum ModifierType
    {
        Flat,       // 단순 합산 (예: +5)
        Percent     // 퍼센트 곱산 (예: +10% -> 1.1배)
    }
}
