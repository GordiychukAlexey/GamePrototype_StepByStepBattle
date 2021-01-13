using OverlapSystem;
using Selection;
using UnityEngine;

namespace Main {
	public class Unit : TileObject {
		public MeshRenderer     Body;
		public OverlapPivot     OverlapPivot;
		public SelectableObject SelectableObject;

		public Battlefield.Team            Team;
		public GameSettings.UnitParameters Parameters;


		private float health;

		public float Health{
			get => health;
			set{
				health                            = Mathf.Clamp(value, 0.0f, float.PositiveInfinity);
				UnitOverlapObject.HealthBar.Value = health / Parameters.InitialHealth;

				if (health <= 0.0f){
					Die();
				}
			}
		}

		private int stepsLeft;

		public int StepsLeft{
			get => stepsLeft;
			set{
				stepsLeft                            = value;
				UnitOverlapObject.StepsCounter.Value = stepsLeft;
			}
		}

		private bool isTurnEnded = false;

		public bool IsTurnEnded{
			get => isTurnEnded;
			set{
				isTurnEnded = value;
				UnitOverlapObject.IsEnableFullInfo(!isTurnEnded);
			}
		}

		public UnitOverlapObject UnitOverlapObject{ get; private set; }

		public void Initialize(GameSettings.UnitParameters parameters, BattlefieldTile tile, Battlefield.Team team){
			base.Initialize(tile);

			Team       = team;
			Parameters = parameters;
			Health     = parameters.InitialHealth;
//			StepsLeft  = parameters.MaxMovingDistance;
		}

		private void Awake(){
			UnitOverlapObject              = OverlapSystem.OverlapSystem.Instance.CreateUnitOverlapObject();
			UnitOverlapObject.OverlapPivot = OverlapPivot;
		}

//		[ContextMenu(nameof(Die))]
		public void Die(){
			{
				OverlapSystem.OverlapSystem.Instance.RemoveUnitOverlapObject(UnitOverlapObject);
				UnitOverlapObject = null;
			}

			{
				foreach (Collider collider in SelectableObject.Colliders){
					collider.gameObject.layer = 0;
				}

				Destroy(SelectableObject);
			}

			Team.Units.Remove(this);

			Tile.TileObject = null;

			transform.Rotate(new Vector3(1, 0, 0), -90.0f, Space.Self);
		}
	}
}