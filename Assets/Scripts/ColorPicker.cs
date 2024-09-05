using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    // Reference to the RawImage component
    [SerializeField] private RawImage[] colorPreview;
    [SerializeField] private RawImage[] textureImage;
    [SerializeField] private Image OriginColorImage;
    public GameObject PlayerOneWin;
    public GameObject PlayerTwoWin;
    private bool active = false;
    private int state = 0;

    // Update is called once per frame
    void Update()
    {
        // Check if there is a touch input
        if (Input.touchCount > 0 && active)
        {
            // Get the first touch input
            Touch touch = Input.touches[0];

            // Check if the touch is a tap (i.e., not a swipe or drag)
            if (touch.phase == TouchPhase.Began)
            {
                // Get the touch position
                Vector2 touchPosition = touch.position;

                // Get the RawImage component's RectTransform
                RectTransform rawImageRect = textureImage[state].rectTransform;

                // Convert the touch position to local coordinates of the RawImage
                Vector2 localTouchPosition;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImageRect, touchPosition, null, out localTouchPosition))
                {
                    return;
                }

                // Convert the local touch position to texture coordinates
                Vector2 textureCoords = new Vector2((localTouchPosition.x + rawImageRect.rect.width / 2) / rawImageRect.rect.width, (localTouchPosition.y + rawImageRect.rect.height / 2) / rawImageRect.rect.height);

                // Get the color at the texture coordinates
                Color pickedColor = GetColorFromRawImage(textureImage[state], textureCoords);

                // Update the color preview
                colorPreview[state].color = pickedColor;

                if (active)
                {
                    if (state == colorPreview.Length - 1)
                    {
                        CheckOriginColor();
                        state = 0;
                    }
                    else
                    {
                        state++;
                    }
                }
                active = false;
            }
        }
    }

    public void SwitchActiveState()
    {
        if (active)
        {
            if (state == colorPreview.Length - 1)
            {
                state = 0;
            }
            else
            {
                state++;
            }
        }
        active = !active;
    }

    // Get the color from the RawImage texture at the specified coordinates
    Color GetColorFromRawImage(RawImage rawImage, Vector2 coords)
    {
        // Calculate the pixel coordinates
        int x = (int)(coords.x * rawImage.texture.width);
        int y = (int)(coords.y * rawImage.texture.height);

        // Create a new RenderTexture
        RenderTexture renderTexture = new RenderTexture(rawImage.texture.width, rawImage.texture.height, 0);

        // Render the RawImage texture to the RenderTexture
        Graphics.Blit(rawImage.texture, renderTexture);

        // Read the pixel data from the RenderTexture
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(1, 1);
        texture.ReadPixels(new Rect(x, y, 1, 1), 0, 0);
        RenderTexture.active = null;

        // Get the color from the pixel data
        Color color = texture.GetPixel(0, 0);

        // Clean up
        DestroyImmediate(texture);
        DestroyImmediate(renderTexture);

        return color;
    }

    public void CheckOriginColor()
    {
        Color OriginColor = OriginColorImage.color;
        List<Color> PreviewColors = new List<Color>();
        List<float> Delta = new List<float>();
        Debug.Log(colorPreview.Length);
        for (int i = 0; i < colorPreview.Length; i++)
        {
            //float similarity = ColorUtils.CalculateColorSimilarity(OriginColor, colorPreview[i].color);
            //Debug.Log($"Similarity: {similarity:F2}%");

            //ColorDifference = Mathf.Sqrt(
            //    Mathf.Pow(OriginColor.r - colorPreview[i].color.r, 2) + Mathf.Pow(OriginColor.g - colorPreview[i].color.g, 2) + Mathf.Pow(OriginColor.b - colorPreview[i].color.b, 2));
            //Debug.Log(ColorDifference);

            XYZColor xyz = XYZColor.FromColor(OriginColor);
            CIELabColor lab = CIELabColor.FromXYZ(xyz);

            XYZColor targetXyz = XYZColor.FromColor(colorPreview[i].color);
            CIELabColor targetLab = CIELabColor.FromXYZ(targetXyz);

            Delta.Add( lab.DistanceTo(targetLab));

            //code by: https://discussions.unity.com/t/c-color-detection-issue/710940/5
        }
        int DeltaIndex = Delta.IndexOf(Delta.Min());

        if (DeltaIndex == 0)
        {
            Debug.Log("Player 1 wins");
            PlayerOneWin.SetActive(true);
        }
        else
        {
            Debug.Log("Player 2 wins");
            PlayerTwoWin.SetActive(true);
        }

    }

    public class CIELabColor
    {
        public float l;
        public float a;
        public float b;

        public static CIELabColor FromXYZ(XYZColor color)
        {
            CIELabColor result = new CIELabColor();

            float x = color.x / 95.047f;
            float y = color.y / 100f;
            float z = color.z / 108.883f;

            if (x > 0.008856f)
            {
                x = Mathf.Pow(x, 1f / 3f);
            }
            else
            {
                x = (7.787f * x) + (16f / 116f);
            }

            if (y > 0.008856f)
            {
                y = Mathf.Pow(y, 1f / 3f);
            }
            else
            {
                y = (7.787f * y) + (16f / 116f);
            }

            if (z > 0.008856f)
            {
                z = Mathf.Pow(z, 1f / 3f);
            }
            else
            {
                z = (7.787f * z) + (16f / 116f);
            }

            result.l = (116f * y) - 16f;
            result.a = 500f * (x - y);
            result.b = 200f * (y - z);

            return result;
        }

        public float DistanceTo(CIELabColor color)
        {
            float deltaL = Mathf.Pow(color.l - l, 2f);
            float deltaA = Mathf.Pow(color.a - a, 2f);
            float deltaB = Mathf.Pow(color.b - b, 2f);
            float delta = Mathf.Sqrt(deltaL + deltaA + deltaB);

            return delta;
        }
    }

    public class XYZColor
    {
        public float x;
        public float y;
        public float z;

        public static XYZColor FromColor(Color color)
        {
            XYZColor result = new XYZColor();

            float r = color.r;// / 255f;
            float g = color.g;// / 255f;
            float b = color.b;// / 255f;

            if (r > 0.04045f)
            {
                r = Mathf.Pow((r + 0.055f) / 1.055f, 2.4f);
            }
            else
            {
                r = r / 12.92f;
            }

            if (g > 0.04045f)
            {
                g = Mathf.Pow((g + 0.055f) / 1.055f, 2.4f);
            }
            else
            {
                g = g / 12.92f;
            }

            if (b > 0.04045f)
            {
                b = Mathf.Pow((b + 0.055f) / 1.055f, 2.4f);
            }
            else
            {
                b = b / 12.92f;
            }

            r *= 100f;
            g *= 100f;
            b *= 100f;

            result.x = r * 0.4124f + g * 0.3576f + b * 0.1805f;
            result.y = r * 0.2126f + g * 0.7152f + b * 0.0722f;
            result.z = r * 0.0193f + g * 0.1192f + b * 0.9505f;

            return result;
        }
    }

    
}