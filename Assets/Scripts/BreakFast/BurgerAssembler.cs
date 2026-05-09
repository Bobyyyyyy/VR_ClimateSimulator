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

        if (hamburgerSocket != null) hamburgerSocket.selectEntered.AddListener(OnHamburgerSocketed);
        if (eggSocket != null)       eggSocket.selectEntered.AddListener(OnEggSocketed);
        if (topBunSocket != null)    topBunSocket.selectEntered.AddListener(OnTopBunSocketed);
    }

    void OnDestroy()
    {
        if (hamburgerSocket != null) hamburgerSocket.selectEntered.RemoveListener(OnHamburgerSocketed);
        if (eggSocket != null)       eggSocket.selectEntered.RemoveListener(OnEggSocketed);
        if (topBunSocket != null)    topBunSocket.selectEntered.RemoveListener(OnTopBunSocketed);
    }

    void Update()
    {
        // Solo le salse sono pollate (vengono spruzzate, non socketate)
        if (currentStage == Stage.Sauces && panino != null
            && panino.HasSauce(SauceType.Maionese)
            && panino.HasSauce(SauceType.Ketchup))
        {
            GoTo(Stage.Hamburger);
        }
    }

    private void OnHamburgerSocketed(SelectEnterEventArgs args)
    {
        if (currentStage != Stage.Hamburger) return;
        var go = args.interactableObject.transform.gameObject;
        var cookable = go.GetComponent<CookableFood>();
        if (cookable == null || !cookable.IsCooked || cookable.IsBurnt) return;

        LockItem(go);
        GoTo(Stage.Egg);
    }

    private void OnEggSocketed(SelectEnterEventArgs args)
    {
        if (currentStage != Stage.Egg) return;
        var go = args.interactableObject.transform.gameObject;
        var cookable = go.GetComponent<CookableFood>();
        if (cookable == null || !cookable.IsCooked || cookable.IsBurnt) return;

        LockItem(go);
        GoTo(Stage.TopBun);
    }

    private void OnTopBunSocketed(SelectEnterEventArgs args)
    {
        if (currentStage != Stage.TopBun) return;
        var go = args.interactableObject.transform.gameObject;

        LockItem(go);
        GoTo(Stage.Complete);
    }

    private void GoTo(Stage next)
    {
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
                if (finalGrab != null) finalGrab.enabled = true;
                onAssemblyComplete?.Invoke();
                Debug.Log("Panino pronto da mangiare!");
                break;
        }
    }

    private void SetSocket(XRSocketInteractor socket, bool active)
    {
        if (socket != null) socket.socketActive = active;
    }

    private void LockItem(GameObject go)
    {
        // Imparenta al pane così segue i movimenti
        go.transform.SetParent(transform, true);
        // Kinematic così non cade
        var rb = go.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        // Disabilita grab → non si può più riprendere
        var grab = go.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;
    }
}