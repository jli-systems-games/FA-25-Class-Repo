using UnityEngine;
using System.Collections.Generic;

public class BrushManager : MonoBehaviour
{
    public List<BrushSettings> brushes = new();
    public int currentBrushIndex = 0;

    public BrushSettings Current =>
        (brushes.Count > 0) ? brushes[Mathf.Clamp(currentBrushIndex, 0, brushes.Count - 1)] : null;

    public void SetBrush(int index) { currentBrushIndex = Mathf.Clamp(index, 0, brushes.Count - 1); }
}
