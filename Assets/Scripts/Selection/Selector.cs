using System;
using Lean.Touch;
using UnityEngine;

namespace Selection {
	public class Selector : MonoBehaviour {
		private float maxRayDistance = 1000.0f;

		public event Action<SelectableObject> OnObjectSelected;
		public event Action OnSelectionReset;

		protected void OnEnable(){
			LeanTouch.OnFingerTap += HandleFingerTap;
		}

		protected void OnDisable(){
			LeanTouch.OnFingerTap -= HandleFingerTap;
		}


		private void HandleFingerTap(LeanFinger finger){
			Ray ray = MainCameraLink.Instance.MainCamera.ScreenPointToRay(finger.ScreenPosition);
			LayerMask unitsLayerMask = LayerMask.GetMask(Layers.Selectable);
			if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, unitsLayerMask)){
				SelectableObject selectedObject = SelectableObject.GetSelectableObjectByChildCollider(hit.collider);
				if (selectedObject != null){
					OnObjectSelected?.Invoke(selectedObject);
					return;
				}
			}

			OnSelectionReset?.Invoke();
		}
	}
}