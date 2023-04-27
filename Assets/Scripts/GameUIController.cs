using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
	[Serializable]
	public class PlayerUI
	{
		public GameObject Root;
		public Slider PerformanceBar;
		public GameObject VictoryModel;
	}

	public PlayerUI[] _PlayerUis;
	
	[SerializeField] GameObject _victoryPanel;

	public void Init(GameController controller)
	{
		foreach (var ui in _PlayerUis)
		{
			ui.Root.SetActive(false);
			ui.PerformanceBar.minValue = 0;
			ui.PerformanceBar.maxValue = controller.WinScore;
		}

		controller.PlayerJoined += SetPlayerData;
		controller.GameEnd += ShowVictory;
	}

	void ShowVictory(int playerIndex)
	{
		_victoryPanel.SetActive(true);
		_PlayerUis[playerIndex].VictoryModel.SetActive(true);
	}

	void SetPlayerData(PlayerController controller)
	{
		PlayerUI playerUI = _PlayerUis[controller.PlayerIndex];
		playerUI.PerformanceBar.value = controller.Score;
		playerUI.Root.SetActive(true);
		controller.ScoreUpdated += (score) =>
		{
			playerUI.PerformanceBar.value = score;
		};
	}
}
