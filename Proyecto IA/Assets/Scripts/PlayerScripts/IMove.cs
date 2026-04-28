using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove 
{
    void Move(Vector3 dir);
    void Look(Vector3 dir);
    void Look(Transform target);
}
