using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DeathZone : MonoBehaviour {

    public Transform spawnPoint;

    private BoxCollider boxCollider;

	void Start () {
        boxCollider = GetComponent<BoxCollider>();
	}

    private void OnTriggerEnter(Collider other) {
        other.transform.position = spawnPoint.position;
    }

    void Update () {
		
	}
}
