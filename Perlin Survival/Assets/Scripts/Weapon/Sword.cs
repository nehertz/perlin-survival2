using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {
    public float damage = 10f;
    public float range = 3f;
    public float attackSpeed = 1.5f;
    public float attackCooldown = 0f;

    public Camera view;
    public GameObject target;
    public Animator swingAnimation;
    public Collider swordCollider;

    // Use this for initialization
    void Start () {
        swingAnimation = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) {
            SwingWeapon(swordCollider);
        }

        // Decrease attack cooldown according to game time
        attackCooldown -= Time.deltaTime;
    }

    /**
     * Triggers an animation of the sword swinging and damages and enemy within range
     */
    void SwingWeapon(Collider collider)
    {
        // If the sword is ready to swing
        if (attackCooldown <= 0f) {
            //Play animation
            swingAnimation.SetTrigger("pressedAttackKey");

            print("Attacking");

            // Get the list of colliders hit by the sword collider
            Collider[] hitColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation);

            // Loop through the hit colliders and apply damage
            foreach (Collider c in hitColliders) {
                if (c.tag == "Enemy") {
                    c.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                }
            }
        }
    }
}
