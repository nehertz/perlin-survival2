using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Singleton-pattern manager that manages the instance of the Player and gives a point of access for other scripts.
 */
[System.Serializable]
public class PlayerManager : MonoBehaviour {
    #region Singleton

    public static PlayerManager instance;

    void Awake()
    {
        instance = this;
    }

    #endregion

    public GameObject playerObject;
}
