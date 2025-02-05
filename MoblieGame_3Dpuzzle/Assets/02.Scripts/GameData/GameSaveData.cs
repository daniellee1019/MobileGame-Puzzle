using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int currentDay;                   // 현재 진행 중인 Day
    public List<DayRecordData> dayRecords;         // 각 Day별 보스 클리어 여부 기록
    public CurrencyData currencyData;          // 보스 보상 기준 재화 (예: Day1 보상)
}
