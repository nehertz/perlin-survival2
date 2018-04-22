using UnityEngine;
using System.Collections;

/**
 * First weapon added to the game: a ray gun
 * Inherits from RangedWeapon and has a cool laser effect when you fire
 */
public class RayGun : MonoBehaviour {
    public float damage = 10f;
    public float range = 100f;
    public float attackSpeed = 1f;
    public float attackCooldown = 0f;

    public Camera view;
    public LineRenderer laser;
    public ParticleSystem beam;

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
            FireWeapon();
        }

        // Decrease attack cooldown according to game time
        attackCooldown -= Time.deltaTime;
	}

    /**
     * "Fires the weapon" or shoots a raycast out from the weapon and if it hits an enemy, subtracts health from it
     */
    public void FireWeapon()
    {
        // If the gun is ready to fire
        if (attackCooldown <= 0f) {
            beam.Play();

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit raycastHit;

            laser.enabled = true;
            laser.SetPosition(0, ray.origin);

            // Raycast hit an object
            if (Physics.Raycast(view.transform.position, view.transform.forward, out raycastHit, range)) {
                laser.SetPosition(1, raycastHit.point);

                Enemy target = raycastHit.transform.GetComponent<Enemy>();

                if (target != null) {
                    target.TakeDamage(damage);
                }
            } else {
                laser.SetPosition(1, ray.GetPoint(100));
            }

            laser.enabled = false;

            // Reset attackCooldown
            attackCooldown = 1f / attackSpeed;
        }
    }
}
