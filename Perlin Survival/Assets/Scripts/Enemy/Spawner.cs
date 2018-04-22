using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	public List<GameObject> spawnableEnemies;
	public List<GameObject> spawnedEnemies;

	public float spawnRate;
	public float spawnCooldown = 0f;

	void Start() {
		spawnedEnemies = new List<GameObject>();
	}

	// Update is called once per frame
	void Update() {
		// If the spawner is ready to spawn
        if (spawnCooldown <= 0f) {
            SpawnEnemy();

            // Reset spawn cooldown
            spawnCooldown = 1f / spawnRate;
        }

		// Decrease spawn cooldown according to game time
		spawnCooldown -= Time.deltaTime;
	}

	void SpawnEnemy() {
		// Get random enemy index
		int enemyIndex = Random.Range(0, spawnableEnemies.Count);
		Debug.Log(enemyIndex + ": " + spawnableEnemies[enemyIndex].ToString());

		GameObject enemyToSpawn = spawnableEnemies[enemyIndex];

		spawnedEnemies.Add((GameObject)Instantiate(enemyToSpawn, transform.position, transform.rotation));
	}
}
