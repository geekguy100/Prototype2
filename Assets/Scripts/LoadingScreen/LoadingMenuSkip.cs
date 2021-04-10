using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMenuSkip : MonoBehaviour
{
    public GameManager GM;
    public GameObject Results;

    private bool activated;

    void OnEnable()
    {
        print("LOADING SCREEN START CALLED");
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.anyKey || Input.anyKeyDown) && !activated)
        {
            activated = true;

            if (Results.activeInHierarchy)
            {
                Transition.instance.StartTransition(() =>
                {
                    GM.ContinueFromResults();
                    gameObject.SetActive(false);
                });
            }
            else
            {
                Transition.instance.StartTransition(() => 
                {
                    GM.ConfirmScenarioSelection(true);
                    gameObject.SetActive(false);
                });
            }

        }
    }
}
