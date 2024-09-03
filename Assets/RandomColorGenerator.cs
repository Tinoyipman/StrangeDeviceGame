using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomColorGenerator : MonoBehaviour
{
    public Image DisplayImage;
    public float PopUpTime;

    private Color RandomColor;
    // Start is called before the first frame update
    void Start()
    {
        RandomColor = DisplayImage.color;

        ColorPopUp();
    }

    public void ColorPopUp()
    {
        Color random = new Color(Random.value, Random.value, Random.value);

        StartCoroutine(FlashColor(random));
    }

    private System.Collections.IEnumerator FlashColor(Color color)
    {
        // Set the Image's color to the random color.
        DisplayImage.color = color;

        // Wait for the specified flash duration.
        yield return new WaitForSeconds(PopUpTime);

        // Revert the Image's color back to the original.
        DisplayImage.color = RandomColor;
        DisplayImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
