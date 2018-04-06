using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
 * Implements the functionality of an enemy that has health and can die
 */
public class Enemy : MonoBehaviour
{
    public float startHealth = 50f;
    public float health;
    public Image healthBar;
    public Canvas enemyInfo;
    public Camera playerCam;

    void Start()
    {
        health = startHealth;
    }

    void Update()
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
        for (var i = 0; i < objectCorners.Length; i++)
        {
            // Transform world space position of corner to screen space
            tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]);

            // If the corner is inside the screen
            if (screenBounds.Contains(tempScreenSpaceCorner))
            {
                visibleCorners++;
            }
        }
        return visibleCorners;
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
     */
    public void TakeDamage(float damage)
    {
        StartCoroutine(TurnRed());

        health -= damage;
        healthBar.fillAmount = health / startHealth;

        if (health <= 0) {
            Die();
        }
    }

    /**
     * Method that imitates death; deletes the object from the scene
     */
    void Die()
    {
        Destroy(gameObject);
    }
}
