using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingTextChange : MonoBehaviour
{
    [SerializeField] public List<string> loadOutTitles;
    [SerializeField] public List<string> loadOutHints;
    public TextMeshProUGUI titleTextObject;
    public TextMeshProUGUI hintTextObject;
    private int lastLoadout = 0;

    public int choice;

    public float swapTimeDefaultValue = 12;

    //timer length
    public float swapTimer = 0;


    // Start is called before the first frame update
    private void OnEnable()
    {

        choice++;

        if (choice >= loadOutTitles.Count)
        {
            choice = 0;
        }
        
        if (loadOutTitles.Count > 2)
        {
            //choice = Random.Range(0, loadOutTitles.Count);
            

            titleTextObject.text = loadOutTitles[choice];
            hintTextObject.text = loadOutHints[choice];

            swapTimer = swapTimeDefaultValue;
        }

    }

    private void FixedUpdate()
    {
        swapTimer -= Time.deltaTime;

        if (swapTimer <= 0)
        {
            choice++;

            if(choice >= loadOutTitles.Count)
            {
                choice = 0;
            }


            titleTextObject.text = loadOutTitles[choice];
            hintTextObject.text = loadOutHints[choice];

            swapTimer = swapTimeDefaultValue;
        }
    }

}
