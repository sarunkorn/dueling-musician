using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
	public event Action GameStart;
	public event Action GameEnd;
	
	[SerializeField] Transform[] _spawnPoints;

	public int NumberOfPlayer => _playerList.Count;
	
	List<PlayerController> _playerList = new List<PlayerController>();
	int _playerIndex = 0;

	void Update()
	{
		//TODO: point increase?
	}

	public bool CanStart()
	{
		return NumberOfPlayer == 2 || NumberOfPlayer == 4;
	}
	
	void InitPlayer(PlayerInput playerInput)
	{
		var controller = playerInput.GetComponent<PlayerController>();
		int currentIndex = _playerIndex;
		controller.Init(currentIndex, playerInput, _spawnPoints[currentIndex].position);
		//TODO: team up?
		controller.SetTeam(currentIndex);
		controller.GotMic += OnPlayerGotMic;
		controller.LostMic += OnPlayerLostMic;
		_playerList.Add(controller);
		Debug.Log("Created player " + currentIndex);
		_playerIndex++;
	}

	void OnPlayerGotMic(int playerIndex)
	{
		Debug.Log($"Player {playerIndex} got mic!");
	}

	void OnPlayerLostMic(int playerIndex)
	{
		Debug.Log($"Player {playerIndex} lost mic...");
	}

	public void PlayerJoinedEvent(PlayerInput playerInput)
	{
		Debug.Log("Player joined");
		InitPlayer(playerInput);
	}

	public void PlayerLeftEvent(PlayerInput playerInput)
	{
		Debug.Log("Player left");
	}
}
