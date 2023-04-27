using System;
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
	[SerializeField] float _dashDistance = 10.0f;
	[SerializeField] float _dashDelay = 5f;
	[SerializeField] float _bumpDistance = 2f;
	[SerializeField] float _bumpDuration = 0.5f;

	[Header("Reference")] 
	[SerializeField] GameObject _micParticle;
	
	[Header("Data")]
	[SerializeField] float _micStealDelay = 2f;
	[SerializeField] float _scoreIncreaseRate = 0.5f;

	public int PlayerIndex => _playerInput.playerIndex;
	public int TeamIndex => _teamIndex;
	public bool HasMic => _hasMic;
	public float Score => _score;

	// movement
	bool _canMove = true;
	bool _canDash = true;
	bool _isDashing = false;
	bool _isBumping = false;
	float _lastDashTime;
	float _lastBumpTime;
	Vector3 _bumpDirection;
	
	// input
	PlayerInput _playerInput;
	Vector2 _moveInputValue;
	
	// data
	int _teamIndex = 0;
	bool _hasMic = false;
	float _lastMicTime;
	float _score = 0;

	public void Init(PlayerInput playerInput, Vector3 spawnPosition)
	{
		_playerInput = playerInput;
		
		string playerName = "Player_" + PlayerIndex;
		gameObject.name = playerName;
		_playerInput.defaultActionMap = playerName;
		_micParticle.SetActive(false);
		_score = 0;
		
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
		// moving
		if (_isDashing)
		{
			float progress = (Time.time - _lastDashTime) / _dashDuration;
			_charController.Move(transform.forward * _dashDistance * Time.deltaTime);
			// transform.position = Vector3.Lerp(_dashStartPos, _dashEndPos, progress);
			if (progress >= 1f)
			{
				_isDashing = false;
			}
		}

		if (_isBumping)
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

	bool ValidatedMovement()
	{
		return _canMove;
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
		_lastDashTime = Time.time;
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
			Destroy(other.gameObject);
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
		
		// force stop dashing
		_isDashing = false;
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
