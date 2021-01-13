using Tools;
using UnityEngine;

/// <summary>
/// Так как Camera.main медленный
/// </summary>
public class MainCameraLink : MonoBehaviourSingleton<MainCameraLink> {
	public Camera MainCamera;

	private void Reset(){
		MainCamera = Camera.main;
	}
}