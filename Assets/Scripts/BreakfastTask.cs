using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        // utilizzare una scena diversa? voglio fare in modo che l'utente si ritrovi davanti a ciò che deve fare
    }

    public void CompleteStep(int step)
    {
        switch(step) {
            case 0:
                if(this.step == 0)
                {
                     //Spremuta
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
