using UnityEngine;

namespace Tools {
	public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour {
		protected static T instance;

		public static T Instance{
			get{
				if (instance == null){
					instance = (T) FindObjectOfType(typeof(T));

					if (instance == null){
						Debug.LogError("В сцене нужен экземпляр " + typeof(T) +
						               ", но он отсутствует.");
					}
				}

				return instance;
			}
		}
	}
}