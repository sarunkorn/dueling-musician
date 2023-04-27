using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
	
	[SerializeField] GameObject _micRef;

	[SerializeField] GameUIController _uiController;
	[SerializeField] AudioManager _audioManager;
	
	public int NumberOfPlayer => _playerList.Count;
	public float WinScore => _winScore;
	
	List<PlayerController> _playerList = new List<PlayerController>();
	PlayerController _performingPlayer;
	bool _isPlaying = false;
	bool _gameEnded = false;
	int _bgmPlayIndex = -1;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(this);
		}
		
		_uiController.Init(this);
		
	}

	void Update()
	{
		if (!_isPlaying && !_gameEnded)
		{
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
		controller.AllowMove(false);
		controller.SetAllowInput(false);
		controller.GotMic += OnPlayerGotMic;
		controller.LostMic += OnPlayerLostMic;
		controller.TryStart += OnPlayerTryStart;
		_playerList.Add(controller);
		
		PlayerJoined?.Invoke(controller);
		Debug.Log("Created player " + controller.PlayerIndex);
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
				Debug.Log("Game Start");
				_isPlaying = true;
				foreach (var player in _playerList)
				{
					player.AllowMove(true);
				}
				GameStart?.Invoke();
				AudioManager.Instance.PlaySound(GameSoundKeys.GameStart);
			}
		}
	}

	void OnPlayerGotMic(PlayerController player)
	{
		Debug.Log($"Player {player.PlayerIndex} got mic!");
		_performingPlayer = player;
		_lights[player.PlayerIndex].SetActive(true);
		_micRef.SetActive(false);
		AudioManager.Instance.PlaySound(GameSoundKeys.MicGrab);
		PlayBGM(PlayerBGM[player.PlayerIndex]);

	}

	void OnPlayerLostMic(PlayerController player)
	{
		Debug.Log($"Player {player.PlayerIndex} lost mic...");
		if (_performingPlayer == player)
		{
			_performingPlayer = null;
		}

		if (player.IsFell && _performingPlayer == null)
		{
			_micRef.SetActive(true);
			PlayBGM(GameSoundKeys.DefaultBGM);
		}
		_lights[player.PlayerIndex].SetActive(false);
	}

	void CheckWinner()
	{
		if (_isPlaying && _performingPlayer != null && _performingPlayer.Score > _winScore)
		{
			_isPlaying = false;
			_gameEnded = true;
			GameEnd?.Invoke(_performingPlayer.PlayerIndex);
			AudioManager.Instance.PlaySound(GameSoundKeys.Victory);
		}
	}

	public void PlayerJoinedEvent(PlayerInput playerInput)
	{
		Debug.Log("Player joined");
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
