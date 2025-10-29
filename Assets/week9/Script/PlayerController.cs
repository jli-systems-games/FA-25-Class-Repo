using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _screenBorder;

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private Button _replayButton;
    [SerializeField] private Image _gameOverImage;
    [SerializeField] private TextMeshProUGUI _finalScoreText; // 👈 추가됨

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Camera _camera;
    private Animator _animator;
    private bool _isDead = false;

    private ScoreController _scoreController; // 👈 점수 가져오기용

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _scoreController = FindObjectOfType<ScoreController>(); // 👈 씬에 있는 점수 컨트롤러 참조

        if (_gameOverText != null)
            _gameOverText.gameObject.SetActive(false);

        if (_replayButton != null)
        {
            _replayButton.gameObject.SetActive(false);
            _replayButton.onClick.AddListener(OnReplayButtonClicked);
        }

        if (_gameOverImage != null)
            _gameOverImage.gameObject.SetActive(false);

        if (_finalScoreText != null)
            _finalScoreText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        SetPlayerVelocity();
        RotateInDirectionOfInput();
        SetAnimation();
    }

    private void SetAnimation()
    {
        bool isMoving = _movementInput != Vector2.zero;
        _animator.SetBool("IsMoving", isMoving);
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
            _smoothedMovementInput,
            _movementInput,
            ref _movementInputSmoothVelocity,
            0.1f);

        _rigidbody.linearVelocity = _smoothedMovementInput * _speed;
        PreventPlayerGoingOffScreen();
    }

    private void PreventPlayerGoingOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < _screenBorder && _rigidbody.linearVelocity.x < 0) ||
            (screenPosition.x > _camera.pixelWidth - _screenBorder && _rigidbody.linearVelocity.x > 0))
        {
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
        }

        if ((screenPosition.y < _screenBorder && _rigidbody.linearVelocity.y < 0) ||
            (screenPosition.y > _camera.pixelHeight - _screenBorder && _rigidbody.linearVelocity.y > 0))
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
        }
    }

    private void RotateInDirectionOfInput()
    {
        if (_movementInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(_smoothedMovementInput.y, _smoothedMovementInput.x) * Mathf.Rad2Deg - 90f;
            float newAngle = Mathf.MoveTowardsAngle(_rigidbody.rotation, targetAngle, _rotationSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(newAngle);
        }
    }

    private void OnMove(InputValue inputValue)
    {
        _movementInput = inputValue.Get<Vector2>();
    }

    // 🟥 플레이어 사망 시 호출
    public void OnPlayerDeath()
    {
        _isDead = true;
        _rigidbody.linearVelocity = Vector2.zero;
        _animator.SetTrigger("isDead");

        if (_gameOverText != null)
            _gameOverText.gameObject.SetActive(true);

        if (_replayButton != null)
            _replayButton.gameObject.SetActive(true);

        if (_gameOverImage != null)
            _gameOverImage.gameObject.SetActive(true);

        // 점수 표시 부분 👇
        if (_finalScoreText != null && _scoreController != null)
        {
            _finalScoreText.text = $"Score : {_scoreController.Score}";
            _finalScoreText.gameObject.SetActive(true);
        }
    }

    // 🟩 REPLAY 버튼 클릭 시 씬 재시작
    private void OnReplayButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
