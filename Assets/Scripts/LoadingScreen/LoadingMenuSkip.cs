using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingMenuSkip : MonoBehaviour
{
    public GameObject GM;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey || Input.anyKeyDown)
        {
            GM.GetComponent<GameManager>().ContinueFromResults();
            gameObject.SetActive(false);
        }
    }
}
