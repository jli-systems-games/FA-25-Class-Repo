using UnityEngine;

public static class SelectionData
{
    public static AnimalStats P1;  // 선택된 캐릭터 SO
    public static AnimalStats P2;  // 선택된 캐릭터 SO

    public static void Clear()
    {
        P1 = null;
        P2 = null;
    }
}
