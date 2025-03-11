using UnityEngine;
using UnityEngine.UI;

public class WebCamDuplicate : MonoBehaviour
{
    public WebCamTex mainWebCam;
    private bool textureAssigned = false;

    void Update()
    {
        if (mainWebCam == null)
        {
            mainWebCam = FindObjectOfType<WebCamTex>();
        }

        if (mainWebCam != null && mainWebCam.backCam != null && mainWebCam.backCam.isPlaying && !textureAssigned)
        {
            AssignTexture();
            textureAssigned = true;
        }
    }

    void AssignTexture()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.mainTexture = mainWebCam.backCam;
        }
        else
        {
            RawImage rawImage = GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = mainWebCam.backCam;
            }
        }
    }
}