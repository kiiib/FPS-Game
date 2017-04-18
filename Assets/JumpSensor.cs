using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSensor : MonoBehaviour {

    private int TouchedCount = 0;

	void OnTriggerEnter(Collider other)
    {
        TouchedCount++;
    }
    void OnTriggerExit(Collider other)
    {
        TouchedCount--;
    }

    public bool IsCanJump()
    {
        return TouchedCount > 0;
    }
}
