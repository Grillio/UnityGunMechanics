using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deletebullet : MonoBehaviour
{
    private float timer;
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 2.5f)
            Destroy(gameObject);
    }
}
