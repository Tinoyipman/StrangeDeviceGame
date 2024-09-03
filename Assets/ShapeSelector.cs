using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSelector : MonoBehaviour
{
    public Image displayImage; // Reference to the UI Image where the selected image will be displayed.
    public string folderPath = "Resources/Shapes"; // Path inside Resources folder, e.g., Resources/Images.

    private Sprite[] imageSprites; // Array to hold loaded sprites.

    void Start()
    {
        LoadImagesFromResources();
        DisplayRandomImage();
    }

    // Load all images from the specified Resources folder.
    void LoadImagesFromResources()
    {
        // Load all sprites from the folderPath inside Resources.
        imageSprites = Resources.LoadAll<Sprite>(folderPath);

        if (imageSprites.Length == 0)
        {
            Debug.LogError($"No images found in Resources/{folderPath}");
        }
    }

    // Select and display a random image from the loaded images.
    void DisplayRandomImage()
    {
        if (imageSprites.Length > 0)
        {
            // Generate a random index to select an image.
            int randomIndex = Random.Range(0, imageSprites.Length);
            // Assign the selected sprite to the UI Image.
            displayImage.sprite = imageSprites[randomIndex];
            Color ImageColor = displayImage.color;
            ImageColor.a = 0.2f;
            displayImage.color = ImageColor;
        }
    }

    // Optional: Call this method to change the displayed image randomly.
    public void ChangeImage()
    {
        DisplayRandomImage();
    }
}
