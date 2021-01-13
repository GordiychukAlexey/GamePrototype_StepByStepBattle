using UnityEngine;

namespace OverlapSystem {
	[RequireComponent(typeof(RectTransform))]
	public class UnitOverlapObject : MonoBehaviour {
		public bool         isVisible = true;
		public HealthBar    HealthBar;
		public StepsCounter StepsCounter;

		[HideInInspector]
		public OverlapPivot OverlapPivot;

		[HideInInspector]
		public RectTransform RectTransform;

		private void Reset(){
			RectTransform = GetComponent<RectTransform>();
		}


		private void Awake(){
			IsEnableFullInfo(false);
		}

		public void IsEnableFullInfo(bool value){
			StepsCounter.StepsLeftText.gameObject.SetActive(value);
		}
	}
}