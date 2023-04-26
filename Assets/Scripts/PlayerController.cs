using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] CharacterController _controller;
	[SerializeField] float _playerSpeed = 2.0f;
	
	[Header("Input")]
	[SerializeField] PlayerInput _playerInput;
	[SerializeField] InputAction _inputAction;

	Vector2 _moveInputValue;
	int _playerIndex = -1;

	void Start()
	{
		if (_inputAction == null)
		{
			_inputAction = new InputAction();
		}
	}

	void Update()
	{
		MoveLogic();
	}

	public void Init(int playerIndex)
	{
		_playerIndex = playerIndex;
		_playerInput.defaultActionMap = "Player" + playerIndex;
	}

	bool ValidatedMove()
	{
		return _playerInput.playerIndex == _playerIndex;
	}

	void MoveLogic()
	{
		Vector3 move = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
		_controller.Move(move * Time.deltaTime * _playerSpeed);

		if (move != Vector3.zero)
		{
			gameObject.transform.forward = move;
		}
	}

	#region Input Actions
	public void OnMove(InputAction.CallbackContext input)
	{
		if (!ValidatedMove())
		{
			return;
		}
		_moveInputValue = input.ReadValue<Vector2>();
	}
	#endregion

}
