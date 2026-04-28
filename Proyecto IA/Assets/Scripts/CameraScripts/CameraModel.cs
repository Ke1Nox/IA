using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : MonoBehaviour, IAlert
{
    bool _isAlert;
    public bool IsAlert
    {
        get
        {
            return _isAlert;
        }
        set
        {
            _isAlert = value;
        }
    }
}
