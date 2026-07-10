using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VRLoadingOverlay : MonoBehaviour
{

    public GameObject VRPlayer;
    public GameObject LoadingScreen;
    GameObject LoadingScreenClone;

    Slider loadingBar;

    Camera MainCamera;
    Camera LoadingScreenCamera;
    GameObject overlayCanvas;

    readonly List<GraphicRaycaster> disabledRaycasters = new List<GraphicRaycaster>();

    [ContextMenu("BusTravel")]
    public void StartBusTravel() => StartCoroutine(LoadingSequence(15f));
    [ContextMenu("TaxiTravel")]
    public void StartCarTravel() => StartCoroutine(LoadingSequence(2f));

    private IEnumerator LoadingSequence(float duration)
    {

        MainCamera = VRPlayer.GetComponentInChildren<Camera>();

        LoadingScreenClone = Instantiate(LoadingScreen, MainCamera.transform.position, MainCamera.transform.rotation);
      
        loadingBar = LoadingScreenClone.GetComponentInChildren<Slider>();
        LoadingScreenCamera = LoadingScreenClone.GetComponent<Camera>();
        
        overlayCanvas = LoadingScreenClone.GetComponentInChildren<Canvas>().gameObject;

        overlayCanvas.SetActive(false);

        // Le canvas world-space che usano la camera del VRPlayer come event camera vanno
        // disattivate finché quella camera resta spenta: altrimenti l'EventSystem continua a
        // proiettare i raycast su una camera disattivata e spamma
        // "Screen position out of view frustum (-nan(ind), -nan(ind))" ogni frame.
        disabledRaycasters.Clear();
        foreach (var raycaster in FindObjectsByType<GraphicRaycaster>(FindObjectsSortMode.None))
        {
            var canvas = raycaster.GetComponent<Canvas>();
            if (raycaster.enabled && canvas != null && canvas.worldCamera == MainCamera)
            {
                raycaster.enabled = false;
                disabledRaycasters.Add(raycaster);
            }
        }

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

        foreach (var raycaster in disabledRaycasters)
            if (raycaster != null) raycaster.enabled = true;
        disabledRaycasters.Clear();

        Destroy(LoadingScreenClone);
        
    }
}