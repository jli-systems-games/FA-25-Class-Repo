using UnityEngine;
using TMPro;

public class WhipCreamMicrogame : MicrogameBase
{
    [Header("Game Settings")]
    public int basePresses = 10;

    [Header("Orbit Settings")]
    public Transform bowl;
    public Transform whisk;  
    public float orbitRadius = 1.0f;
    public float angularSpeedDeg = 180f;
    public float startAngleDeg = 0f;

    [Header("UI")]
    public TextMeshProUGUI promptText; 

    int needPresses;
    int count;
    float angleDeg;

    protected override void OnBegin()
    {
        needPresses = Mathf.RoundToInt(basePresses + (3.5f - timeLimit) * 4.0f);

        if (promptText != null)
        {
            promptText.text = "Press SPACE!";
            promptText.enabled = true;
        }

        angleDeg = startAngleDeg;
        if (bowl != null && whisk != null)
        {
            Vector3 center = bowl.position;
            Vector3 offset = AngleToOffset(angleDeg, orbitRadius);
            whisk.position = center + offset;
            AlignWhiskTangent();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { Debug.Log("SPACE pressed"); }

        if (!running) return;


        if (bowl != null && whisk != null)
        {
            angleDeg -= angularSpeedDeg * Time.deltaTime;
            Vector3 center = bowl.position;
            Vector3 offset = AngleToOffset(angleDeg, orbitRadius);
            whisk.position = center + offset;

            AlignWhiskTangent();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            count++;
            if (count >= needPresses)
            {
                if (promptText) promptText.enabled = false;
                Finish(true);
            }
        }
    }

    protected override bool CheckAutoComplete() => false;

    Vector3 AngleToOffset(float angDeg, float radius)
    {
        float rad = angDeg * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius;
    }

    void AlignWhiskTangent()
    {
        float tangentDeg = angleDeg - 90f;
        whisk.rotation = Quaternion.Euler(0f, 0f, tangentDeg);
    }
}
