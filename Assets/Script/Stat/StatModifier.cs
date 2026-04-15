using UnityEngine;

namespace StatSystem
{
    // 스탯에 가해지는 개별적인 변화 정보를 담는 클래스
    [System.Serializable]
    public class StatModifier
    {
        public float Value;         // 변화량 (수치)
        public ModifierType Type;   // 계산 방식 (합산 / 퍼센트)
        public object Source;       // 이 변화를 준 출처 (예: 어떤 ItemData인지)

        public StatModifier(float value, ModifierType type, object source)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
}
