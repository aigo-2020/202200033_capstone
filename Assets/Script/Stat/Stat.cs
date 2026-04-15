using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace StatSystem
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField] private float baseValue; // 기본 수치 (아이템 없을 때)
        public float BaseValue => baseValue;

        // 적용된 모든 모디파이어 리스트
        private readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> StatModifiers;

        // 값이 변경되었을 때 알림을 주는 이벤트
        public event System.Action OnValueChanged;

        public Stat(float value)
        {
            baseValue = value;
            statModifiers = new List<StatModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        private void NotifyChange()
        {
            OnValueChanged?.Invoke();
        }

        // 최종 계산된 수치 반환
        public float GetValue()
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0; // 퍼센트 증가량 합계

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == ModifierType.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == ModifierType.Percent)
                {
                    // 퍼센트는 합연산 후 최종적으로 곱함 (예: 10% 증가 2개면 20% 증가)
                    sumPercentAdd += mod.Value;
                }
            }

            // (기본값 + 합산값) * (1 + 퍼센트합계)
            return finalValue * (1 + sumPercentAdd);
        }

        // 모디파이어 추가
        public void AddModifier(StatModifier mod)
        {
            statModifiers.Add(mod);
            NotifyChange();
        }

        // 특정 출처(아이템 등)에서 온 모든 모디파이어 삭제 (정확한 삭제 가능)
        public bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;

            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].Source == source)
                {
                    statModifiers.RemoveAt(i);
                    didRemove = true;
                }
            }

            if (didRemove)
            {
                NotifyChange();
            }

            return didRemove;
        }
    }
}
