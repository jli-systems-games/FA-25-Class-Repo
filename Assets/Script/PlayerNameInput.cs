using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NameRosterUI : MonoBehaviour
{
    public TMP_InputField input;          
    public Transform listContainer;        
    public TMP_Text nameItemPrefab;         

    public int maxPlayers = 4;
    public int maxNameLength = 16;

 
    public void HandleEndEdit(string raw)
    {
        string name = Clean(raw);
        if (string.IsNullOrEmpty(name)) { ReFocus(); return; }


        if (RuntimePlayersRegistry.Instance && RuntimePlayersRegistry.Instance.players.Count >= maxPlayers)
        {
            ReFocus(clear: false);
            return;
        }


        // 计算 index
        int idx = RuntimePlayersRegistry.Instance ? RuntimePlayersRegistry.Instance.players.Count : 0;

        // 1） 生成运行时 SO
        var so = ScriptableObject.CreateInstance<PlayerData>();
        so.playerName = name;
        so.index = idx;

        // 2）注册器
        if (RuntimePlayersRegistry.Instance)
            RuntimePlayersRegistry.Instance.players.Add(so);

        // 3）生成去列阵里
        var item = Instantiate(nameItemPrefab, listContainer);
        item.text = $"{idx + 1}. {name}"; 

        // 4) 重新写入
        ReFocus(clear: true);

        Debug.Log($"创建 PlayerData：{name} (index={idx})，当前总数={RuntimePlayersRegistry.Instance.players.Count}");
    }

    private bool IsDuplicate(string name)
    {
        if (RuntimePlayersRegistry.Instance == null) return false;
        foreach (var p in RuntimePlayersRegistry.Instance.players)
            if (p.playerName == name) return true;
        return false;
    }

    private string Clean(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        s = s.Trim();
        return s.Length > maxNameLength ? s.Substring(0, maxNameLength) : s;
    }

    // 重新写入
    private void ReFocus(bool clear = true)
    {
        if (clear) input.text = "";
        input.ActivateInputField(); 
        input.Select();
        input.caretPosition = input.text.Length;
    }
}