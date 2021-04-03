using UnityEngine;
using TMPro;

public class WaitBeforeLoadingText : MonoBehaviour
{
    public float timeBeforeFadeIn = 7f;
    private float originalTime;
    public TextMeshProUGUI textObject;
    bool flipAlpha = false;
    private string loadText = "Now Loading...";
    private string continueText = "Press any button to continue.";

    private void Awake()
    {
        textObject = GetComponent<TextMeshProUGUI>();
        
        originalTime = timeBeforeFadeIn;

        textObject.text = loadText;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBeforeFadeIn > 0)
        {
            timeBeforeFadeIn -= Time.deltaTime;
        }

        if ( !flipAlpha)
        {
            textObject.color = textObject.color + new Color(0,0,0,Time.deltaTime);
            
        }

        if( flipAlpha)
        {
            textObject.color = textObject.color - new Color(0, 0, 0, Time.deltaTime);
        }

        if(textObject.color.a >= 1 || textObject.color.a <= 0)
        {
            flipAlpha = !flipAlpha;

            if (timeBeforeFadeIn < 0 && textObject.color.a <= 0)
            {
                textObject.text = continueText;
            }
        }
    }

    public void OnDisable()
    {
        timeBeforeFadeIn = originalTime;
        flipAlpha = false;
        textObject.color =  new Color(1, 1, 1, 0);
        textObject.text = loadText;
    }
}
