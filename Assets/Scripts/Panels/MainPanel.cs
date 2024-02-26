using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Windows;
using DG.Tweening;

public class MainPanel : Panel
{
    [SerializeField] Button nextPageButton;
    [SerializeField] Button previousPageButton;
    [SerializeField] Button pokemonSearchButton;
    [SerializeField] TMP_InputField pokemonSearch;
    [SerializeField] PokemonGrid pokemonGrid;
    [SerializeField] GameObject emptyPanelInfo;
    [SerializeField] int _currentPage = 1;
    [SerializeField] RectTransform pokemonGridRect;

    Tween currentTween;
    Sequence sequence;
    const float animationDuration = 0.5f;
    int currentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            if (currentPage > 1)
            {
                previousPageButton.gameObject.SetActive(true);
            }
            else
            {
                previousPageButton.gameObject.SetActive(false);
            }
        }
    }

    void Awake()
    {
        nextPageButton.onClick.AddListener(() => InitPageData(true));
        previousPageButton.onClick.AddListener(() => InitPageData(false));
        previousPageButton.gameObject.SetActive(false);
        pokemonSearchButton.onClick.AddListener(DisplaySearchedPokemon);
        pokemonSearch.onValueChanged.AddListener(OnPokemonSearchInputFieldChange);
        pokemonGrid.onPokemonDisplay += OnPokemonSearched;
        emptyPanelInfo.SetActive(false);
    }

    private void OnDestroy()
    {
        nextPageButton.onClick.RemoveAllListeners();
        previousPageButton.onClick.RemoveAllListeners();
        pokemonSearchButton.onClick.RemoveAllListeners();
        pokemonSearch.onValueChanged.RemoveAllListeners();
        pokemonGrid.onPokemonDisplay -= OnPokemonSearched;
    }

    public override void Init()
    {
        pokemonGrid.CreatePokemonsGrid(currentPage).Forget();
    }

    void InitPageData(bool forward)
    {
        sequence = DOTween.Sequence();
        currentTween?.Kill();
        if (forward)
        {
            currentPage++;
            sequence.Append(pokemonGridRect.DOAnchorPosX(-Screen.width*2, animationDuration));
            sequence.Append(pokemonGridRect.DOAnchorPosX(Screen.width*2, 0f));
        }
        else
        {
            currentPage--;;
            sequence.Append(pokemonGridRect.DOAnchorPosX(Screen.width*2, animationDuration));
            sequence.Append(pokemonGridRect.DOAnchorPosX(-Screen.width*2, 0));
        }

        CreatePokemonsGrid().Forget();
    }

    async UniTask CreatePokemonsGrid()
    {
        await pokemonGrid.CreatePokemonsGrid(currentPage);
        await sequence.AsyncWaitForCompletion();
        currentTween = pokemonGridRect.DOAnchorPosX(0, animationDuration);
    }

    void DisplaySearchedPokemon()
    {
        string pokemonName = pokemonSearch.text.ToLower();
        if (!string.IsNullOrEmpty(pokemonName))
        {
            pokemonGrid.DisplayPokemon(pokemonName).Forget();
        }
    }

    void OnPokemonSearchInputFieldChange(string input)
    {
        if (string.IsNullOrEmpty(input) && pokemonGrid.displayedPokemons < 2)
        {
            emptyPanelInfo.SetActive(false);
            pokemonGrid.CreatePokemonsGrid(currentPage).Forget();
        }
    }

    void OnPokemonSearched(PokemonCompactData pokemonData)
    {
        if (pokemonData == null)
        {
            emptyPanelInfo.SetActive(true);
        }
        else
        {
            emptyPanelInfo.SetActive(false);
        }
    }

}
