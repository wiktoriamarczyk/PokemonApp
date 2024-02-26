using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Extensions;

public class StatisticsPanel : Panel
{
    [SerializeField] Button previousPageButton;
    [SerializeField] EvolutionChainDisplay evolutionChainDisplay;
    [SerializeField] TextMeshProUGUI pokemonName;
    [SerializeField] TextMeshProUGUI pokemonWeight;
    [SerializeField] TextMeshProUGUI pokemonHeight;
    [SerializeField] TextMeshProUGUI pokemonXP;
    [SerializeField] StatisticsContainer statistics;
    [SerializeField] TypesContainer types;
    [SerializeField] AbilitiesContainer abilities;

    RectTransform rect;
    Tween currentTween;

    PokemonCompactData pokemonData;
    Color styleColor;

    const int fixedPointDivider = 10;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        previousPageButton.onClick.AddListener(ReturnToMainPanel);
    }

    private void OnDestroy()
    {
        previousPageButton.onClick.RemoveAllListeners();
        rect.DOKill();
    }

    public override void Show()
    {
        base.Show();
        currentTween?.Kill();
        currentTween = rect.DOScale(0, 0);
        currentTween = rect.DOScale(1, 0.3f);
    }

    public override void Hide()
    {
        currentTween?.Kill();
        currentTween = rect.DOScale(0, 0.3f);
        currentTween.onComplete += base.Hide;
    }

    public void Init(PokemonCompactData pokemonData)
    {
        if (this.pokemonData == pokemonData)
        {
            return;
        }
        this.pokemonData = pokemonData;
        styleColor = pokemonData.pokemonBaseData.sprite.GetDominantColor();
        evolutionChainDisplay.Deinit();
        evolutionChainDisplay.Init(pokemonData.pokemonBaseData.id);
        InitPokemonMainData();
        statistics.Init(pokemonData.pokemonBaseData.statistics, styleColor);
        types.Init(pokemonData.pokemonBaseData.types, styleColor);
        abilities.Init(pokemonData.pokemonExtendedData.abilities, styleColor);
    }

    void InitPokemonMainData()
    {
        pokemonName.text = pokemonData.pokemonBaseData.name.FirstCharacterToUpper();
        float weight = float.Parse(pokemonData.pokemonBaseData.weight) / fixedPointDivider;
        pokemonWeight.text = weight.ToString();
        float height = float.Parse(pokemonData.pokemonBaseData.height) / fixedPointDivider;
        pokemonHeight.text = height.ToString();
        int xpValue = pokemonData.pokemonBaseData.xp ?? 0;
        pokemonXP.text = xpValue.ToString();
    }

    void ReturnToMainPanel()
    {
        PanelManager.instance.ShowPanel<MainPanel>();
        Hide();
    }
}
