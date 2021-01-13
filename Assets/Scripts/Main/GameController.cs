using System.Linq;
using Selection;
using Tools;
using UnityEngine;

namespace Main {
	public class GameController : MonoBehaviourSingleton<GameController> {
		public Battlefield Battlefield;
		public Selector    Selector;
		public BattleUI    BattleUI;


		public Battlefield.Team CurrentPlayedTeam;

		private void OnEnable(){
			Selector.OnObjectSelected += ObjectSelected;
			Selector.OnSelectionReset += () => ObjectSelected(null);

			BattleUI.OnTurnEnded += GoToNextTeam;
		}


		private Unit _selectedUnit;

		private Unit selectedUnit{
			get => _selectedUnit;
			set{
				if (_selectedUnit){
					_selectedUnit.GetComponent<SelectableObject>().IsSelected = false;
				}

				_selectedUnit = value;
				if (_selectedUnit){
					_selectedUnit.GetComponent<SelectableObject>().IsSelected = true;
				}
			}
		}

		public void ObjectSelected(SelectableObject value){
			if (value == null){
				selectedUnit = null;
				return;
			}

			if (value.TryGetComponent<Unit>(out Unit unit)){
				if (unit.Team == CurrentPlayedTeam){
					if (unit.IsTurnEnded){
						StageEnded();
					} else{
						selectedUnit = unit;
					}

					return;
				} else{
					if (selectedUnit){
						if (TryAttack(selectedUnit, unit)){
							StageEnded();
						}

						selectedUnit = null;
						return;
					}
				}
			}

			if (value.TryGetComponent<BattlefieldTile>(out BattlefieldTile tile)){
				if (tile.TileObject == null){
					if (selectedUnit){
						if (!TryMove(selectedUnit, tile.Position)){
							selectedUnit = null;
						}

						return;
					}
				}
			}

			selectedUnit = null;
		}


		private bool TryMove(Unit unit, Vector2Int position){
			if (unit.IsTurnEnded){
				return false;
			}

			Vector2Int[] path = Battlefield.FoundPath(unit.Tile.Position, position);
//			Debug.Log(string.Join(", ", path));
			if (path.Length - 1 <= unit.StepsLeft){
				Battlefield.MoveUnit(path);
				return true;
			}

			return false;
		}

		private bool TryAttack(Unit attacker, Unit attacked){
			if (attacker.IsTurnEnded){
				return false;
			}

			Vector2Int asd = attacked.Tile.Position - attacker.Tile.Position;
			if (Mathf.Abs(asd.x) <= 1 && Mathf.Abs(asd.y) <= 1){
				Battlefield.Attack(attacker, attacked);
				return true;
			}

			return false;
		}


		private void Start(){
			Battlefield.CreateBattlefield(GameSettings.Instance.BattleSettings.BattlefieldSize);

			Battlefield.transform.position = (-(GameSettings.Instance.BattleSettings.BattlefieldSize.x / 2.0f) + 0.5f) * Vector3.right
			                                 + (-(GameSettings.Instance.BattleSettings.BattlefieldSize.y / 2.0f) + 0.5f) * Vector3.forward;

			foreach (GameSettings.InitialTeamSettings teamSettings in GameSettings.Instance.BattleSettings.TeamsParameters){
				foreach (GameSettings.InitialUnitSettings unitSettings in teamSettings.UnitsParameters){
					GameSettings.UnitParameters unitParameters = GameSettings.Instance.GetUnitParameters(unitSettings.UnitName);
					Battlefield.SpawnUnit(unitParameters, unitSettings.Position, teamSettings);
				}
			}

			Battlefield.Team firstTurnTeam = Battlefield.GetOrCreateTeam(
				GameSettings.Instance.GetTeamParameters(
					GameSettings.Instance.BattleSettings.FirstTurnTeamName));

			PrepareTeam(firstTurnTeam);
		}

		private void StageEnded(){
			if (Battlefield.Teams.Count(team => team.Units.Count > 0) == 1){
				Battlefield.Team winTeam = Battlefield.Teams.Find(team => team.Units.Count > 0);
				BattleUI.ShowWinnerScreen(winTeam.InitialTeamSettings.TeamColor);
			}

			if (CurrentPlayedTeam.Units.All(unit => unit.IsTurnEnded)){
				GoToNextTeam();
			}
		}

		private void GoToNextTeam(){
			int nextTeamIndex = Battlefield.Teams.IndexOf(CurrentPlayedTeam) + 1;
			nextTeamIndex = nextTeamIndex % Battlefield.Teams.Count;
			PrepareTeam(Battlefield.Teams[nextTeamIndex]);
		}

		private void PrepareTeam(Battlefield.Team team){
			if (CurrentPlayedTeam != null){
				foreach (Unit unit in CurrentPlayedTeam.Units){
					unit.UnitOverlapObject.IsEnableFullInfo(false);
				}
			}

			selectedUnit = null;

			CurrentPlayedTeam = team;
			foreach (Unit unit in team.Units){
				unit.IsTurnEnded = false;
				unit.StepsLeft   = unit.Parameters.MaxMovingDistance;
				unit.UnitOverlapObject.IsEnableFullInfo(true);
			}

			BattleUI.CurrentTeamImage.color = CurrentPlayedTeam.InitialTeamSettings.TeamColor;
		}
	}
}