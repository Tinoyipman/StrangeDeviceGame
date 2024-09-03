using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCamera;
    private List <Texture> placeholders = new();
    private int state = 0;

    [SerializeField] private RawImage[] Images;
    [SerializeField] private AspectRatioFitter fit;




    // Start is called before the first frame update
    void Start()
    {
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
            if (/*!devices[i].isFrontFacing*/ devices[i].name == "HD Webcam" )
            {
                backCamera = new WebCamTexture(devices[i].name,Screen.width,Screen.height);
            }

        }

        if(backCamera == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCamera.Play();
        Images[state].texture = backCamera;
        camAvailable = true;

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

            if (state > Images.Length)
            {
                state = 0;
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
            Images[1].texture = placeholders[1];
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
}
