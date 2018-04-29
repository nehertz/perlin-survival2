using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour {
	public List<GameObject> spawnableEnemies;
	public List<Enemy> spawnedEnemies;

	public float spawnRate;
	public float spawnCooldown = 0f;
    public int spawnLimit;

    GameObject playerObject;

    // Built-in Unity method called on the start of the game
    void Start() {
		spawnedEnemies = new List<Enemy>();

        playerObject = PlayerManager.instance.playerObject;
    }

	// Update is called once per frame
    // Built-in Unity method
	void Update() {
		// If the spawner is ready to spawn
        if (spawnCooldown <= 0f && spawnedEnemies.Count < spawnLimit) {
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
        Vector3 spawnPosition = playerObject.transform.position + playerObject.transform.forward * -20;

        // Move the enemy spawn position onto the NavMesh
        NavMeshHit closestPos;

        if (NavMesh.SamplePosition(spawnPosition, out closestPos, 100f, NavMesh.AllAreas)) {
            spawnPosition = new Vector3(closestPos.position.x, closestPos.position.y + 0.5f, closestPos.position.z);
        } else {
            print("Cannot place" + spawnableEnemies[enemyIndex].ToString() + "on NavMesh.");
            return;
        }

        GameObject enemyToSpawn = spawnableEnemies[enemyIndex];
        GameObject spawnedEnemy = (GameObject)Instantiate(enemyToSpawn, spawnPosition, transform.rotation);

        spawnedEnemy.transform.parent = transform;
        spawnedEnemy.GetComponent<Enemy>().OnEnemyDeath += RemoveEnemy;

        spawnedEnemies.Add(spawnedEnemy.GetComponent<Enemy>());
	}

    void RemoveEnemy(Enemy enemy)
    {
        spawnedEnemies.Remove(enemy);
    }
}
