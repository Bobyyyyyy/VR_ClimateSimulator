using UnityEngine;

public class BinDetector : MonoBehaviour
{

    public TrashType BinType;

    private void OnTriggerEnter(Collider other)
    {
        // Il tag "Trash" e lo script TrashScript stanno sull'oggetto radice del prefab del rifiuto,
        // ma il collider/rigidbody che genera il trigger può stare su un figlio (mesh importata).
        // Risaliamo i genitori fino a trovare quello taggato "Trash" (non fino alla radice della scena:
        // il rifiuto può essere annidato dentro la gerarchia della stanza).
        GameObject collidedObject = FindTrashRoot(other.transform);
        if (collidedObject == null) return;

        TrashScript trashScript = collidedObject.GetComponent<TrashScript>();
        TrashType? actualType = trashScript != null && trashScript.data != null ? trashScript.data.type : (TrashType?)null;
        string taskName = TrashTask.Instance != null && TrashTask.Instance.trashTask != null
            ? TrashTask.Instance.trashTask.taskName : "Trash";

        if (BinType == TrashType.undifferentiated)
        {
            // User decided to throw in the big undifferentiated bin instead of separating it
            TrashTask.Instance.NotifyWrongSort();
            TrashTask.Instance.NotifyTrashInsertion();

            ActionLogger.Instance?.LogAction(taskName, "Rifiuto gettato",
                $"Oggetto={collidedObject.name} | MaterialeReale={FormatType(actualType)} | CestinoScelto=Indifferenziata | Corretto=No");

            Destroy(collidedObject);
            return;
        }

        bool isCorrectBin = actualType.HasValue && actualType.Value == BinType;

        if (isCorrectBin)
        {
            TrashTask.Instance.NotifyCorrectSort();
            TrashTask.Instance.NotifyTrashInsertion();

            ActionLogger.Instance?.LogAction(taskName, "Rifiuto gettato",
                $"Oggetto={collidedObject.name} | MaterialeReale={FormatType(actualType)} | CestinoScelto={BinType} | Corretto=Si");

            Destroy(collidedObject);
        }
        else
        {
            ActionLogger.Instance?.LogAction(taskName, "Tentativo di rifiuto respinto",
                $"Oggetto={collidedObject.name} | MaterialeReale={FormatType(actualType)} | CestinoScelto={BinType} | Corretto=No");

            if (other.attachedRigidbody != null) other.attachedRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    private static string FormatType(TrashType? type) => type.HasValue ? type.Value.ToString() : "sconosciuto";

    private static GameObject FindTrashRoot(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Trash")) return t.gameObject;
            t = t.parent;
        }
        return null;
    }
}
