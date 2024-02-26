using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using PokeApiNet;
using System;
using Cysharp.Threading.Tasks;

public class PokemonGridElement : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Button button;

    public Func<UniTask> InitDetails;

    PokemonCompactData pokemonData;

    void Awake()
    {
        button.onClick.AddListener(DisplayPokemonDetails);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
        Destroy(image.sprite);
    }


    public void Init(PokemonCompactData pokemonData)
    {
        this.pokemonData = pokemonData;
        nameText.text = pokemonData.pokemonBaseData.name.FirstCharacterToUpper();
        image.sprite = pokemonData.pokemonBaseData.sprite;
    }

    async void DisplayPokemonDetails()
    {
        if (InitDetails != null)
            await InitDetails();

        StatisticsPanel panel = PanelManager.instance.GetPanel<StatisticsPanel>();
        panel.Init(pokemonData);
        panel.Show();
    }
}
