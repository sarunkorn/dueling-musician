using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	public static GameController Instance;
	public event Action GameStart;
	public event Action<int> GameEnd;
	public event Action<PlayerController> PlayerJoined;
	
	[SerializeField] Transform[] _spawnPoints;
	[SerializeField] GameObject[] _lights;

	[SerializeField] float _winScore = 20;
	
	[SerializeField] GameObject _micRef;

	[SerializeField] GameUIController _uiController;
	
	public int NumberOfPlayer => _playerList.Count;
	public float WinScore => _winScore;
	
	List<PlayerController> _playerList = new List<PlayerController>();
	PlayerController _performingPlayer;
	bool _isPlaying = true;
	bool _gameEnded = false;

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
		CheckWinner();
	}

	public bool CanStart()
	{
		return NumberOfPlayer == 2 || NumberOfPlayer == 4;
	}
	
	void InitPlayer(PlayerInput playerInput)
	{
		var controller = playerInput.GetComponent<PlayerController>();
		int spawnPointIndex = NumberOfPlayer;
		controller.Init(playerInput, _spawnPoints[spawnPointIndex].position);
		//TODO: team up?
		controller.SetTeam(playerInput.playerIndex);
		controller.GotMic += OnPlayerGotMic;
		controller.LostMic += OnPlayerLostMic;
		controller.TryStart += OnPlayerTryStart;
		_playerList.Add(controller);
		
		PlayerJoined?.Invoke(controller);
		Debug.Log("Created player " + controller.PlayerIndex);
	}

	void OnPlayerTryStart()
	{
		if (!_isPlaying && _gameEnded)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	void OnPlayerGotMic(PlayerController player)
	{
		Debug.Log($"Player {player.PlayerIndex} got mic!");
		_performingPlayer = player;
		_lights[player.PlayerIndex].SetActive(true);
		_micRef.SetActive(false);
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
		}
	}

	public void PlayerJoinedEvent(PlayerInput playerInput)
	{
		Debug.Log("Player joined");
		InitPlayer(playerInput);
	}

	public void PlayerLeftEvent(PlayerInput playerInput)
	{
		int index = _playerList.FindIndex(o => o.PlayerIndex == playerInput.playerIndex);
		Destroy(_playerList[index].gameObject);
		_playerList.RemoveAt(index);
		Debug.Log("Player left");
	}
}
