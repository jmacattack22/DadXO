using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BaseController
{

    void Update()
    {
        if (Health <= 0)
        {
            Anim.SetTrigger("Die");
        }
        if (CheckIFrame())
        {
            iframe -= Time.deltaTime;
        }
    }

}