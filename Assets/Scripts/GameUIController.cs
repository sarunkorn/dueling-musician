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

	public void Init(GameController controller)
	{
		foreach (var ui in _PlayerUis)
		{
			ui.Root.SetActive(false);
			ui.PerformanceBar.minValue = 0;
			ui.PerformanceBar.maxValue = controller.WinScore;
		}

		_startPanel.SetActive(true);
		controller.PlayerJoined += SetPlayerData;
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
		controller.ScoreUpdated += (score) =>
		{
			playerUI.PerformanceBar.value = score;
			playerUI.ScoreAnimator.SetBool("Updating", true);
			playerUI.ScoreAnimator.SetFloat("ScoreProgress", score/20f);
		};
	}
}
