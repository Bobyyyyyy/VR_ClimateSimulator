using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Diagnostica temporanea per capire perché il laser sembra non "cliccare" la UI: logga hover,
// select e (soprattutto) hover/press sulla UI per ogni XRRayInteractor trovato in scena, così si
// vede in Console se il raggio arriva sul bottone e se il click viene davvero registrato.
public class RayInteractorDebugLogger : MonoBehaviour
{
    void Start()
    {
        XRRayInteractor[] rayInteractors = FindObjectsByType<XRRayInteractor>(FindObjectsSortMode.None);
        Debug.Log($"[RayDebug] Trovati {rayInteractors.Length} XRRayInteractor in scena.");

        foreach (XRRayInteractor ray in rayInteractors)
        {
            string name = ray.gameObject.name;
            Debug.Log($"[RayDebug] '{name}' -> enabled={ray.enabled}, activeInHierarchy={ray.gameObject.activeInHierarchy}, enableUIInteraction={ray.enableUIInteraction}");

            ray.hoverEntered.AddListener(args => Debug.Log($"[RayDebug] '{name}' HOVER ENTER su '{args.interactableObject.transform.name}'"));
            ray.hoverExited.AddListener(args => Debug.Log($"[RayDebug] '{name}' HOVER EXIT"));
            ray.selectEntered.AddListener(args => Debug.Log($"[RayDebug] '{name}' SELECT ENTER su '{args.interactableObject.transform.name}'"));
            ray.selectExited.AddListener(args => Debug.Log($"[RayDebug] '{name}' SELECT EXIT"));
            ray.uiHoverEntered.AddListener(args => Debug.Log($"[RayDebug] '{name}' UI HOVER ENTER su '{args.uiObject.name}'"));
            ray.uiHoverExited.AddListener(args => Debug.Log($"[RayDebug] '{name}' UI HOVER EXIT su '{args.uiObject.name}'"));
        }
    }
}
