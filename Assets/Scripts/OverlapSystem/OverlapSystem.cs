using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace OverlapSystem {
	[RequireComponent(typeof(RectTransform))]
	public class OverlapSystem : MonoBehaviourSingleton<OverlapSystem> {
		public UnitOverlapObject UnitOverlapObjectPrefab;

		private readonly HashSet<UnitOverlapObject> unitOverlapObjects = new HashSet<UnitOverlapObject>();

		public UnitOverlapObject CreateUnitOverlapObject(){
			UnitOverlapObject newUnitOverlapObject = Instantiate(UnitOverlapObjectPrefab, transform);
			unitOverlapObjects.Add(newUnitOverlapObject);
			return newUnitOverlapObject;
		}

		public void RemoveUnitOverlapObject(UnitOverlapObject unitOverlapObject){
			unitOverlapObjects.Remove(unitOverlapObject);
			Destroy(unitOverlapObject.gameObject);
		}

		private void Update(){
			foreach (UnitOverlapObject unitOverlapObject in unitOverlapObjects){
				if (unitOverlapObject.gameObject.activeInHierarchy != unitOverlapObject.isVisible){
					unitOverlapObject.gameObject.SetActive(unitOverlapObject.isVisible);
				}

				if (unitOverlapObject.isVisible){
					unitOverlapObject.RectTransform.position =
						MainCameraLink.Instance.MainCamera.WorldToScreenPoint(unitOverlapObject.OverlapPivot.transform.position);
				}
			}
		}
	}
}