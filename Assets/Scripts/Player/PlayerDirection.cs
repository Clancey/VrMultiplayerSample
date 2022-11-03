using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Head;
    public Transform Mid;
    public Quaternion Rotation;
    private void Update()
    {
        // (var rotation, var midpoint) = CalculateRotation();

        //Debug.DrawLine(Mid.position, Mid.position + Mid.forward * 1f, Color.black);

    }
    public (Quaternion rotation, Vector3 midpoint) CalculateRotation()
    {
        Vector3 midpointAtoB = (LeftHand.position + RightHand.position) / 2;

        var middle = (LeftHand.position + RightHand.position + Head.position) / 3;
        middle.y = midpointAtoB.y;

        Mid.position = midpointAtoB;
        //Look at a hand, Then rotate 90 degrees to be facing forward
        Mid.LookAt(LeftHand.position, this.transform.up);
        var rotation = Mid.transform.rotation * Quaternion.Euler(0, 90, 0);
        rotation.x = 0;
        rotation.z = 0;
        Mid.transform.rotation = rotation;
        return (Mid.rotation, midpointAtoB);
    }
}
