using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZombieMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    private Rigidbody2D _rigidbody;
    private PlayerAwareController _playerAwareController;
    private Vector2 _targetDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwareController = GetComponent<PlayerAwareController>();
    }

    private void FixedUpdate()
    {
        UpgradeTargetDirection();
        RotateTowardsTarget();
        SetVelocity();
    }

    private void UpgradeTargetDirection()
    {
        if(_playerAwareController.AwareOfPlayer)
        {
            _targetDirection = _playerAwareController.DirectionToPlayer;
        }
        else
        {
            _targetDirection = Vector2.zero;
        }
    }

    private void RotateTowardsTarget()
    {
        if (_targetDirection == Vector2.zero)
        { return; }

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        _rigidbody.SetRotation(rotation);
    }

    private void SetVelocity()
    {
        if(_targetDirection==Vector2.zero)
        {
            _rigidbody.linearVelocity = transform.up * _speed;
        }
    }
}
