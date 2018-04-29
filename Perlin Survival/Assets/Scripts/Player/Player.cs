﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour {
    public float startHP = 100f;
    public float hp = 100f;
    public Image healthBar;
    public Text hpText;

	// Use this for initialization
    // Built-in Unity method called on the start of the game
	void Start () {
        // Display HP at the start of game
        int hpInt = (int)hp;

        hpText.text = hpInt.ToString();
    }

    /**
     * Method that subtracts the damage taken from the total health; supports death
     */
    public void TakeDamage(float damage)
    {
        hp -= damage;
        healthBar.fillAmount = hp / startHP;

        int hpInt = (int)hp;

        hpText.text = hpInt.ToString();

        if (hp <= 0) {
            Die();
        }
    }

    /**
     * Method that imitates death; deletes the object from the scene
     */
    void Die()
    {
        Debug.Log("You died.");
        // TODO: Implement death
    }
}
