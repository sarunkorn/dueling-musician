using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	public event Action TryStart;
	public event Action<bool> Taunted;
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
	[SerializeField] float _tauntScoreIncreaseRate = 2f;
	[SerializeField] float _respawnCooldown = 3f;

	public int PlayerIndex => _playerInput.playerIndex;
	public int TeamIndex => _teamIndex;
	public bool HasMic => _hasMic;
	public float Score => _score;
	public bool IsFell => _isFall;
	public bool AllowInput => _allowInput;
	public float JoinedTime => _joinedTime;

	// movement
	bool _canMove = true;
	bool _allowMove = false;
	bool _allowInput = false;
	bool _isFall = false;
	bool _isDashing = false;
	bool _isCharging = false;
	bool _isBumping = false;
	bool _holdCharging = false;
	bool _isTaunting = false;
	float _dashDistance;
	float _dashFinalDuration;
	float _lastDashChargeStartTime;
	float _lastDashTime;
	float _lastBumpTime;
	float _joinedTime;
	Vector3 _bumpDirection;
	Vector3 _spawnPos;
	
	// input
	PlayerInput _playerInput;
	Vector2 _moveInputValue;
	Animator _animator;
	
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
		_joinedTime = Time.time;
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

	public void AllowMove(bool allowMove)
	{
		_allowMove = allowMove;
	}
	
	public void SetAllowInput(bool allowInput)
	{
		_allowInput = allowInput;
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
		
		_isDashing = false;
		CancelCharge();
		StopTaunt();
		Bumped?.Invoke();
	}

	void StartCharge()
	{
		_isCharging = true;
		_lastDashChargeStartTime = Time.time;
		_arrowRoot.SetActive(true);
	}

	void CancelCharge()
	{
		_isCharging = false;
		_arrowRoot.SetActive(false);
	}

	async Task SetupModel()
	{
		var loader = new AddressableResourceLoader();
		GameObject model = await loader.Load<GameObject>(PlayerModelPrefix + PlayerIndex);
		var modelObj = Instantiate(model, _modelRoot, false);
		_animator = modelObj.GetComponent<Animator>();
	}

	void Update()
	{
		if (_holdCharging && !_isCharging && IsAllowDash())
		{
			StartCharge();
		}
		
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
			float increaseRate = _isTaunting ? _tauntScoreIncreaseRate : _scoreIncreaseRate;
			_score += increaseRate * Time.deltaTime;
			ScoreUpdated?.Invoke(_score);
		}
	}

	void ForceSetPosition(Vector3 pos)
	{
		_charController.enabled = false;
		transform.SetPositionAndRotation(pos, Quaternion.Euler(0,180,0));
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
		return _allowMove && _canMove && !_isFall && !_isBumping;
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
				if (otherPlayer._isDashing)
				{
					Bump(otherPlayer);
				}
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

	void Taunt()
	{
		_isTaunting = true;
		_moveInputValue = Vector2.zero;
		_canMove = false;
		_animator.SetBool("Taunt", true);
		Taunted?.Invoke(true);
	}

	void StopTaunt()
	{
		_isTaunting = false;
		_canMove = true;
		_animator.SetBool("Taunt", false);
		Taunted?.Invoke(false);
	}

	bool IsAllowDash()
	{
		bool isValidated = ValidatedMovement();
		return isValidated && !_isDashing && !IsDashCooldown();
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
		if (!IsAllowDash())
		{
			if (input.phase == InputActionPhase.Started)
			{
				_holdCharging = true;
			}
			else if (input.phase == InputActionPhase.Canceled)
			{
				_holdCharging = false;
			}
			return;
		}

		Debug.Log("input phase : " + input.phase);
		if (input.phase == InputActionPhase.Started && !_isCharging)
		{
			StartCharge();
		}
		else if (input.phase == InputActionPhase.Canceled && _isCharging)
		{
			_holdCharging = false;
			StartDash();
		}
	}
	
	public void OnStart(InputAction.CallbackContext input)
	{
		if (!_allowInput)
			return;
		
		TryStart?.Invoke();
	}
	
	public void OnAction(InputAction.CallbackContext input)
	{
		if (!_isTaunting && input.phase == InputActionPhase.Started)
		{
			Taunt();
		}
		else if(_isTaunting && input.phase == InputActionPhase.Canceled)
		{
			StopTaunt();
		}
	}
	#endregion

}
