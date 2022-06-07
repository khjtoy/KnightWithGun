using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlash : Character
{
    private readonly int hashSlash = Animator.StringToHash("IsSlash");
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            ani.SetTrigger(hashSlash);
        }
    }
}
