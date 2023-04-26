using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	[SerializeField] PlayerController _playerControllerPrefeb;
	[SerializeField] Transform[] _spawnPoints;
	
	//TODO: set from menu
	[SerializeField] int _numberOfPlayers;

	public int NumberOfPlayer => _numberOfPlayers;
	
	List<PlayerController> _playerList = new List<PlayerController>();

	
	//TODO: change to init later
	public void Start()
	{
		InitPlayers(_numberOfPlayers);
	}

	void InitPlayers(int numberOfPlayer)
	{
		_numberOfPlayers = numberOfPlayer;

		for (int i = 0; i < _numberOfPlayers; i++)
		{
			PlayerController controller = Instantiate(_playerControllerPrefeb, _spawnPoints[i], false);
			controller.Init(i);
			_playerList.Add(controller);
		}
	}
}
