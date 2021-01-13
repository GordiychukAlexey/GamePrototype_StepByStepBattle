using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools {
	public static class Pathfinding {
		/// <summary>
		/// Волновой алгоритм поиска пути.
		/// Alexey Gordiychuk, 2020
		/// </summary>
		/// <param name="fromPosition">Старт</param>
		/// <param name="toPosition">Финиш</param>
		/// <param name="isWalkable">Возможно ли двигаться на заданную позицию</param>
		/// <param name="criticalWaveSize">При достижении волной этого размера, алгоритм прерывается.
		/// Использовать в случае открытой области поиска</param>
		/// <returns>Последовательность шагов от начальной позиции до конечной</returns>
		public static Vector2Int[] WaveAlgorithm(Vector2Int fromPosition, Vector2Int toPosition, Predicate<Vector2Int> isWalkable,
		                                         int criticalWaveSize = int.MaxValue){
			Vector2Int[] movingDirections = {
				Vector2Int.up,
				Vector2Int.right,
				Vector2Int.down,
				Vector2Int.left,
				Vector2Int.up + Vector2Int.right,
				Vector2Int.right + Vector2Int.down,
				Vector2Int.down + Vector2Int.left,
				Vector2Int.left + Vector2Int.up,
			};

			Dictionary<int, List<Vector2Int>> allWaves = new Dictionary<int, List<Vector2Int>>();
			List<Vector2Int> previousWave = new List<Vector2Int>();
			List<Vector2Int> currentWave = new List<Vector2Int>();
			List<Vector2Int> newWave = new List<Vector2Int>();

			bool IsVisited(Vector2Int position) =>
				previousWave.Any(item => position == item)
				|| currentWave.Any(item => position == item)
				|| newWave.Any(item => position == item);

			bool IsOpenToResearch(Vector2Int position) =>
				isWalkable(position) && !IsVisited(position);


			int newWaveStep = 0;
			newWave.Add(fromPosition);
			bool isToPositionResearch = false;
			do{
				allWaves.Add(newWaveStep, new List<Vector2Int>(newWave));
				previousWave = new List<Vector2Int>(currentWave);
				currentWave  = new List<Vector2Int>(newWave);

				++newWaveStep;

				newWave.Clear();
				foreach (Vector2Int lastWavePosition in currentWave){
					foreach (Vector2Int movingDirection in movingDirections){
						Vector2Int checkedPosition = lastWavePosition + movingDirection;
						if (IsOpenToResearch(checkedPosition)){
							newWave.Add(checkedPosition);

							if (checkedPosition == toPosition){
//								Debug.Log("ToPosition reached!");
								isToPositionResearch = true;
								break;
							}
						}
					}

					if (isToPositionResearch){
						break;
					}
				}

				if (isToPositionResearch){
					break;
				}

//				Debug.Log($"{newWaveStep} {string.Join(", ", newWave)}");
				if (newWave.Count >= criticalWaveSize){
					Debug.LogError($"{nameof(criticalWaveSize)} reached!");
					break;
				}
			} while (newWave.Count > 0);


			if (!isToPositionResearch){
				Debug.LogError("Path not found!");
				return new Vector2Int[0];
			}

			List<Vector2Int> backStepPath = new List<Vector2Int>();
			Vector2Int previousPosition = toPosition;
			backStepPath.Add(previousPosition);
			int backStepWaveWeight = newWaveStep;
			while (backStepWaveWeight > 0){
				--backStepWaveWeight;
				List<Vector2Int> nextStepWave = allWaves[backStepWaveWeight];

				Vector2Int nextStepPosition = nextStepWave.Find(
					pos => Math.Abs(previousPosition.x - pos.x) <= 1
					       && Math.Abs(previousPosition.y - pos.y) <= 1);

				backStepPath.Add(nextStepPosition);
				previousPosition = nextStepPosition;
			}

			backStepPath.Reverse();
			return backStepPath.ToArray();
		}
	}
}