using UnityEngine;

namespace Main {
	public class BattlefieldTile : MonoBehaviour {
		public Vector2Int Position{ get; private set; }
		public TileObject TileObject;

		public bool IsWalkable => TileObject == null;

		public void Initialize(Vector2Int position){
			Position = position;
		}
	}
}