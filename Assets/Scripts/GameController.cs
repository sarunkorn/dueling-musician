using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
	[SerializeField] PlayerController _playerControllerPrefeb;
	[SerializeField] Transform[] _spawnPoints;

	public int NumberOfPlayer => _playerList.Count;
	
	List<PlayerController> _playerList = new List<PlayerController>();

	int playerIndex = 0;

	void InitPlayer(PlayerInput playerInput)
	{
		var controller = playerInput.GetComponent<PlayerController>();
		controller.Init(playerIndex, playerInput, _spawnPoints[playerIndex].position);
		_playerList.Add(controller);
		Debug.Log("Created player " + playerIndex);
		playerIndex++;
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
