using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] CharacterController _charController;
	[SerializeField] float _playerSpeed = 10.0f;
	[SerializeField] float _dashDuration = 1f;
	[SerializeField] float _dashDistance = 10.0f;
	[SerializeField] float _dashDelay = 5f;
	
	int _playerIndex = -1;
	
	PlayerInput _playerInput;
	Vector2 _moveInputValue;
	
	// movement
	bool _canMove = true;
	bool _canDash = true;
	bool _isDashing = false;
	float _lastDashTime;
	Vector3 _dashStartPos;
	Vector3 _dashEndPos;
	
	public void Init(int playerIndex, PlayerInput playerInput, Vector3 spawnPosition)
	{
		string playerName = "Player_" + playerIndex;
		gameObject.name = playerName;
		_playerIndex = playerIndex;
		_playerInput = playerInput;
		_playerInput.defaultActionMap = playerName;
		ForceSetPosition(spawnPosition);
	}

	void Update()
	{
		if (_isDashing)
		{
			float progress = (Time.time - _lastDashTime) / _dashDuration;
			transform.position = Vector3.Lerp(_dashStartPos, _dashEndPos, progress);
			if (progress >= 1f)
			{
				_isDashing = false;
			}
			return;
		}
		
		MoveLogic();
	}

	void ForceSetPosition(Vector3 pos)
	{
		_charController.enabled = false;
		transform.SetPositionAndRotation(pos, Quaternion.identity);
		_charController.enabled = true;
	}

	bool ValidatedMovement()
	{
		return _canMove && _playerInput.playerIndex == _playerIndex;
	}

	void MoveLogic()
	{
		Vector3 move = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
		float moveSpeed = _playerSpeed;
		_charController.Move(move * Time.deltaTime * moveSpeed);

		if (move != Vector3.zero)
		{
			gameObject.transform.forward = move;
		}
	}

	void StartDash()
	{
		Debug.Log("Dash");
		_isDashing = true;
		_dashStartPos = transform.position;
		_dashEndPos = _dashStartPos + (transform.forward * _dashDistance);
		_lastDashTime = Time.time;
	}

	bool IsDashCooldown()
	{
		return _lastDashTime > 0 && Time.time - _lastDashTime < _dashDelay;
	}

	#region Input Actions
	public void OnMove(InputAction.CallbackContext input)
	{
		if (!ValidatedMovement())
		{
			return;
		}
		_moveInputValue = input.ReadValue<Vector2>();
	}
	
	public void OnDash(InputAction.CallbackContext input)
	{
		if (!ValidatedMovement() || _isDashing || IsDashCooldown())
		{
			return;
		}
		
		StartDash();
	}
	#endregion

}
