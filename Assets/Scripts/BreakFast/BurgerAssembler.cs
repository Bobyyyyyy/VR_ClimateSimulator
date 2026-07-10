using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BurgerAssembler : MonoBehaviour
{
    public enum Stage { Sauces, Hamburger, Egg, TopBun, Complete }

    [Header("Riferimenti")]
    public Panino panino;
    public XRSocketInteractor hamburgerSocket;
    public XRSocketInteractor eggSocket;
    public XRSocketInteractor topBunSocket;
    public XRGrabInteractable finalGrab;

    [Header("Eventi")]
    public UnityEvent onSaucesDone;
    public UnityEvent onHamburgerPlaced;
    public UnityEvent onEggPlaced;
    public UnityEvent onAssemblyComplete;

    [SerializeField] private Stage currentStage = Stage.Sauces;
    public Stage CurrentStage => currentStage;

    void Start()
    {
        SetSocket(hamburgerSocket, false);
        SetSocket(eggSocket, false);
        SetSocket(topBunSocket, false);
        if (finalGrab != null) finalGrab.enabled = false;

        // Durante l'assemblaggio il panino resta kinematic e senza gravità
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            Debug.Log($"[Assembler] Start: Rigidbody trovato su '{name}' → kinematic=ON, gravity=OFF");
        }
        else
        {
            Debug.LogWarning($"[Assembler] Start: NESSUN Rigidbody su '{name}'! GetComponent fallito.");
        }

        Debug.Log($"[Assembler] Start: finalGrab = {(finalGrab != null ? finalGrab.name : "NULL")}, enabled={finalGrab?.enabled}");

        if (hamburgerSocket != null) hamburgerSocket.selectEntered.AddListener(OnHamburgerSocketed);
        if (eggSocket != null) eggSocket.selectEntered.AddListener(OnEggSocketed);
        if (topBunSocket != null) topBunSocket.selectEntered.AddListener(OnTopBunSocketed);
    }

    void OnDestroy()
    {
        if (hamburgerSocket != null) hamburgerSocket.selectEntered.RemoveListener(OnHamburgerSocketed);
        if (eggSocket != null) eggSocket.selectEntered.RemoveListener(OnEggSocketed);
        if (topBunSocket != null) topBunSocket.selectEntered.RemoveListener(OnTopBunSocketed);
    }

    void Update()
    {
        if (currentStage == Stage.Sauces && panino != null
            && panino.HasSauce(SauceType.Maionese)
            && panino.HasSauce(SauceType.Ketchup))
        {
            GoTo(Stage.Hamburger);
        }
    }

    private void OnHamburgerSocketed(SelectEnterEventArgs args)
    {
        Debug.Log($"[Assembler] hamburgerSocket selectEntered: '{args.interactableObject.transform.name}', stage={currentStage}");
        if (currentStage != Stage.Hamburger) return;
        var go = args.interactableObject.transform.gameObject;
        var cookable = go.GetComponent<CookableFood>();
        if (cookable == null || !cookable.IsCooked || cookable.IsBurnt)
        {
            Debug.Log($"[Assembler] Hamburger rifiutato: cookable={(cookable != null)}, cooked={cookable?.IsCooked}, burnt={cookable?.IsBurnt}");
            return;
        }

        GoTo(Stage.Egg);
        StartCoroutine(LockNextFrame(go, hamburgerSocket));
    }

    private void OnEggSocketed(SelectEnterEventArgs args)
    {
        Debug.Log($"[Assembler] eggSocket selectEntered: '{args.interactableObject.transform.name}', stage={currentStage}");
        if (currentStage != Stage.Egg) return;
        var go = args.interactableObject.transform.gameObject;
        var cookable = go.GetComponent<CookableFood>();
        if (cookable == null || !cookable.IsCooked || cookable.IsBurnt)
        {
            Debug.Log($"[Assembler] Uovo rifiutato: cookable={(cookable != null)}, cooked={cookable?.IsCooked}, burnt={cookable?.IsBurnt}");
            return;
        }

        GoTo(Stage.TopBun);
        StartCoroutine(LockNextFrame(go, eggSocket));
    }

    private void OnTopBunSocketed(SelectEnterEventArgs args)
    {
        Debug.Log($"[Assembler] topBunSocket selectEntered: '{args.interactableObject.transform.name}', stage={currentStage}");
        if (currentStage != Stage.TopBun)
        {
            Debug.Log($"[Assembler] Top bun ignorato: stage attuale è {currentStage}, non TopBun");
            return;
        }
        var go = args.interactableObject.transform.gameObject;

        GoTo(Stage.Complete);
        StartCoroutine(LockNextFrame(go, topBunSocket));
    }

    private void GoTo(Stage next)
    {
        Debug.Log($"[Assembler] GoTo: {currentStage} → {next}");
        currentStage = next;
        switch (next)
        {
            case Stage.Hamburger:
                SetSocket(hamburgerSocket, true);
                onSaucesDone?.Invoke();
                Debug.Log("Salse OK → metti l'hamburger cotto");
                break;
            case Stage.Egg:
                SetSocket(hamburgerSocket, false);
                SetSocket(eggSocket, true);
                onHamburgerPlaced?.Invoke();
                Debug.Log("Hamburger OK → metti l'uovo cotto");
                break;
            case Stage.TopBun:
                SetSocket(eggSocket, false);
                SetSocket(topBunSocket, true);
                onEggPlaced?.Invoke();
                Debug.Log("Uovo OK → metti il pane sopra");
                break;
            case Stage.Complete:
                SetSocket(topBunSocket, false);

                foreach (var s in GetComponentsInChildren<XRSocketInteractor>())
                    s.socketActive = false;

                CleanupChildColliders();
                FitColliderToVisual();

                var burgerRb = GetComponent<Rigidbody>();
                if (burgerRb != null)
                {
                    burgerRb.isKinematic = false;
                    burgerRb.useGravity = true;
                    Debug.Log($"[Assembler] Complete: fisica attivata → kinematic={burgerRb.isKinematic}, gravity={burgerRb.useGravity}");
                }
                else
                {
                    Debug.LogWarning($"[Assembler] Complete: NESSUN Rigidbody su '{name}'!");
                }

                // Conta i collider rimasti sul burger (root, non figli distrutti)
                var ownColliders = GetComponents<Collider>();
                Debug.Log($"[Assembler] Complete: collider sul root '{name}' = {ownColliders.Length}");

                if (finalGrab != null)
                {
                    finalGrab.enabled = true;
                    Debug.Log($"[Assembler] Complete: finalGrab '{finalGrab.name}' abilitato, colliders registrati = {finalGrab.colliders.Count}");
                }
                else
                {
                    Debug.LogWarning("[Assembler] Complete: finalGrab è NULL!");
                }

                onAssemblyComplete?.Invoke();
                Debug.Log("Panino pronto da mangiare!");

                // Controlla lo stato del rigidbody dopo 1 secondo
                StartCoroutine(CheckPhysicsLater());
                break;
        }
    }

    private IEnumerator CheckPhysicsLater()
    {
        if (finalGrab != null && finalGrab.isSelected)
        {
            foreach (var interactor in finalGrab.interactorsSelecting)
                Debug.Log($"[Assembler] Burger SELEZIONATO da: '{interactor.transform.name}'");
        }
        yield return new WaitForSeconds(1f);
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Debug.Log($"[Assembler] Check dopo 1s: kinematic={rb.isKinematic}, gravity={rb.useGravity}, velocity={rb.linearVelocity}, pos={transform.position}");
        }
        if (finalGrab != null)
        {
            Debug.Log($"[Assembler] Check dopo 1s: finalGrab.enabled={finalGrab.enabled}, isSelected={finalGrab.isSelected}, colliders={finalGrab.colliders.Count}");
        }
    }

    private void SetSocket(XRSocketInteractor socket, bool active)
    {
        if (socket != null) socket.socketActive = active;
    }

    private IEnumerator LockNextFrame(GameObject go, XRSocketInteractor socket)
    {
        Debug.Log($"[Assembler] LockNextFrame START per '{go.name}'");
        yield return null;

        var grab = go.GetComponent<XRGrabInteractable>();

        if (grab != null && grab.isSelected && grab.interactionManager != null)
        {
            Debug.Log($"[Assembler] '{go.name}' è ancora selezionato → CancelInteractableSelection");
            grab.interactionManager.CancelInteractableSelection((IXRSelectInteractable)grab);
            yield return null;
        }

        if (socket != null)
        {
            Transform socketAttach = socket.attachTransform != null ? socket.attachTransform : socket.transform;
            Transform itemAttach = (grab != null && grab.attachTransform != null) ? grab.attachTransform : go.transform;

            Debug.Log($"[Assembler] Allineo '{go.name}': itemAttach='{itemAttach.name}' → socketAttach='{socketAttach.name}'");

            Quaternion rotDelta = socketAttach.rotation * Quaternion.Inverse(itemAttach.rotation);
            go.transform.rotation = rotDelta * go.transform.rotation;

            Vector3 posDelta = socketAttach.position - itemAttach.position;
            go.transform.position += posDelta;
        }

        go.transform.SetParent(transform, true);

        if (grab != null) Destroy(grab);

        var rb = go.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);

        int colCount = 0;
        foreach (var col in go.GetComponentsInChildren<Collider>())
        {
            Destroy(col);
            colCount++;
        }

        Debug.Log($"[Assembler] LockNextFrame DONE per '{go.name}': distrutti grab={(grab != null)}, rb={(rb != null)}, colliders={colCount}");
    }

    private void FitColliderToVisual()
    {
        var box = GetComponent<BoxCollider>();
        if (box == null) return;

        bool first = true;
        Bounds b = new Bounds();
        foreach (var r in GetComponentsInChildren<MeshRenderer>()) // solo mesh, niente particelle
        {
            if (!r.enabled) continue;
            if (first) { b = r.bounds; first = false; }
            else b.Encapsulate(r.bounds);
        }
        if (first) return;

        box.center = transform.InverseTransformPoint(b.center);
        Vector3 ls = transform.lossyScale;
        box.size = new Vector3(b.size.x / ls.x, b.size.y / ls.y, b.size.z / ls.z);

        Debug.Log($"[Assembler] Collider adattato: center={box.center}, size={box.size}");
    }

    private void CleanupChildColliders()
    {
        var rootBox = GetComponent<BoxCollider>();
        foreach (var c in GetComponentsInChildren<Collider>())
        {
            if (c != rootBox && !c.isTrigger)
            {
                Debug.Log($"[Assembler] Disabilito collider fisico residuo: '{c.name}' ({c.GetType().Name})");
                c.enabled = false;
            }
        }
    }
}