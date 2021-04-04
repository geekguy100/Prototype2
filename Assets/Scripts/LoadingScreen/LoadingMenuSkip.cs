using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMenuSkip : MonoBehaviour
{
    public GameObject GM;
    public GameObject Results;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey || Input.anyKeyDown)
        {
            if (Results.activeInHierarchy)
            {
                GM.GetComponent<GameManager>().ContinueFromResults();
                gameObject.SetActive(false);
            }
            else
            {
                GM.GetComponent<GameManager>().ConfirmScenarioSelection(true);
                
                gameObject.SetActive(false);
            }

        }
    }
}
