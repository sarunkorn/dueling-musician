using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public event Action Dash;
	public event Action Bumped;
	public event Action<PlayerController> GotMic;
	public event Action<PlayerController> LostMic;
	public event Action<float> ScoreUpdated;

	const string PlayerModelPrefix = "PlayerPrefab_";


	[Header("Movement")]
	[SerializeField] CharacterController _charController;
	[SerializeField] Transform _modelRoot;
	[SerializeField] float _playerSpeed = 10.0f;
	[SerializeField] float _dashDuration = 1f;
	[SerializeField] float _dashDurationMax = 2f;
	[SerializeField] float _dashMinDistance = 10.0f;
	[SerializeField] float _dashMaxDistance = 10.0f;
	[SerializeField] float _dashChargeDuration = 3.0f;
	[SerializeField] float _dashDelay = 5f;
	[SerializeField] float _bumpDistance = 2f;
	[SerializeField] float _bumpDuration = 0.5f;
	
	[Header("Arrow")]
	[SerializeField] GameObject _arrowRoot;
	[SerializeField] MeshRenderer _arrowMesh;
	[SerializeField] Material[] _arrowMatRef;
	[SerializeField] float _arrowStartZ = 0.25f;
	[SerializeField] float _arrowEndZ = 2.5f;


	[Header("Reference")] 
	[SerializeField] GameObject _micParticle;

	[Header("Data")]
	[SerializeField] float _micStealDelay = 2f;
	[SerializeField] float _scoreIncreaseRate = 0.5f;
	[SerializeField] float _respawnCooldown = 3f;

	public int PlayerIndex => _playerInput.playerIndex;
	public int TeamIndex => _teamIndex;
	public bool HasMic => _hasMic;
	public float Score => _score;
	public bool IsFell => _isFall;

	// movement
	bool _canMove = true;
	bool _canDash = true;
	bool _isFall = false;
	bool _isDashing = false;
	bool _isCharging = false;
	bool _isBumping = false;
	float _dashDistance;
	float _dashFinalDuration;
	float _lastDashChargeStartTime;
	float _lastDashTime;
	float _lastBumpTime;
	Vector3 _bumpDirection;
	Vector3 _spawnPos;
	
	// input
	PlayerInput _playerInput;
	Vector2 _moveInputValue;
	
	// data
	int _teamIndex = 0;
	bool _hasMic = false;
	float _lastMicTime;
	float _score = 0;
	float _respawnTime = 0;

	public void Init(PlayerInput playerInput, Vector3 spawnPosition)
	{
		_playerInput = playerInput;
		_spawnPos = spawnPosition;
		
		string playerName = "Player_" + PlayerIndex;
		gameObject.name = playerName;
		_playerInput.defaultActionMap = playerName;
		_micParticle.SetActive(false);
		_score = 0;

		_arrowMesh.material = _arrowMatRef[PlayerIndex];
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
		GotMic?.Invoke(this);
	}
	
	public void LostMicrophone()
	{
		Debug.Assert(_hasMic, "Should have mic.");
		_hasMic = false;
		_micParticle.SetActive(false);
		LostMic?.Invoke(this);
	}

	public void Bump(PlayerController other)
	{
		_isBumping = true;
		_lastBumpTime = Time.time;
		_bumpDirection = other.transform.forward;
		Bumped?.Invoke();
	}

	async Task SetupModel()
	{
		var loader = new AddressableResourceLoader();
		GameObject model = await loader.Load<GameObject>(PlayerModelPrefix + PlayerIndex);
		Instantiate(model, _modelRoot, false);
	}

	void Update()
	{
		if (_isFall && Time.time > _respawnTime)
		{
			Debug.Log($"Player {PlayerIndex} Respawn in... " + (Time.time - _respawnTime));
			Respawn();
		}

		if (_isCharging)
		{
			float progress = (Time.time - _lastDashChargeStartTime) / _dashChargeDuration;
			_dashDistance = Mathf.Lerp(_dashMinDistance, _dashMaxDistance, progress);
			_dashFinalDuration = Mathf.Lerp(_dashDuration, _dashDurationMax, progress);
			Debug.Log(_dashDistance);
			float arrowSize = Mathf.Lerp(_arrowStartZ, _arrowEndZ, progress);
			Vector3 scale = _arrowRoot.transform.localScale;
			scale.z = arrowSize;
			_arrowRoot.transform.localScale = scale;
			if (progress >= 1f)
			{
				StartDash();
			}
		}
		else if (_isDashing)
		{
			float progress = (Time.time - _lastDashTime) / _dashFinalDuration;
			_charController.Move(transform.forward * _dashDistance * Time.deltaTime);
			if (progress >= 1f)
			{
				_isDashing = false;
			}
		}else if(_isBumping)
		{
			float progress = (Time.time - _lastBumpTime) / _bumpDuration;
			_charController.Move(_bumpDirection * _bumpDistance * Time.deltaTime);
			if (progress >= 1f)
			{
				_isBumping = false;
				_bumpDirection = Vector3.zero;
			}
		}

		if (_isBumping || _isDashing)
		{
			return;
		}
		
		MoveLogic();
		
		// point
		if (HasMic)
		{
			_score += _scoreIncreaseRate * Time.deltaTime;
			ScoreUpdated?.Invoke(_score);
		}
	}

	void ForceSetPosition(Vector3 pos)
	{
		_charController.enabled = false;
		transform.SetPositionAndRotation(pos, Quaternion.identity);
		_charController.enabled = true;
	}

	void Fall()
	{
		_isFall = true;
		_canMove = false;
		_respawnTime = Time.time + _respawnCooldown;
		_moveInputValue = Vector2.zero;

		if (HasMic)
		{
			LostMicrophone();
		}
	}

	void Respawn()
	{
		ForceSetPosition(_spawnPos);
		_canMove = true;
		_isBumping = false;
		_isDashing = false;
		_isCharging = false;
		_isFall = false;
	}

	bool ValidatedMovement()
	{
		return _canMove && !_isFall;
	}

	void MoveLogic()
	{
		Vector3 move = new Vector3(_moveInputValue.x, 0, _moveInputValue.y);
		float moveSpeed = _playerSpeed;
		Vector3 moveVector = move * Time.deltaTime * moveSpeed;
		_charController.Move(moveVector - transform.up);

		if (move != Vector3.zero)
		{
			gameObject.transform.forward = move;
		}

		if (!_charController.isGrounded && !_isFall)
		{
			Fall();
		}
	}

	void StartDash()
	{
		Debug.Log("Dash");
		_isCharging = false;
		_isDashing = true;
		_lastDashTime = Time.time;
		_arrowRoot.SetActive(false);
		Dash?.Invoke();
	}

	bool IsDashCooldown()
	{
		return _lastDashTime > 0 && Time.time - _lastDashTime < _dashDelay;
	}
	
	void OnControllerColliderHit(ControllerColliderHit other)
	{
		if (!HasMic && other.gameObject.CompareTag("Microphone"))
		{
			GetMicrophone();
		}
		else if (other.gameObject.CompareTag("Player"))
		{
			var otherPlayer = other.gameObject.GetComponent<PlayerController>();
			if (_isDashing)
			{
				otherPlayer.Bump(this);
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
			|| !otherPlayer.IsStealProtected())
		{
			return;
		}
		
		otherPlayer.LostMicrophone();
		GetMicrophone();
	}

	#region Input Actions
	public void OnMove(InputAction.CallbackContext input)
	{
		if (!ValidatedMovement() && _isCharging)
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

		Debug.Log("input phase : " + input.phase);
		if (input.phase == InputActionPhase.Started && !_isCharging)
		{
			_isCharging = true;
			_moveInputValue = Vector2.zero;
			_lastDashChargeStartTime = Time.time;
			_arrowRoot.SetActive(true);
		}
		else if (input.phase == InputActionPhase.Canceled && _isCharging)
		{
			StartDash();
		}
	}
	#endregion

}
