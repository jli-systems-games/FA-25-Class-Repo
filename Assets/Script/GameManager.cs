using System.Collections;
using TMPro;
using UnityEngine;
// From Youtube, Bilibili, and Comments
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private TMP_Text p1ScoreText;//玩家1的分数
    [SerializeField] private TMP_Text p2ScoreText;//玩家2的分数
    [SerializeField] private TMP_Text p1WinText;//庆祝1
    [SerializeField] private TMP_Text p2WinText;//庆祝2
    [SerializeField] private int winningScore = 11;//获胜分数要求
    [SerializeField] private float serveDelay = 1.0f;//发球样式
    [SerializeField] private GameObject ballPrefab;//球
    [SerializeField] private Transform ballSpawnCenter;//初始发球点
    [SerializeField] private Transform ballSpawnLeft;//玩家1发球
    [SerializeField] private Transform ballSpawnRight;//玩家2发球
    [SerializeField] private float serveForceX = 4000f;//发球速度

    private int p1Score;
    private int p2Score;
    private bool gameOver;

    private void Awake()
    {
        //确保不重复
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        //归零
        p1Score = 0;
        p2Score = 0;
        gameOver = false;

        UpdateScoreUI();

        if (p1WinText) p1WinText.gameObject.SetActive(false);
        if (p2WinText) p2WinText.gameObject.SetActive(false);
        //准备时间
        StartCoroutine(ServeAfterDelay(0, 0.5f));
    }

    private void Update()
    {
        //R重开
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetMatch();
        }
    }

    private void UpdateScoreUI()
    {
        //记分板
        if (p1ScoreText) p1ScoreText.text = p1Score.ToString();
        if (p2ScoreText) p2ScoreText.text = p2Score.ToString();
    }

    public void GoalAtLeft()
    {
        //玩家2得分
        if (gameOver) return;

        p2Score++;
        UpdateScoreUI();
        CheckWinOrServe(2);
    }

    public void GoalAtRight()
    {
        //玩家1得分
        if (gameOver) return;

        p1Score++;
        UpdateScoreUI();
        CheckWinOrServe(1);
    }

    private void CheckWinOrServe(int scoringPlayer)
    {
        //获胜条件判断
        if (p1Score >= winningScore || p2Score >= winningScore)
        {
            gameOver = true;
            if (p1WinText || p2WinText)
            {
                if (p1Score >= winningScore && p1WinText) p1WinText.gameObject.SetActive(true);
                if (p2Score >= winningScore && p2WinText) p2WinText.gameObject.SetActive(true);
            }
            return;
        }
        //发球延时
        StartCoroutine(ServeAfterDelay(scoringPlayer, serveDelay));
    }

    private IEnumerator ServeAfterDelay(int servingPlayer, float delay)
    {
        //等待+发球
        yield return new WaitForSeconds(delay);
        SpawnAndServe(servingPlayer);
    }

    private void SpawnAndServe(int servingPlayer)
    {
        //生成球+第一次发球，从中场检测初始点
        Vector3 spawnPos;

        if (servingPlayer == 0)
        {
            spawnPos = ballSpawnCenter ? ballSpawnCenter.position : Vector3.zero;
        }
        else if (servingPlayer == 1)
        {
            spawnPos = ballSpawnLeft ? ballSpawnLeft.position : Vector3.zero;
        }
        else
        {
            spawnPos = ballSpawnRight ? ballSpawnRight.position : Vector3.zero;
        }

        GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        var rb = ball.GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;

        if (servingPlayer == 0)
        {
            //0-0从中场随机发
            int dirSign = Random.Range(0, 2) == 0 ? -1 : 1;
            rb.AddForce(new Vector2(serveForceX * dirSign, 0));
        }
        else if (servingPlayer == 1)
        {
            //玩家1发
            rb.AddForce(new Vector2(serveForceX, 0));
        }
        else if (servingPlayer == 2)
        {
            //玩家2发
            rb.AddForce(new Vector2(-serveForceX, 0));
        }
    }

    public void ResetMatch()
    {
        //重开
        p1Score = 0;
        p2Score = 0;
        gameOver = false;
        UpdateScoreUI();
        if (p1WinText) p1WinText.gameObject.SetActive(false);
        if (p2WinText) p2WinText.gameObject.SetActive(false);
        //确保场上没有球，直接清除，后发球
        var ball = GameObject.FindGameObjectWithTag("Ball");
        if (ball) Destroy(ball);
        StartCoroutine(ServeAfterDelay(0, 0.5f));
    }
}