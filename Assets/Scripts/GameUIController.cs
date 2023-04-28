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
		public Animator ScoreAnimator;
	}

	public PlayerUI[] _PlayerUis;
	
	[SerializeField] GameObject _victoryPanel;
	[SerializeField] GameObject _startPanel;
	[SerializeField] GameObject gameTitle;

	GameController _gameController;

	public void Init(GameController controller)
	{
		_gameController = controller;
		foreach (var ui in _PlayerUis)
		{
			ui.Root.SetActive(false);
			ui.PerformanceBar.minValue = 0;
			ui.PerformanceBar.maxValue = controller.WinScore;
		}

		_startPanel.SetActive(true);
		controller.PlayerJoined += (controller)=>
		{
			gameTitle.SetActive(false);
			SetPlayerData(controller);
		};
		controller.GameStart += () => { _startPanel.SetActive(false); };
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
		controller.GotMic += (_) =>
		{
			playerUI.ScoreAnimator.SetBool("Updating", true);
		};
		controller.LostMic += (_) =>
		{
			playerUI.ScoreAnimator.SetBool("Updating", false);
		};
		controller.ScoreUpdated += (score) =>
		{
			playerUI.PerformanceBar.value = score;
			playerUI.ScoreAnimator.SetFloat("ScoreProgress", score/_gameController.WinScore);
		};
	}
}
