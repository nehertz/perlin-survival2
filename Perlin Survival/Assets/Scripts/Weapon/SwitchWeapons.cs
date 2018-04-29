using UnityEngine;

/**
 * Class that enables weapon switching to the player
 */
public class SwitchWeapons : MonoBehaviour {
    public int selectedWeaponIndex;

	// Use this for initialization
	void Start () {
        selectedWeaponIndex = 0;

        SwitchWeapon();
	}
	
	// Update is called once per frame
	void Update () {
        int previousWeaponIndex = selectedWeaponIndex;

        // If the player scrolls up
		if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            if (selectedWeaponIndex >= transform.childCount - 1) {
                selectedWeaponIndex = 0;
            } else {
                selectedWeaponIndex++;
            }
        }

        // If the player scrolls down
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            if (selectedWeaponIndex <= 0) {
                selectedWeaponIndex = transform.childCount - 1;
            } else {
                selectedWeaponIndex--;
            }
        }

        // A weapon switch has occurred
        if (previousWeaponIndex != selectedWeaponIndex) {
            SwitchWeapon();
        }
    }

    /**
     * Method that allows the player to switch weapons by enabling the active weapon and disabling the inactive one
     */
    void SwitchWeapon()
    {
        int i = 0;

        foreach (Transform weapon in transform) {
            // If the correct weapon is selected, show it; otherwise, hide it
            if (i == selectedWeaponIndex) {
                weapon.gameObject.SetActive(true);
            } else {
                weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}
