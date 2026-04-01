using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VRLoadingOverlay : MonoBehaviour
{

    public GameObject VRPlayer;
    public GameObject LoadingScreen;
    GameObject LoadingScreenClone;

    Slider loadingBar;

    Camera MainCamera;    
    Camera LoadingScreenCamera;
    GameObject overlayCanvas;

    [ContextMenu("BusTravel")]
    public void StartBusTravel() => StartCoroutine(LoadingSequence(15f));
    [ContextMenu("TaxiTravel")]
    public void StartTaxiTravel() => StartCoroutine(LoadingSequence(2f));

    private IEnumerator LoadingSequence(float duration)
    {

        MainCamera = VRPlayer.GetComponentInChildren<Camera>();

        LoadingScreenClone = Instantiate(LoadingScreen, MainCamera.transform.position, MainCamera.transform.rotation);
      
        loadingBar = LoadingScreenClone.GetComponentInChildren<Slider>();
        LoadingScreenCamera = LoadingScreenClone.GetComponent<Camera>();
        
        overlayCanvas = LoadingScreenClone.GetComponentInChildren<Canvas>().gameObject;

        overlayCanvas.SetActive(false);



        VRPlayer.SetActive(false);
        //Disable VRPlayer and set newCamera
        LoadingScreenClone.tag = "MainCamera";
        LoadingScreenClone.SetActive(true);
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
        LoadingScreenClone.SetActive(false); 
        LoadingScreenCamera.tag = "Untagged";
        VRPlayer.SetActive(true);

        Destroy(LoadingScreenClone);
        
    }
}