using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Controller/Stats")]
public class ControllerStats : ScriptableObject
{
    public float moveSpeed = 4;
    public float boostSpeed = 6;
    public float rotateSpeed = 8;
    public float torque = 8;
    public float maxAngularVelocity = 7;
    public float drag = 4;
    public float angularDrag = 4;
    public float jumpTrust = 4;
    public float downForce = 10;

}
