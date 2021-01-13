using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Tools;
using UnityEngine;

namespace Main {
	public class Battlefield : MonoBehaviour {
		public BattlefieldTile BattlefieldTilePrefab;
		public Transform       TilesRoot;
		public Transform       UnitsRoot;

		private void Awake(){
			Transform CreateNewRootObject(string name){
				Transform tr = new GameObject(name).transform;
				tr.parent        = transform;
				tr.localPosition = Vector3.zero;
				tr.localRotation = Quaternion.identity;
				return tr;
			}

			TilesRoot = CreateNewRootObject(nameof(TilesRoot));
			UnitsRoot = CreateNewRootObject(nameof(UnitsRoot));
		}

		private BattlefieldTile[,] Tiles;
		private Unit[,]            Units;

		public List<Team> Teams = new List<Team>();

		[Serializable]
		public class Team {
			public readonly string                           TeamName;
			public          List<Unit>                       Units = new List<Unit>();
			public          GameSettings.InitialTeamSettings InitialTeamSettings;

			public Team(GameSettings.InitialTeamSettings initialTeamSettings){
				InitialTeamSettings = initialTeamSettings;
				TeamName            = initialTeamSettings.TeamName;
			}
		}

		public void CreateBattlefield(Vector2Int size){
			Tiles = new BattlefieldTile[size.x, size.y];
			for (int yIndex = 0; yIndex < size.y; yIndex++){
				for (int xIndex = 0; xIndex < size.x; xIndex++){
					BattlefieldTile tile = Instantiate(
						BattlefieldTilePrefab,
						transform.position + new Vector3(xIndex, 0.0f, yIndex),
						Quaternion.identity,
						TilesRoot);
					tile.Initialize(new Vector2Int(xIndex, yIndex));
					Tiles[xIndex, yIndex] = tile;
				}
			}

			Units = new Unit[size.x, size.y];
		}

		public Team GetOrCreateTeam(GameSettings.InitialTeamSettings teamParameters){
			Team foundTeam = Teams.FirstOrDefault(team => teamParameters.TeamName == team.TeamName);
			if (foundTeam != null){
				return foundTeam;
			} else{
				Team newTeam = new Team(teamParameters);
				Teams.Add(newTeam);
				return newTeam;
			}
		}

		public void SpawnUnit(GameSettings.UnitParameters unitParameters, Vector2Int position, GameSettings.InitialTeamSettings teamParameters){
			BattlefieldTile tile = GetTile(position);
			if (tile.TileObject != null){
				Debug.LogError($"Tile {position} already occupied by another object");
				return;
			}

			Unit newUnit = Instantiate(GameSettings.Instance.GetUnitParameters(unitParameters.Name).Prefab,
				transform.position + new Vector3(position.x, 0.0f, position.y),
				Quaternion.identity,
				UnitsRoot);
			newUnit.Body.material.color = teamParameters.TeamColor;
			Team team = GetOrCreateTeam(teamParameters);
			newUnit.Initialize(unitParameters, tile, team);
			Units[position.x, position.y] = newUnit;
			tile.TileObject               = newUnit;
			team.Units.Add(newUnit);
		}

		public void MoveUnit(Vector2Int[] path){
			foreach (Vector2Int vector2Int in path.Skip(1)){
				BattlefieldTile tile = GetTile(vector2Int);
				if (tile.TileObject != null){
					Debug.LogError($"Tile {vector2Int} already occupied and blocked move");
					return;
				}
			}

			BattlefieldTile firstTile = GetTile(path[0]);
			BattlefieldTile lastTile = GetTile(path[path.Length - 1]);

			Unit unit = firstTile.TileObject as Unit;
			if (!unit){
				Debug.LogError($"Tile object is not Unit");
				return;
			}

			if (unit.IsTurnEnded){
				Debug.LogError($"Unit already end his turn");
				return;
			}

			{
				lastTile.TileObject  = unit;
				firstTile.TileObject = null;

				unit.Tile      =  lastTile;
				unit.StepsLeft -= path.Length - 1;
			}


			Sequence movingOperation = DOTween.Sequence();
			foreach (Vector2Int position in path.Skip(1)){
				Vector3 positionInWS = transform.TransformPoint(new Vector3(
					position.x,
					0.0f,
					position.y));
				movingOperation.Append(unit.transform.DOLookAt(
					positionInWS,
					0.2f));
				movingOperation.Append(unit.transform.DOMove(positionInWS, 0.4f));
			}

//			movingOperation.AppendCallback(() => {
//				lastTile.TileObject = firstTile.TileObject;
//				firstTile.TileObject  = null;
//
//				lastTile.TileObject.Tile = lastTile;
//			});
		}

		public void Attack(Unit attacker, Unit attacked){
			if (attacker.IsTurnEnded){
				Debug.LogError("Attacker already end his turn");
				return;
			}

			Vector2Int asd = attacked.Tile.Position - attacker.Tile.Position;
			if (Mathf.Abs(asd.x) > 1 || Mathf.Abs(asd.y) > 1){
				Debug.LogError("Attacked target too far");
				return;
			}

			float attackMultiplier = (Mathf.Abs(asd.x) == 0 || Mathf.Abs(asd.y) == 0)
				? 1.0f
				: GameSettings.Instance.DiagonalAttackForceMultiplier;

			attacked.Health      -= attacker.Parameters.AttackForce * attackMultiplier;
			attacker.IsTurnEnded =  true;

			Sequence movingOperation = DOTween.Sequence();
			Vector3 positionInWS = transform.TransformPoint(new Vector3(
				attacked.Tile.Position.x,
				0.0f,
				attacked.Tile.Position.y));
			movingOperation.Append(attacker.transform.DOLookAt(
				positionInWS,
				0.2f));
			Vector3 eee = new Vector3(asd.x, 0.0f, asd.y) * 0.3f;
			movingOperation.Append(attacker.transform.DOMove(attacker.transform.position + eee, 0.2f).SetLoops(2, LoopType.Yoyo));
//			movingOperation.Append(attacker.transform.DOMove(attacker.transform.position + eee, 0.2f));
//			movingOperation.AppendCallback(() => {
//				attacked.Health      -= attacker.Parameters.AttackForce * attackMultiplier;
//				attacker.IsTurnEnded =  true;
//			});
//			movingOperation.Append(attacker.transform.DOMove(attacker.transform.position, 0.2f));
		}

		public Vector2Int[] FoundPath(Vector2Int from, Vector2Int to) => Pathfinding.WaveAlgorithm(from, to, IsWalkable);

		public bool IsWalkable(Vector2Int tilePosition){
			return IsPositionInsideBorders(tilePosition)
			       && GetTile(tilePosition).IsWalkable;
		}

		public BattlefieldTile GetTile(Vector2Int position){
			if (!IsPositionInsideBorders(position)){
				throw new ArgumentException($"Tile position {position} out of tiles bounds");
			}

			return Tiles[position.x, position.y];
		}

		private bool IsPositionInsideBorders(Vector2Int position){
			return position.x >= 0 && position.x < Tiles.GetLength(0) && position.y >= 0 && position.y < Tiles.GetLength(1);
		}
	}
}