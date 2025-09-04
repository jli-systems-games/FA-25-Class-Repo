using UnityEngine;
// From Youtube, Bilibili, and Comments
public class Goal : MonoBehaviour
{
    public bool isRightGoal = true; //分辨球门左右

    private void Reset()
    {
        //box碰撞框设置为isTrigger，以防在unity中找半天
        var bc = GetComponent<BoxCollider2D>();
        bc.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //确保只被Ball触发
        if (!other.CompareTag("Ball")) return;
        //进门后自动销毁
        Destroy(other.gameObject);
        //判断那一方得分
        if (isRightGoal)
            GameManager.Instance.GoalAtRight();
        else
            GameManager.Instance.GoalAtLeft();
    }
}