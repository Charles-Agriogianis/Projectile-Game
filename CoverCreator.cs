using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCreator : MonoBehaviour {
    public Transform Cover;

    void Start() {
        for (int z = 1; z <= 49; z = z + 12) {
            for (int x = 1; x <= 49; x = x + 12) {
                Transform cover = Instantiate(Cover, new Vector3(x, .5f, z), Quaternion.identity);
            }
        }
    }

    void Update() {
        
    }
}
