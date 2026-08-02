using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{


    [Title("Layout References")]
    [SerializeField] private LayoutElement _staminaLayout;
    [SerializeField] private LayoutElement _ghostLayout;
    [SerializeField] private LayoutElement _hitLayout;
    [SerializeField] private LayoutElement _poisonLayout;
    [SerializeField] private LayoutElement _hungerLayout;

    [Title("Ghost Animation Settings")]
    [SerializeField] private float _ghostDelay = 0.25f;
    [SerializeField] private float _ghostDuration = 0.4f;
    [SerializeField] private Ease _ghostEase = Ease.OutQuad;
    private Tween _ghostTween;
    private float _currentGhostValue;


    // TODO:
    // maybe dans d'autre script
    // des bouton pour give de la vie au joueur, give du poison, clear ceci cela, un vrai panel digne de ce nom.

    // Comment lié les valeurs genre:
    // Hunger = 0 rien 100 il peu plus sprinter et il meurt car ça a tout rempli.
    // Poison = 0 rien 100 pareil
    // Hit = 0 full vie donc la stamina 100 sa barre est rempli de rouge de hit. (c'est life dans notre code, il faut qu'on refacto tout et qu'on se mette OK sur des nom)
    // Et une fonction sprint ! qui sera lié a la barre espace avec le panel de bouton. (appui ça sprint, relache ça arrête et ça remonte doucement.)
    // ce sprint diminuera notre stamina (100 tout vert défaut début du jeu a 0 rien, mais c'est Ghost qui s'augmente dans notre UI pour représenter ça)

    // probleme: il faut lier ce 0 a 100 pour notre width qui est chelou, genre nous c'est 940 de large en tout, et puis plus on rajoute des valeurs sur des truc, plus j'ai l'impression que c'est pas précis car Unity avec ses layout doit ptet faire des % pour que tout les preffered sois d'accord entre eux.
    // plus tard il faudrat donc comme dans peak la rendre dépassable, et le dépassement c'est des pointillé quoi.
    // c'est une étape plus lointaine.

    //plus besoin de setactive parceque plus de spacing a gerer !

    [Button("Test Update Bar", ButtonSizes.Medium)]
    public void UpdateHealthBar(float stamina, float hitDamage, float poison, float hunger)
    {
        //update flexibleWidth (0 -> 100)
        _staminaLayout.flexibleWidth = Mathf.Max(0, stamina);
        _hitLayout.flexibleWidth = Mathf.Max(0, hitDamage);
        _poisonLayout.flexibleWidth = Mathf.Max(0, poison);
        _hungerLayout.flexibleWidth = Mathf.Max(0, hunger);

        // animation
        float targetGhost = Mathf.Max(0, 100f - (stamina + hitDamage + poison + hunger));
        _ghostTween?.Kill();
        _ghostTween = DOVirtual.Float(_currentGhostValue, targetGhost, _ghostDuration, value =>
        {
            _currentGhostValue = value;
            _ghostLayout.flexibleWidth = value;
        })
        .SetDelay(_ghostDelay)
        .SetEase(_ghostEase);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
