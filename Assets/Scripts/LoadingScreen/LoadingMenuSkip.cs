using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMenuSkip : MonoBehaviour
{
    public GameManager GM;
    public GameObject Results;


    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey || Input.anyKeyDown)
        {
            if (Results.activeInHierarchy)
            {
                GM.ContinueFromResults();
                gameObject.SetActive(false);
            }
            else
            {
               GM.ConfirmScenarioSelection(true);              
               gameObject.SetActive(false);
            }

        }
    }
}
