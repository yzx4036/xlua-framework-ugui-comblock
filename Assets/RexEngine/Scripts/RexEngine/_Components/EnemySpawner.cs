using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RexEngine
{
	public class EnemySpawner:MonoBehaviour 
	{
		public RexPool rexPool;
		public int maxEnemies = 3;
		public float spawnInterval = 1.5f;
		public SpawnPosition spawnPosition;

		protected EnemySpawnValues spawnValues;

		protected class EnemySpawnValues
		{
			public bool hasSetSpawnValues = false;
			public bool usesGravity = true;
		}

		[System.Serializable]
		public class SpawnPosition
		{
			public SpawnPositionType spawnPositionType;
			public Vector2 plusMin;
			public Vector2 plusMax;
			public Vector2 minusMin;
			public Vector2 minusMax;
		}

		public enum SpawnPositionType
		{
			AtSpawner,
			AtPlayerX,
			AtPlayerY,
			AtPlayerXAndY
		}

		void Awake() 
		{
			spawnValues = new EnemySpawnValues();
		}

		void Start() 
		{
			InvokeRepeating("SpawnEnemyIfAble", spawnInterval, spawnInterval);
		}

		public void StopSpawning()
		{
			CancelInvoke();
		}

		protected void SpawnEnemyIfAble()
		{
			if(maxEnemies > 0 && rexPool != null && rexPool.ActiveObjects() < maxEnemies)
			{
				SpawnEnemy();
			}
		}

		protected void SpawnEnemy()
		{
			GameObject enemyGameObject = rexPool.Spawn();
			Enemy enemy = enemyGameObject.GetComponent<Enemy>();

			if(enemy != null)
			{
				if(!spawnValues.hasSetSpawnValues)
				{
					spawnValues.hasSetSpawnValues = true;
					AssignSpawnValues(enemy);
				}

				Vector2 spawnPositionAdjustment = GetSpawnPositionAdjustment();
				if(spawnPosition.spawnPositionType == SpawnPositionType.AtSpawner)
				{
					enemy.SetPosition(new Vector2(transform.position.x + spawnPositionAdjustment.x, transform.position.y + spawnPositionAdjustment.y));
				}
				else if(spawnPosition.spawnPositionType == SpawnPositionType.AtPlayerX)
				{
					enemy.SetPosition(new Vector2(GameManager.Instance.player.transform.position.x + spawnPositionAdjustment.x, transform.position.y + spawnPositionAdjustment.y));
				}
				else if(spawnPosition.spawnPositionType == SpawnPositionType.AtPlayerY)
				{
					enemy.SetPosition(new Vector2(transform.position.x + spawnPositionAdjustment.x, GameManager.Instance.player.transform.position.y + spawnPositionAdjustment.y));
				}
				else if(spawnPosition.spawnPositionType == SpawnPositionType.AtPlayerXAndY)
				{
					enemy.SetPosition(new Vector2(GameManager.Instance.player.transform.position.x + spawnPositionAdjustment.x, GameManager.Instance.player.transform.position.y + spawnPositionAdjustment.y));
				}

				enemy.parentSpawnPool = rexPool;

				if(enemy.slots.controller != null)
				{
					enemy.slots.controller.SetToAlive();
					enemy.slots.controller.AnimateEnable();
				}

				enemy.Revive();
				enemy.slots.physicsObject.SetVelocityX(0.0f);
				enemy.slots.physicsObject.SetVelocityY(0.0f);
				enemy.slots.spriteHolder.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

				SetSpawnValuesOnEnemy(enemy);

				enemy.OnSpawned();
			}
		}

		protected Vector2 GetSpawnPositionAdjustment()
		{
			Vector2 spawnPositionAdjustment = new Vector2();

			bool willUseMinusX = (Mathf.Abs(spawnPosition.minusMin.x) > 0.0f || Mathf.Abs(spawnPosition.minusMax.x) > 0.0f) && (RexMath.RandomInt(0, 1) == 1);
			if(willUseMinusX)
			{
				spawnPositionAdjustment.x = RexMath.RandomFloat(spawnPosition.minusMin.x, spawnPosition.minusMax.x);
			}
			else
			{
				spawnPositionAdjustment.x = RexMath.RandomFloat(spawnPosition.plusMin.x, spawnPosition.plusMax.x);
			}

			bool willUseMinusY = (Mathf.Abs(spawnPosition.minusMin.y) > 0.0f || Mathf.Abs(spawnPosition.minusMax.y) > 0.0f) && (RexMath.RandomInt(0, 1) == 1);
			if(willUseMinusY)
			{
				spawnPositionAdjustment.y = RexMath.RandomFloat(spawnPosition.minusMin.y, spawnPosition.minusMax.y);
			}
			else
			{
				spawnPositionAdjustment.y = RexMath.RandomFloat(spawnPosition.plusMin.y, spawnPosition.plusMax.y);
			}

			return spawnPositionAdjustment;
		}

		protected void SetSpawnValuesOnEnemy(Enemy _enemy)
		{
			_enemy.slots.physicsObject.gravitySettings.usesGravity = spawnValues.usesGravity;
		}

		protected void AssignSpawnValues(Enemy _enemy)
		{
			spawnValues.usesGravity = _enemy.slots.physicsObject.gravitySettings.usesGravity;
		}
	}
}

