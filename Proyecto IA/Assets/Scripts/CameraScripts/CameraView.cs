using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public GameObject image;
    IAlert _alert;
   
    private void Awake()
    {
        _alert = GetComponent<IAlert>();
    }

    void Update()
    {
        image.SetActive(_alert.IsAlert);
    }
}
