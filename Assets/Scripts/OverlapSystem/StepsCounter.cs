using UnityEngine;
using UnityEngine.UI;

namespace OverlapSystem {
	public class StepsCounter : MonoBehaviour {
		public Text StepsLeftText;

		private int stepsLeft;

		public int Value{
			get => stepsLeft;
			set{
				stepsLeft          = value;
				StepsLeftText.text = stepsLeft.ToString();
			}
		}
	}
}