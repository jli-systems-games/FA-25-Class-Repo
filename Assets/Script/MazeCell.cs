using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _frontWall;

    [SerializeField]
    private GameObject _backWall;

    [SerializeField]
    private GameObject _unvisitedBlock;

    public bool IsVisited { get; private set; }

    public void Visit()
    {
        IsVisited = true;
        _unvisitedBlock.SetActive(false);
    }

    public void ClearLeftWall()
    {
        _leftWall.SetActive(false);
    }

    public void ClearRightWall()
    {
        _rightWall.SetActive(false);
    }

    public void ClearFrontWall()
    {
        _frontWall.SetActive(false);
    }

    public void ClearBackWall()
    {
        _backWall.SetActive(false);
    }
    public bool IsOpenLeft  => _leftWall  == null || !_leftWall.activeSelf;
    public bool IsOpenRight => _rightWall == null || !_rightWall.activeSelf;
    public bool IsOpenFront => _frontWall == null || !_frontWall.activeSelf;
    public bool IsOpenBack  => _backWall  == null || !_backWall.activeSelf;

    public Vector2Int GridPos
    {
        get
        {
            return new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.z)
            );
        }
    }
}
