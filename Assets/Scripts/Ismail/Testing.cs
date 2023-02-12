using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GameObject anim;
    //public Anim_Roots anim_anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
           anim.GetComponent<ExtendableRoot>().Ahead_Root();
           // anim_anim.AheadRoot();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            anim.GetComponent<ExtendableRoot>().Retreat_Root();
            // anim_anim.AheadRoot();
        }
    }
}
