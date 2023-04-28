using System;
using System.Collections.Generic;
using SmileProject.Generic.Audio;
using SmileProject.SpaceInvader.Sounds;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	public static GameController Instance;
	public event Action GameStart;
	public event Action<int> GameEnd;
	
	public event Action PlayerReady;
	public event Action<PlayerController> PlayerJoined;

	readonly List<GameSoundKeys> PlayerBGM = new List<GameSoundKeys>()
	{
		GameSoundKeys.RockBGM,
		GameSoundKeys.DJBGM,
		GameSoundKeys.JazzBGM,
		GameSoundKeys.MaestroBGM,

	};
	
	[SerializeField] Transform[] _spawnPoints;
	[SerializeField] GameObject[] _lights;

	[SerializeField] float _winScore = 20;
	[SerializeField] float _joinerStartDelay = 0.5f;
	[SerializeField] float _readyWaiting = 2f;
	
	[SerializeField] GameObject _micRef;
	[SerializeField] GameObject _obstaclesRef;
	[SerializeField] GameObject _cameraTargetRef;
	[SerializeField] Animator _cameraAnimator;

	[SerializeField] GameUIController _uiController;
	[SerializeField] Animator _audienceAnimator;
	[SerializeField] PlayerInputManager _playerInputManager;
	
	public int NumberOfPlayer => _playerList.Count;
	public float WinScore => _winScore;
	public bool IsPlaying => _isPlaying;
	
	List<PlayerController> _playerList = new List<PlayerController>();
	PlayerController _performingPlayer;
	bool _isPlaying = false;
	bool _gameEnded = false;
	int _bgmPlayIndex = -1;

	bool _isReadyStage = false;
	float _readyTime = -1;

	void Start()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(this);
		}
		_obstaclesRef.SetActive(false);
		
		_uiController.Init(this);
		PlayBGM(GameSoundKeys.DefaultBGM);
	}

	void Update()
	{
		if (!_isPlaying && !_gameEnded)
		{
			if (_isReadyStage)
			{
				if (Time.time - _readyTime > _readyWaiting)
				{
					StartGame();
				}
			}

			foreach (var player in _playerList)
			{
				if(player.AllowInput)
					continue;

				bool isAllowInput = Time.time - player.JoinedTime > _joinerStartDelay;
				player.SetAllowInput(isAllowInput);
			}
		}
		CheckWinner();
	}

	async void PlayBGM(GameSoundKeys soundKey)
	{
		if (_bgmPlayIndex > -1)
		{
			AudioManager.Instance.StopSound(_bgmPlayIndex);
		}
		_bgmPlayIndex = await AudioManager.Instance.PlaySound(soundKey, true);
	}

	public bool CanStart()
	{
		return NumberOfPlayer > 1;
	}
	
	void InitPlayer(PlayerInput playerInput)
	{
		var controller = playerInput.GetComponent<PlayerController>();
		int spawnPointIndex = NumberOfPlayer;
		controller.Init(playerInput, _spawnPoints[spawnPointIndex].position);
		//TODO: team up?
		controller.SetTeam(playerInput.playerIndex);
		controller.SetAllowInput(false);
		controller.GotMic += OnPlayerGotMic;
		controller.LostMic += OnPlayerLostMic;
		controller.TryStart += OnPlayerTryStart;
		controller.Taunted += SetAudienceCheerPerforming;
		_playerList.Add(controller);
		
		PlayerJoined?.Invoke(controller);
		Debug.Log("Created player " + controller.PlayerIndex);
	}

	void Ready()
	{
		Debug.Log("Game Ready");
		
		_playerInputManager.DisableJoining();
		foreach (var player in _playerList)
		{
			player.MoveToStartPoint();
			player.AllowMove(false);
		}
		_obstaclesRef.SetActive(true);
		_micRef.SetActive(true);
		
		_readyTime = Time.time;
		_isReadyStage = true;
		PlayerReady?.Invoke();
	}

	void StartGame()
	{
		Debug.Log("Game Start");
		_isReadyStage = false;
		_isPlaying = true;
		
		foreach (var player in _playerList)
		{
			player.AllowMove(true);
		}
		
		GameStart?.Invoke();
		AudioManager.Instance.PlaySound(GameSoundKeys.GameStart);
	}

	void OnPlayerTryStart()
	{
		if (!_isPlaying)
		{
			if (_gameEnded)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				return;
			}

			if (CanStart())
			{
				Ready();
			}
		}
	}

	void OnPlayerGotMic(PlayerController player)
	{
		Debug.Log($"Player {player.PlayerIndex} got mic!");
		_cameraAnimator.SetInteger("PlayerInt", player.PlayerIndex);
		_cameraAnimator.SetTrigger("CutIn");
		_cameraTargetRef.transform.SetParent(player.gameObject.transform);
		_cameraTargetRef.transform.localPosition = new Vector3(0f,0f,0f);
		_performingPlayer = player;
		_lights[player.PlayerIndex].SetActive(true);
		_micRef.SetActive(false);
		AudioManager.Instance.PlaySound(GameSoundKeys.MicGrab);
		PlayBGM(PlayerBGM[player.PlayerIndex]);
		SetAudienceCheerMic(true);
	}

	void OnPlayerLostMic(PlayerController player)
	{
		_cameraTargetRef.transform.SetParent(null);
		_cameraTargetRef.transform.localPosition = new Vector3(0f,0f,0f);
		_cameraAnimator.SetTrigger("CutOut");
		Debug.Log($"Player {player.PlayerIndex} lost mic...");
		if (_performingPlayer == player)
		{
			_performingPlayer = null;
		}

		if (player.IsFell && _performingPlayer == null)
		{
			_micRef.SetActive(true);
			PlayBGM(GameSoundKeys.DefaultBGM);
			SetAudienceCheerMic(false);

		}
		_lights[player.PlayerIndex].SetActive(false);
	}

	void CheckWinner()
	{
		if (_isPlaying && _performingPlayer != null && _performingPlayer.Score > _winScore)
		{
			_isPlaying = false;
			_gameEnded = true;
			_obstaclesRef.SetActive(false);
			GameEnd?.Invoke(_performingPlayer.PlayerIndex);
			AudioManager.Instance.PlaySound(GameSoundKeys.Victory);
		}
	}

	void SetAudienceCheerMic(bool isCheer)
	{
		_audienceAnimator.SetBool("HasMic", isCheer);
	}

	void SetAudienceCheerPerforming(bool isCheer)
	{
		_audienceAnimator.SetBool("Performing", isCheer);
	}

	public void PlayerJoinedEvent(PlayerInput playerInput)
	{
		Debug.Log("Player joined");
		if (_micRef.activeSelf == true)
		{
			_micRef.SetActive(false);
		}
		InitPlayer(playerInput);
		AudioManager.Instance.PlaySound(GameSoundKeys.PlayerJoined);
	}

	public void PlayerLeftEvent(PlayerInput playerInput) 
	{
		int index = _playerList.FindIndex(o => o.PlayerIndex == playerInput.playerIndex);
		Destroy(_playerList[index].gameObject); 
		_playerList.RemoveAt(index);
		Debug.Log("Player left");
	}
}
