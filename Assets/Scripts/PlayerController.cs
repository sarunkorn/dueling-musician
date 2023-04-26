using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public event Action Dash;
	public event Action<int> GotMic;
	public event Action<int> LostMic;


	[Header("Movement")]
	[SerializeField] CharacterController _charController;
	[SerializeField] Transform _modelRoot;
	[SerializeField] float _playerSpeed = 10.0f;
	[SerializeField] float _dashDuration = 1f;
	[SerializeField] float _dashDistance = 10.0f;
	[SerializeField] float _dashDelay = 5f;

	[Header("Reference")] 
	[SerializeField] GameObject _micParticle;
	
	[Header("Data")]
	[SerializeField] float _micStealDelay = 2f;

	public int PlayerIndex => _playerIndex;
	public int TeamIndex => _teamIndex;
	public bool HasMic => _hasMic;

	// movement
	bool _canMove = true;
	bool _canDash = true;
	bool _isDashing = false;
	float _lastDashTime;
	Vector3 _dashStartPos;
	Vector3 _dashEndPos;
	
	// input
	PlayerInput _playerInput;
	Vector2 _moveInputValue;
	
	// data
	int _playerIndex = -1;
	int _teamIndex = 0;
	bool _hasMic = false;
	float _lastMicTime;

	public void Init(int playerIndex, PlayerInput playerInput, Vector3 spawnPosition)
	{
		string playerName = "Player_" + playerIndex;
		gameObject.name = playerName;
		_playerIndex = playerIndex;
		_playerInput = playerInput;
		_playerInput.defaultActionMap = playerName;
		_micParticle.SetActive(false);
		ForceSetPosition(spawnPosition);
		SetupModel();
	}

	public void SetTeam(int teamIndex)
	{
		_teamIndex = teamIndex;
		//change color?
	}
	
	public void GetMicrophone()
	{
		Debug.Assert(!_hasMic, "Shouldn't have mic.");
		_hasMic = true;
		_lastMicTime = Time.time;
		_micParticle.SetActive(true);
		GotMic?.Invoke(_playerIndex);
	}
	
	public void LostMicrophone()
	{
		Debug.Assert(_hasMic, "Should have mic.");
		_hasMic = false;
		_micParticle.SetActive(false);
		LostMic?.Invoke(_playerIndex);
	}

	async Task SetupModel()
	{
		var loader = new AddressableResourceLoader();
		GameObject model = await loader.Load<GameObject>("PlayerPrefab_" + _playerIndex);
		Instantiate(model, _modelRoot, false);
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
		Dash?.Invoke();
	}

	bool IsDashCooldown()
	{
		return _lastDashTime > 0 && Time.time - _lastDashTime < _dashDelay;
	}
	
	void OnControllerColliderHit(ControllerColliderHit other)
	{
		// force stop dashing
		_isDashing = false;
		
		if (!HasMic)
		{
			if (other.gameObject.CompareTag("Microphone"))
			{
				GetMicrophone();
				Destroy(other.gameObject);
			}
			else if (other.gameObject.CompareTag("Player"))
			{
				var otherPlayer = other.gameObject.GetComponent<PlayerController>();
				TryStealMicrophone(otherPlayer);
			}
		}
	}

	bool IsStealProtected()
	{
		// give performance time
		return _lastMicTime > 0 && ((Time.time - _lastMicTime) > _micStealDelay);
	}

	void TryStealMicrophone(PlayerController otherPlayer)
	{
		if (otherPlayer == null 
			|| !otherPlayer.HasMic 
			|| otherPlayer.TeamIndex == _teamIndex 
			|| otherPlayer.IsStealProtected())
		{
			return;
		}
		
		otherPlayer.LostMicrophone();
		GetMicrophone();
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
