using UnityEngine;

namespace Main {
	public abstract class TileObject : MonoBehaviour {
		public BattlefieldTile Tile;

		protected void Initialize(BattlefieldTile tile){
			Tile = tile;
		}
	}
}