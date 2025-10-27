using UnityEngine;

/// <summary>
/// 连接CatManager和CatAnimationController
/// 将CatManager的状态变化传递给动画控制器
/// </summary>
public class CatAnimationBridge : MonoBehaviour
{
    public CatAnimationController animationController;
    
    private CatState lastState = CatState.Idle;
    
    void Start()
    {
        if (animationController == null)
        {
            animationController = GetComponent<CatAnimationController>();
        }
    }
    
    void Update()
    {
        if (CatManager.Instance == null || animationController == null)
            return;
        
        CatState currentState = CatManager.Instance.currentState;
        
        if (currentState != lastState)
        {
            animationController.SetState(currentState);
            lastState = currentState;
        }
    }
}

