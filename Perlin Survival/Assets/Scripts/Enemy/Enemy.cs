using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;
using System;

/**
 * Implements the functionality of an enemy that has health and can die
 */
public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public Image healthBar;
    public Canvas enemyInfo;
    public Camera playerCam;
    public GameObject playerObject;
    public Player player;
    public event Action<Enemy> OnEnemyDeath;

    // The target for the enemy's nav mesh
    Transform target;
    // Nav mesh for the enemy
    NavMeshAgent agent;
    // Poistion of the enemy
    Vector3 pos;
    // Previous position of the enemy
    Vector3 previousPos;
    public bool isInactive;

    public float attackCooldown = 0f;

    void Start()
    {
        data.hp = data.startHP;

        isInactive = false;

        playerObject = PlayerManager.instance.playerObject;
        target = playerObject.transform;
        player = playerObject.GetComponent<Player>();
        playerCam = playerObject.transform.GetChild(0).GetComponent<Camera>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = data.moveSpeed;

        StartCoroutine(CheckInactive());
    }

    void Update()
    {
        pos = transform.position;

        // If the enemy is not on the NavMesh
        if (!agent.isOnNavMesh) {
            NavMeshHit closestPos;

            // Move the enemy onto the NavMesh
            if (NavMesh.SamplePosition(transform.position, out closestPos, 100f, NavMesh.AllAreas)) {
                pos = new Vector3(closestPos.position.x, closestPos.position.y + 0.1f, closestPos.position.z);
                transform.position = pos;
            } else {
                print("Moving enemy to NavMesh.");
            }
        }

        /* Currently broken
        // Kill the enemy if it's inactive
        if (isInactive) {
            Die(0);
        }
        */

        float distanceFromPlayer = Vector3.Distance(target.position, pos);

        // Display the enemy's info above their head.
        DisplayEnemyInfo();

        // If the player enters the aggro range of the enemy
        if (distanceFromPlayer < data.aggroRadius) {
            ChasePlayer();

            // If the enemy gets close enough to attack
            if (distanceFromPlayer < agent.stoppingDistance) {
                Attack();

                FacePlayer();
            }
        }

        // Decrease the attackCooldown according to game time passed
        attackCooldown -= Time.deltaTime;
        previousPos = pos;
    }

    /**
     * Checks every 3 seconds if the enemy has moved and sets the inactive boolean accordingly
     */
    IEnumerator CheckInactive()
    {
        if (pos == previousPos) {
            isInactive = true;
        } else {
            isInactive = false;
        }

        yield return new WaitForSeconds(30);
    }

    /**
     * Method that handles the enemy's attack functionality
     */
    void Attack()
    {
        // If the enemy is ready to attack
        if (attackCooldown <= 0f) {
            player.TakeDamage(data.damage);
            // Reset attackCooldown
            attackCooldown = 1f / data.attackSpeed;
        }
    }

    /**
     * Makes the enemy face the player if they are within range
     */
    void FacePlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    /**
     * Chase the player using a NavMesh
     */
    void ChasePlayer()
    {
        // Chase player
        if (agent.isOnNavMesh) {
            agent.SetDestination(target.position);
        }
    }

    /**
     * Counts the number of corners of a RectTransform that are visible to a camera
     * @param rectTransform The rectTransform
     * @param camera The camera
     * @return The number of corners visible to the camera
     */
    private static int CountCornersVisibleFrom(RectTransform rectTransform, Camera camera)
    {
        // Screen space bounds (assumes camera renders across the entire screen)
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        int visibleCorners = 0;
        Vector3 tempScreenSpaceCorner;
        for (var i = 0; i < objectCorners.Length; i++) {
            // Transform world space position of corner to screen space
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]);

            // If the corner is inside the screen
            if (screenBounds.Contains(tempScreenSpaceCorner)) {
                visibleCorners++;
            }
        }

        return visibleCorners;
    }

    /**
     * If the enemy's info is within camera view, display the info. Otherwise, disable it.
     * Also, make the info face toward the camera.
     */
    void DisplayEnemyInfo()
    {
        //Rect infoRect = enemyInfo.pixelRect;
        RectTransform infoTransform = enemyInfo.GetComponent<RectTransform>();

        if (CountCornersVisibleFrom(infoTransform, playerCam) > 0) {
            // Make the enemy's health bar and name face the player's camera
            Vector3 faceCamera = playerCam.transform.position - enemyInfo.transform.position;

            enemyInfo.enabled = true;
            faceCamera.x = 0f;
            faceCamera.z = 0f;

            enemyInfo.transform.LookAt(playerCam.transform.position - faceCamera);
            enemyInfo.transform.Rotate(0, 180, 0);
        } else {
            enemyInfo.enabled = false;
        }
    }

    /**
     * Makes the enemy turn red for 0.3 seconds
     */
    IEnumerator TurnRed()
    {
        float timePassed = 0;
        Color color = GetComponent<Renderer>().material.color;

        while (timePassed < 0.3) {
            GetComponent<Renderer>().material.color = new Color(color.r + 0.5f, color.g, color.b);

            timePassed += Time.deltaTime;

            yield return null;
        }

        GetComponent<Renderer>().material.color = new Color(color.r, color.g, color.b);
    }

    /**
     * Method that subtracts the damage taken from the total health; supports death
     * @param damage The amount of damage the enemy will take
     */
    public void TakeDamage(float damage)
    {
        StartCoroutine(TurnRed());

        data.hp -= damage;
        healthBar.fillAmount = data.hp / data.startHP;

        if (data.hp <= 0) {
            Die(10);
        }
    }

    /**
     * Method that imitates death; deletes the object from the scene
     * @param pointsForDeath Number of points given to the player for this death
     */
    void Die(int pointsForDeath)
    {
        OnEnemyDeath(this);
        Destroy(gameObject);
    }

    /**
     * Struct that keeps track of useful enemy data
     */
    [System.Serializable]
    public struct EnemyData
    {
        // Radius of aggression for the enemy
        public float aggroRadius;
        // Starting hp for enemy (full health)
        public float startHP;
        // Current enemy HP
        public float hp;
        // Damage the enemy does
        public float damage;
        // The rate at which the enemy may attack
        public float attackSpeed;
        // The rate at which the enemy may move
        public float moveSpeed;
    }
}
