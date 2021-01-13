using System;
using UnityEngine;
using UnityEngine.UI;

namespace Main {
	public class BattleUI : MonoBehaviour {
		public event Action OnTurnEnded;

		public Image         CurrentTeamImage;
		public Button        EndTurnButton;
		public RectTransform WinnerScreen;
		public Image         WinnerTeamImage;

		private void OnEnable(){
			EndTurnButton.onClick.AddListener(() => OnTurnEnded?.Invoke());
		}

		public void ShowWinnerScreen(Color winnerColor){
			WinnerScreen.gameObject.SetActive(true);
			WinnerTeamImage.color = winnerColor;
		}
	}
}