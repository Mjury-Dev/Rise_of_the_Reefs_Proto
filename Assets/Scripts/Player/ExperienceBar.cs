using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public PlayerStats player;
    public Slider expSlider;

    private float lastExpCap = -1f;

    void Update()
    {
        if (player == null) return;

        if (player.experienceCap != lastExpCap)
        {
            expSlider.maxValue = player.experienceCap;
            lastExpCap = player.experienceCap;
        }

        expSlider.value = player.experience;
    }
}
