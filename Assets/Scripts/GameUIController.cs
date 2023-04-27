using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
	[Serializable]
	public class PlayerUI
	{
		[SerializeField] Text _name;
		[SerializeField] Slider _performanceBar;
	}
}
