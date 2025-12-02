using UnityEngine;
using TMPro;

public class SimpleEnigma : MonoBehaviour
{
    [Header("--- UI 显示 ---")]
    public TMP_Text screenText;      // 显示结果的屏幕 (比如 "AWAKE")
    
    // 三个转轮的显示文字 (显示 A, B, C...)
    public TMP_Text textRotorL;      
    public TMP_Text textRotorM;
    public TMP_Text textRotorR;

    [Header("--- 核心数据 ---")]
    private string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // 记录三个转轮当前的位置 (0=A, 1=B ... 25=Z)
    private int indexL = 0;
    private int indexM = 0;
    private int indexR = 0;

    private void Start()
    {
        UpdateRotorUI();
        screenText.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearScreen();
        }
    }

    // =================================================
    // 1. 转轮控制 (绑定到三个转轮按钮上)
    // =================================================

    public void ClickRotorLeft()
    {
        indexL = (indexL + 1) % 26;
        UpdateRotorUI();
    }

    public void ClickRotorMid()
    {
        indexM = (indexM + 1) % 26;
        UpdateRotorUI();
    }

    public void ClickRotorRight()
    {
        indexR = (indexR + 1) % 26;
        UpdateRotorUI();
    }

    void UpdateRotorUI()
    {
        if(textRotorL) textRotorL.text = alphabet[indexL].ToString();
        if(textRotorM) textRotorM.text = alphabet[indexM].ToString();
        if(textRotorR) textRotorR.text = alphabet[indexR].ToString();
    }

    // =================================================
    // 2. 键盘输入 (绑定到 A-Z 的26个按钮上)
    // =================================================

    /// <summary>
    /// 核心解密逻辑：输入乱码 -> 得到明文
    /// </summary>
    public void PressKey(string inputChar)
    {
        int charIndex = alphabet.IndexOf(inputChar.ToUpper());

        if (charIndex != -1)
        {
            // --- 核心算法 ---
            // 明文 = (按键输入 - 总偏移量 + 26) % 26
            // 这样设计是为了：如果转轮设对了，按对应的乱码就能出明文
            
            int totalShift = indexL + indexM + indexR;
            
            // 这里我们做一个 "加法加密" (输入+偏移=显示)
            // 这符合直觉：转轮是密钥，把你的输入“转化”成了屏幕上的字
            int finalIndex = (charIndex + totalShift) % 26;

            char outputChar = alphabet[finalIndex];
            screenText.text += outputChar;
        }
    }

    public void ClearScreen()
    {
        screenText.text = "";
    }

    // =================================================
    // 3. 开发者作弊工具 (帮你生成游戏道具！)
    // =================================================
    
    [Header("--- 谜题生成器 (填好后右键脚本执行) ---")]
    public string targetAnswer = "AWAKE"; // 你想让玩家解出的词
    public string settingL = "A";         // 左转轮设置
    public string settingM = "B";         // 中转轮设置
    public string settingR = "C";         // 右转轮设置

    [ContextMenu("生成纸条密文")]
    public void GenerateClue()
    {
        // 1. 获取目标转轮的数字值
        int valL = alphabet.IndexOf(settingL.ToUpper());
        int valM = alphabet.IndexOf(settingM.ToUpper());
        int valR = alphabet.IndexOf(settingR.ToUpper());
        int totalShift = valL + valM + valR;

        string cipherText = "";

        // 2. 逆向反推：为了显示 targetAnswer，玩家需要按哪个键？
        // 因为 PressKey 里的逻辑是: (输入 + 偏移) = 显示
        // 所以: 输入 = (显示 - 偏移)
        
        foreach (char c in targetAnswer.ToUpper())
        {
            int targetIndex = alphabet.IndexOf(c);
            
            // 逆向计算
            int inputIndex = targetIndex - totalShift;
            
            // 处理负数 (比如 -1 变成 25)
            while (inputIndex < 0) inputIndex += 26;
            
            cipherText += alphabet[inputIndex];
        }

        Debug.Log($"<color=cyan>=== 谜题生成结果 ===</color>");
        Debug.Log($"目标明文: <b>{targetAnswer}</b>");
        Debug.Log($"转轮设置: <b>{settingL} - {settingM} - {settingR}</b>");
        Debug.Log($"<color=yellow>请在纸条上写这串密文: {cipherText}</color>");
    }
}