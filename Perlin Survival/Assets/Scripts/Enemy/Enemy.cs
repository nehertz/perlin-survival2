using UnityEngine;
using System.Collections;

/**
 * Implements the functionality of an enemy that has health and can die
 */
public class Enemy : MonoBehaviour {
    public float health = 50f;

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
