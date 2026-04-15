using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

    public class BreakfastTask : MonoBehaviour
{
   
    public static BreakfastTask Instance;

    public List<GameObject> BreakFastPrefabs;
    public List<GameObject> SnackPrefabs;

    public GameObject BreakfastCanvas;

     

    public TaskData task;

    void Awake() => Instance = this;

    private int step = 0;


    [ContextMenu("Test begin task")]
    public void beginTask(bool isHealthy)
    {
        if(isHealthy)
        {
            foreach (var prefab in BreakFastPrefabs) {
                Instantiate(prefab);
            }
        }
        else
        {
            foreach (var prefab in SnackPrefabs) {
                Instantiate(prefab);
            }
        }

        Destroy(BreakfastCanvas);
       
    }



    [ContextMenu("Test Step 0")]
    void TestStep0() { CompleteStep(0); }

    [ContextMenu("Test Step 1")]
    void TestStep1() { CompleteStep(1); }

    [ContextMenu("Test Step 2")]
    void TestStep2() { CompleteStep(2); }

    public void CompleteStep(int step)
    {
        switch(step) {
            case 0:
                if(this.step == 0)
                {
                     GameObject Mug = BreakFastPrefabs.Find(g => g.name == "Mug");

                     Mug.AddComponent<Rigidbody>();
                     Mug.AddComponent<XRGrabInteractable>();
                }
            break;
            case 1:
                if(this.step == 1)
                {
                    GameObject Toast = BreakFastPrefabs.Find(g => g.name == "Toast");

                     Toast.AddComponent<Rigidbody>();
                     Toast.AddComponent<XRGrabInteractable>(); 
                }
              
            break;
            case 2:
                if(this.step == 2)
                {
                    CompleteTask();
                }
             
            break;
        }
    }


    public void CompleteTask()
    {
    if (task != null && !task.isCompleted)
    {
        task.isCompleted = true;
        TaskManager.Instance.UpdateTaskProgress(task);
    }

    }

}
