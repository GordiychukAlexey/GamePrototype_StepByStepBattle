using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tools {
	public class BattlefieldTilesRandomizer : MonoBehaviour {
		private void Start(){
			RandomizeAll();
			Destroy(this);
		}

		[ContextMenu(nameof(ResetRandomize))]
		public void ResetRandomize(){
			ResetRandomize(new List<Transform>(){transform});
		}

		[ContextMenu(nameof(RandomizeAll))]
		public void RandomizeAll(){
			RandomizeAll(new List<Transform>(){transform});
		}

		public void ResetRandomize(List<Transform> transforms){
			foreach (Transform tr in transforms){
				tr.localPosition = new Vector3(
					tr.localPosition.x,
					-0.5f,
					tr.localPosition.z
				);
				tr.localRotation = Quaternion.identity;
			}
		}

		public void RandomizeHeight(List<Transform> transforms){
			foreach (Transform tr in transforms){
				tr.localPosition = tr.localPosition + ((0.12f * Random.value - 0.06f) * Vector3.up);
			}
		}

		public void RandomizeRotation(List<Transform> transforms){
			foreach (Transform tr in transforms){
				tr.localRotation = tr.localRotation * Quaternion.Euler((6f * Random.value - 3.0f) * Vector3.one);
			}
		}

		public void RandomizeAll(List<Transform> transforms){
			ResetRandomize(transforms);
			RandomizeHeight(transforms);
			RandomizeRotation(transforms);
		}
	}
}