using UnityEngine;

public class RecipePanel : MonoBehaviour
{
    [Header("Riferimenti")]
    public BurgerAssembler assembler;
    public Panino panino;

    [Header("Voci della ricetta")]
    public RecipeItem maioneseItem;
    public RecipeItem ketchupItem;
    public RecipeItem hamburgerItem;
    public RecipeItem uovoItem;
    public RecipeItem paneSopraItem;

    private bool maioneseDone, ketchupDone, hamburgerDone, uovoDone, paneSopraDone;

    void Start()
    {
        if (assembler != null)
        {
            assembler.onHamburgerPlaced.AddListener(() => { hamburgerDone = true; hamburgerItem?.SetDone(true); });
            assembler.onEggPlaced.AddListener(() => { uovoDone = true; uovoItem?.SetDone(true); });
            assembler.onAssemblyComplete.AddListener(() => { paneSopraDone = true; paneSopraItem?.SetDone(true); });
        }
    }

    void Update()
    {
        if (panino == null) return;
        if (!maioneseDone && panino.HasSauce(SauceType.Maionese))
        {
            maioneseDone = true;
            maioneseItem?.SetDone(true);
        }
        if (!ketchupDone && panino.HasSauce(SauceType.Ketchup))
        {
            ketchupDone = true;
            ketchupItem?.SetDone(true);
        }
    }
}