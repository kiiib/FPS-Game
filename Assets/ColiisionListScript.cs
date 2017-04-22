using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiisionListScript : MonoBehaviour {

    public List<Collider> CollisionObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other) {
        CollisionObjects.Add(other);
    }

    public void OnTriggerExit(Collider other) {
        CollisionObjects.Remove(other);
    }
}
