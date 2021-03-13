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


    // Start is called before the first frame update
    private void OnEnable()
    {
        if (loadOutTitles.Count > 2)
        {
            int choice = Random.Range(0, loadOutTitles.Count);
            

            titleTextObject.text = loadOutTitles[choice];
            hintTextObject.text = loadOutHints[choice];
        }
    }
}
