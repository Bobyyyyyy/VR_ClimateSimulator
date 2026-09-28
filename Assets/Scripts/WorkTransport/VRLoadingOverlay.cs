using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class VRLoadingOverlay : MonoBehaviour
{

    public GameObject VRPlayer;
    public GameObject LoadingScreen;
    GameObject LoadingScreenClone;

    [Header("Task giornata (opzionale)")]
    [Tooltip("Il lavoro non assegna punti: è solo una schermata di attesa tra colazione e cena")]
    public TaskData workTask;

    [Tooltip("Canvas per andare a lavoro: visibile solo quando è il turno del task lavoro (colazione completata)")]
    public GameObject doorCanvas;

    Slider loadingBar;

    Camera MainCamera;
    Camera LoadingScreenCamera;
    GameObject overlayCanvas;

    readonly List<GraphicRaycaster> disabledRaycasters = new List<GraphicRaycaster>();

    [ContextMenu("BusTravel")]
    public void StartBusTravel()
    {
        ActionLogger.Instance?.LogAction(workTask != null ? workTask.taskName : "Work", "Scelta trasporto", "Mezzo scelto=Autobus");
        StartCoroutine(LoadingSequence(15f));
    }

    [ContextMenu("TaxiTravel")]
    public void StartCarTravel()
    {
        ActionLogger.Instance?.LogAction(workTask != null ? workTask.taskName : "Work", "Scelta trasporto", "Mezzo scelto=Auto");
        StartCoroutine(LoadingSequence(2f));
    }

    void Start()
    {
        UpdateDoorCanvasVisibility();

        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.AddListener(OnTaskChanged);
    }

    void OnDestroy()
    {
        if (TaskManager.Instance != null)
            TaskManager.Instance.onTaskChanged.RemoveListener(OnTaskChanged);
    }

    private void OnTaskChanged(TaskData newCurrentTask)
    {
        UpdateDoorCanvasVisibility();
    }

    private void UpdateDoorCanvasVisibility()
    {
        if (doorCanvas == null) return;

        bool shouldShow = workTask != null && !workTask.isCompleted
            && TaskManager.Instance != null && TaskManager.Instance.IsCurrentTask(workTask);

        doorCanvas.SetActive(shouldShow);
    }

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

        CompleteWorkTask();
    }

    private void CompleteWorkTask()
    {
        if (workTask == null || workTask.isCompleted) return;
        if (TaskManager.Instance != null && !TaskManager.Instance.IsCurrentTask(workTask)) return;

        workTask.isCompleted = true;

        ActionLogger.Instance?.LogAction(workTask.taskName, "Task completata", "");

        TaskManager.Instance.UpdateTaskProgress(workTask); // 0 punti: è solo una schermata di attesa
    }
}