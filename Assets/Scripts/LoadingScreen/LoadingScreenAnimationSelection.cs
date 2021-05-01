using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenAnimationSelection : MonoBehaviour
{
    public GameObject dance1;
    public GameObject dance2;

    private void OnEnable()
    {
        dance1.SetActive(false);
        dance2.SetActive(false);

        if(Random.value > 0.5f)
        {
            dance1.SetActive(true);
        }
        else
        {
            dance2.SetActive(true);
        }
    }
}
