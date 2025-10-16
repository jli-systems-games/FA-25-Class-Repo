using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class ShowPoemToLayout : MonoBehaviour
{

    public PoemData poemData;  

   
    public Transform linesContainer;
    public Transform authorsContainer;

    
    public TMP_Text lineItemPrefab;
    public TMP_Text authorItemPrefab;

  
    public bool showLineNumber = true;
    public string lineNumberFormat = "{0}. ";


    public bool showDistinctAuthorsOnly = true;          
    public bool preferRegistryOrderForAuthors = true;      


    public bool refreshOnEnable = true;

    private void OnEnable()
    {
        if (refreshOnEnable) StartCoroutine(TryRefreshWhenReady());
    }

    private System.Collections.IEnumerator TryRefreshWhenReady()
    {
        if (poemData == null)
        {
            while (PoemDataStore.Instance == null) yield return null;
            while (PoemDataStore.Instance.Current == null) yield return null;
            poemData = PoemDataStore.Instance.Current;
        }

      
        yield return null;

        Refresh();
    }

    
    public void Refresh()
    {
   
        if (poemData == null)
        {
            Debug.LogWarning("[ShowPoemToLayout] poemData 为空（既没手动拖，也没从 Store 拿到）。");
            return;
        }
        if (linesContainer == null || authorsContainer == null)
        {
            Debug.LogWarning("[ShowPoemToLayout] 容器引用未设置。");
            return;
        }
        if (lineItemPrefab == null || authorItemPrefab == null)
        {
            Debug.LogWarning("[ShowPoemToLayout] 预制体引用未设置。");
            return;
        }

        // cleardata
        ClearChildren(linesContainer);
        ClearChildren(authorsContainer);

        //line up
        var orderedLines = poemData.lines?
                           .Where(l => l != null)
                           .OrderBy(l => l.lineIndex)
                           .ToList() ?? new List<PoemLine>();

        for (int i = 0; i < orderedLines.Count; i++)
        {
            var l = orderedLines[i];

            // 诗句项
            var lineGO = Instantiate(lineItemPrefab.gameObject, linesContainer);
            lineGO.SetActive(true);
            var lineText = lineGO.GetComponent<TMP_Text>();
            string prefix = showLineNumber ? string.Format(lineNumberFormat, i + 1) : "";
            lineText.text = prefix + (l.text ?? "");
        }

        //show writer
        if (showDistinctAuthorsOnly)
        {
            // use index for order/ this part confuse me idk i copied this part online sry
            List<string> authorNames = null;

            if (preferRegistryOrderForAuthors &&
                RuntimePlayersRegistry.Instance != null &&
                RuntimePlayersRegistry.Instance.players != null &&
                RuntimePlayersRegistry.Instance.players.Count > 0)
            {
                authorNames = RuntimePlayersRegistry.Instance.players
                               .Where(p => p != null && !string.IsNullOrEmpty(p.playerName))
                               .OrderBy(p => p.index)
                               .Select(p => p.playerName)
                               .Distinct()
                               .ToList();
            }
            else 
            {
                
                authorNames = orderedLines
                              .Select(l => l.authorName)
                              .Where(n => !string.IsNullOrEmpty(n))
                              .Distinct()        // LINQ 的 Distinct 保留首次出现顺序
                              .ToList();
            }

            // show only once
            foreach (var name in authorNames)
            {
                var authorGO = Instantiate(authorItemPrefab.gameObject, authorsContainer);
                authorGO.SetActive(true);
                authorGO.GetComponent<TMP_Text>().text = name;
            }
        }
        else
        {
            
            foreach (var l in orderedLines)
            {
                var authorGO = Instantiate(authorItemPrefab.gameObject, authorsContainer);
                authorGO.SetActive(true);
                var authorText = authorGO.GetComponent<TMP_Text>();
                authorText.text = string.IsNullOrEmpty(l.authorName) ? "—" : l.authorName;
            }
        }

     
        ForceRebuild(linesContainer);
        ForceRebuild(authorsContainer);

       
    }

    private void ClearChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
            Destroy(parent.GetChild(i).gameObject);
    }

    private void ForceRebuild(Transform t)
    {
        if (t is RectTransform rt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            Canvas.ForceUpdateCanvases();
        }
    }
}