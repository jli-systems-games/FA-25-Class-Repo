using UnityEngine;

public class WhipCreamMicrogame : MicrogameBase
{
    [Header("Settings")]
    public int basePresses = 10;
    int needPresses;
    int count;

    protected override void OnBegin()
    {
        needPresses = Mathf.RoundToInt(basePresses + (3.5f - timeLimit) * 4.0f);
    }

    void Update()
    {
        if (!running) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            if (count >= needPresses)
            {
                Finish(true);
            }
        }
    }

    protected override bool CheckAutoComplete() => false;
}

