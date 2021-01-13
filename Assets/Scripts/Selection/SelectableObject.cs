using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Selection {
	public class SelectableObject : MonoBehaviour {
		public List<Collider> Colliders = new List<Collider>();

		[Serializable]
		public class SelectionChangedEvent : UnityEvent<bool> {
		}

		public SelectionChangedEvent OnSelectionChanged = new SelectionChangedEvent();

		private bool isSelected = false;

		public bool IsSelected{
			get => isSelected;
			set{
				if (value != isSelected){
					isSelected = value;
					OnSelectionChanged?.Invoke(value);
				}
			}
		}

		private void Reset(){
			Colliders = GetComponentsInChildren<Collider>().ToList();
		}

		private void OnValidate(){
			for (var i = Colliders.Count - 1; i >= 0; i--){
				Collider coll = Colliders[i];
				if (coll == null){
					Colliders.RemoveAt(i);
				}
			}
		}

		private void Awake(){
			foreach (var col in Colliders.Where(col => col != null)){
				col.gameObject.layer = LayerMask.NameToLayer(Layers.Selectable);
			}
		}

		public static SelectableObject GetSelectableObjectByChildCollider(Collider childCollider){
			GameObject currentSearch = childCollider.gameObject;
			while (currentSearch != null){
				SelectableObject selectableObject = currentSearch.GetComponent<SelectableObject>();
				if (selectableObject != null){
					return selectableObject;
				}

				Transform parent = currentSearch.transform.parent;
				currentSearch = parent != null ? parent.gameObject : null;
			}

			return null;
		}
	}
}