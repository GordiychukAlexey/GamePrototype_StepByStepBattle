using UnityEngine;
using UnityEngine.UI;

namespace OverlapSystem {
	public class HealthBar : MonoBehaviour {
		public Image HealthValueImage;

		public float Value{
			get => HealthValueImage.fillAmount;
			set => HealthValueImage.fillAmount = Mathf.Clamp01(value);
		}
	}
}