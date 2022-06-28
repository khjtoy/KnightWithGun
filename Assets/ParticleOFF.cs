using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOFF : MonoBehaviour
{
    private ParticleSystem myParicle;

    private void Start()
    {
        myParicle = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if(!myParicle.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
