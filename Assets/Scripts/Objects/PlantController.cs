
using UnityEngine;

public class PlantController : MonoBehaviour
{
    public FanController fc;
    private void OnEnable()
    {
        GetComponent<Animator>().SetBool("isPlaying", fc.GetIsOn());
    }
}
