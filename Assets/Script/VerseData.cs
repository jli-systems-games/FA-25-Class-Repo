// PoemData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Poem Data")]
public class PoemData : ScriptableObject
{
    public int totalLines;            
    public List<PoemLine> lines = new();
}