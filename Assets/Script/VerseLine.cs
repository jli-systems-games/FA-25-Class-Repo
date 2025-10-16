using UnityEngine;

[CreateAssetMenu(menuName = "Poetry/Runtime/Poem Line")]
public class PoemLine : ScriptableObject
{
    public string authorName;  
    public string text;       
    public int lineIndex;      
}