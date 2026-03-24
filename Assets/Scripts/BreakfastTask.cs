using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BreakfastTask : MonoBehaviour
{
   
    public static BreakfastTask Instance;

    public List<GameObject> prefabs;
     

    public TaskData task;

    void Awake() => Instance = this;

    private int step = 0;

    void beginTask()
    {
        // In questa funzione andrà messa la logica per far partire la task, come se fosse un minigame,
        // con apposito tracker per vedere a che step sono e per dire cosa fare.
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
                     //rendo la tazza grabbabile
                     GameObject Mug = prefabs.Find(g => g.name == "Mug");

                     Mug.AddComponent<Rigidbody>();
                     Mug.AddComponent<XRGrabInteractable>();

                }
            break;
            case 1:
                if(this.step == 1)
                {
                     //Spalmare marmellata sul toast 
                }
              
            break;
            case 2:
                if(this.step == 2)
                {
                       // Mangiare
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
