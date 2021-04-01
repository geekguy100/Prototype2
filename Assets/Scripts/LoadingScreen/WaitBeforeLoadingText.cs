using UnityEngine;
using TMPro;

public class WaitBeforeLoadingText : MonoBehaviour
{
    public float timeBeforeFadeIn = 7f;
    private float originalTime;
    public TextMeshProUGUI textObject;
    bool flipAlpha = false;

    private void Awake()
    {
        textObject = GetComponent<TextMeshProUGUI>();
        originalTime = timeBeforeFadeIn;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeBeforeFadeIn > 0)
        {
            timeBeforeFadeIn -= Time.deltaTime;
        }

        if (timeBeforeFadeIn <= 0 && !flipAlpha)
        {
            textObject.color = textObject.color + new Color(0,0,0,Time.deltaTime);
        }

        if(timeBeforeFadeIn <= 0 && flipAlpha)
        {
            textObject.color = textObject.color - new Color(0, 0, 0, Time.deltaTime);
        }

        if(textObject.color.a >= 1 || textObject.color.a <= 0)
        {
            flipAlpha = !flipAlpha;
        }
    }

    public void OnDisable()
    {
        timeBeforeFadeIn = originalTime;
        flipAlpha = false;
        textObject.color =  new Color(1, 1, 1, 0);
    }
}
