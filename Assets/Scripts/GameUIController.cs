using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
	[Serializable]
	public class PlayerUI
	{
		public GameObject Root;
		public Text Name;
		public Slider PerformanceBar;
	}

	public PlayerUI[] _PlayerUis;

	public void Init(GameController controller)
	{
		foreach (var ui in _PlayerUis)
		{
			ui.Root.SetActive(false);
			ui.PerformanceBar.minValue = 0;
			ui.PerformanceBar.maxValue = controller.WinScore;
		}

		controller.PlayerJoined += SetPlayerData;
	}

	void SetPlayerData(PlayerController controller)
	{
		PlayerUI playerUI = _PlayerUis[controller.PlayerIndex];
		playerUI.Name.text = "Player " + (controller.PlayerIndex + 1);
		playerUI.PerformanceBar.value = controller.Score;
		playerUI.Root.SetActive(true);
		controller.ScoreUpdated += (score) =>
		{
			playerUI.PerformanceBar.value = score;
		};
	}
}
