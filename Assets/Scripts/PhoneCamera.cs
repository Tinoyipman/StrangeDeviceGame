
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCamera;
    private List <Texture> placeholders = new();
    private int state = 0;
    private UnityEngine.SceneManagement.Scene scene;

    [SerializeField] private RawImage[] Images;
    [SerializeField] private AspectRatioFitter fit;

    public Image ColorImage;
    public GameObject ColorPicker;
    public float PopUpTime;
    public Color StartColor;

    public Image DisplayShape;
    public GameObject ShutterButton;




    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        for (int i = 0; i < Images.Length; i++ )
        {
            placeholders.Add(Images[i].texture);
        }
        
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log(devices.Length);

        if (devices.Length < 0)
        {
            Debug.Log(" NO camera detected");
            camAvailable = false;
            return;
            
        }

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
            //if (!devices[i].isFrontFacing /*devices[i].name == "HD Webcam"*/ )
            //{
                backCamera = new WebCamTexture(devices[0].name,Screen.width,Screen.height);
            //}

        }

        if(backCamera == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCamera.Play();
        Images[state].texture = backCamera;
        camAvailable = true;

        Sequence();

    }

    // Update is called once per frame
    void Update()
    {

        if (!camAvailable) {
            return;
        }
        else if (state < Images.Length)
        {
            float ratio = (float)backCamera.width / backCamera.height;
            fit.aspectRatio = ratio;

            float scaleY = backCamera.videoVerticallyMirrored? -1f:1f;
            Images[state].rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            int orient = -backCamera.videoRotationAngle;
            Images[state].rectTransform.localEulerAngles = new Vector3(0,0, orient);
        }
    }

    public void CapturePhoto()
    {
        if (camAvailable & state < Images.Length)
        {
            // Create a new texture to store the photo
            Texture2D photo = new Texture2D(backCamera.width, backCamera.height);
            photo.SetPixels(backCamera.GetPixels());
            photo.Apply();

            // Set the background texture to the captured photo
            Images[state].texture = photo;

            // Stop the camera feed
            

            Debug.Log("Photo captured and displayed as background");

            state++;
            Sequence();

            if (state > Images.Length)
            {
                state = 0;
                Sequence();
                Images[state].texture = backCamera;
                backCamera.Play();
            }
            else if (state == Images.Length)
            {
                backCamera.Stop();
            }
            else
            {
                Images[state].texture = backCamera;
            }
            
        }
        else if (state >= Images.Length)
        {
            state = 0;
            Sequence();
            for (int i = 0; i < placeholders.Count; i++)
            {
                Images[i].texture = placeholders[i];
            }
            Images[state].texture = backCamera;
            backCamera.Play();

            for (int i = 0; i < Images.Length; i++)
            {
                placeholders.Add(Images[i].texture);
            }
        }
        else
        {
            Debug.Log("Camera not available");
        }
    }

    public void Sequence()
    {
        Debug.Log("scene is" + scene.name);
        if (scene.name == "ColorKing")
        {
            switch (state)
            {
                case 0:
                    Images[1].enabled = false; break;
                case 1:
                    Images[1].enabled = true;
                    Images[0].enabled = false;
                    ColorImage.enabled = true;
                    StartCoroutine(FlashColor()); break;
                    
                case 2:
                    Images[0].enabled = true;
                    Images[1].enabled = true; break;
            }
        }
        else if (scene.name == "SplitImage")
        {
            if (state == 2)
            {
                DisplayShape.enabled = false;
                ShutterButton.SetActive(false);
            }
        }
        
    }

    private System.Collections.IEnumerator FlashColor()
    {
        PopUpTime = ColorPicker.GetComponent<RandomColorGenerator>().PopUpTime;
        yield return new WaitForSeconds(PopUpTime);
        Debug.Log(ColorImage.color);

        // Revert the Image's color back to the original.
        ColorImage.enabled = false;
    }
}
