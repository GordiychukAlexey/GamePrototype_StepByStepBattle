using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Main {
	public class GameSettings : MonoBehaviourSingleton<GameSettings> {
		public InitialBattleSettings BattleSettings = new InitialBattleSettings();

		public List<UnitParameters> UnitsParameters = new List<UnitParameters>();

		public float DiagonalAttackForceMultiplier = 1.0f - 0.33f;

		public UnitParameters GetUnitParameters(string unitName){
			return UnitsParameters.Find(parameters => unitName == parameters.Name);
		}

		public InitialTeamSettings GetTeamParameters(string teamName){
			return BattleSettings.TeamsParameters.Find(parameters => teamName == parameters.TeamName);
		}

		[Serializable]
		public class InitialBattleSettings {
			public Vector2Int                BattlefieldSize = new Vector2Int(8, 6);
			public string                    FirstTurnTeamName;
			public List<InitialTeamSettings> TeamsParameters = new List<InitialTeamSettings>();
		}

		[Serializable]
		public class InitialTeamSettings {
			public string                    TeamName;
			public Color                     TeamColor;
			public List<InitialUnitSettings> UnitsParameters = new List<InitialUnitSettings>();
		}

		[Serializable]
		public class InitialUnitSettings {
			public string     UnitName;
			public Vector2Int Position;
		}

		[Serializable]
		public class UnitParameters {
			public string Name;
			public float  InitialHealth;
			public float  AttackForce;
			public int    MaxMovingDistance;
			public Unit   Prefab;
		}
	}
}