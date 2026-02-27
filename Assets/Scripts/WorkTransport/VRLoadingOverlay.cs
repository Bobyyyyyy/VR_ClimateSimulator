using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VRLoadingOverlay : MonoBehaviour
{

    public GameObject VRPlayer;
    public GameObject LoadingScreen;

    Slider loadingBar;

    Camera MainCamera;    
    Camera LoadingScreenCamera;

    [Header("UI Elements")]
    GameObject overlayCanvas;

    [ContextMenu("BusTravel")]
    public void StartBusTravel() => StartCoroutine(LoadingSequence(15f));
    [ContextMenu("TaxiTravel")]
    public void StartTaxiTravel() => StartCoroutine(LoadingSequence(2f));

    private IEnumerator LoadingSequence(float duration)
    {


        LoadingScreen = Instantiate(LoadingScreen, MainCamera.transform.position, MainCamera.transform.rotation);
      
        loadingBar = LoadingScreen.GetComponentInChildren<Slider>();
        MainCamera = VRPlayer.GetComponentInChildren<Camera>();
        LoadingScreenCamera = LoadingScreen.GetComponent<Camera>();
        
        overlayCanvas = LoadingScreen.GetComponentInChildren<Canvas>().gameObject;

        overlayCanvas.SetActive(false);



        VRPlayer.SetActive(false);
        //Disable VRPlayer and set newCamera
        LoadingScreen.tag = "MainCamera";
        LoadingScreen.SetActive(true);
        overlayCanvas.SetActive(true);


        
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingBar.value = elapsed / duration;
            yield return null;
        }

        // Loading finished: Enable VRPlayer and disable Camera
        overlayCanvas.SetActive(false);
        LoadingScreen.SetActive(false); 
        LoadingScreenCamera.tag = "Untagged";
        VRPlayer.SetActive(true);

        Destroy(LoadingScreen);
        
    }
}