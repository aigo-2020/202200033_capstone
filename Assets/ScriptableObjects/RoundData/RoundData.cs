using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoundData", menuName = "ScriptableObjects/RoundData")]
public class RoundData : ScriptableObject
{
    [Header("라운드 구성")]
    public List<MonsterData> possibleMonsters; // 이 라운드에서 등장할 수 있는 몬스터 목록

    [Header("예산 설정 (난이도 총합)")]
    public int baseBudget = 20;               // 라운드의 첫 번째 스테이지 기본 예산
    public int budgetIncrement = 5;           // 스테이지가 올라갈 때마다 추가될 예산
}
