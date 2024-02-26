using Cysharp.Threading.Tasks;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonGrid : MonoBehaviour
{
    [SerializeField] Transform pokemonsContainer;
    [SerializeField] PokemonGridElement pokemonPrefab;

    public int displayedPokemons => pokemonGridElements.Count;
    public Action<PokemonCompactData> onPokemonDisplay;

    public List<PokemonGridElement> pokemonGridElements = new List<PokemonGridElement>();
    List<PokemonCompactData> pokemonCompactDatas = new List<PokemonCompactData>();

    const int maxElementsInGrid = 9;

    public async UniTask DisplayPokemon(string pokemonName)
    {
        pokemonCompactDatas.Clear();
        ClearPokemonElements();
        Pokemon pokemon = await PokeAPIController.instance.GetPokemonData(pokemonName);
        PokemonCompactData pokemonData = null;
        if (pokemon != null)
        {
            pokemonData = await InitPokemonBaseData(pokemon);
            InstantiatePokemonElements();
        }
        onPokemonDisplay?.Invoke(pokemonData);
    }

    public async UniTask CreatePokemonsGrid(int page)
    {
        pokemonCompactDatas.Clear();
        int startIndex = (page - 1) * maxElementsInGrid + 1;
        int endIndex = startIndex + maxElementsInGrid;
        List<Pokemon> pokemons = await PokeAPIController.instance.GetPokemonsData(startIndex, endIndex);

        List<UniTask<PokemonCompactData>> pokemonElementsTasks = new List<UniTask<PokemonCompactData>>();

        foreach (Pokemon pokemon in pokemons)
        {
            pokemonElementsTasks.Add(InitPokemonBaseData(pokemon));
        }

        await UniTask.WhenAll(pokemonElementsTasks);
        ClearPokemonElements();
        InstantiatePokemonElements();
    }

    async UniTask<PokemonCompactData> InitPokemonBaseData(Pokemon pokemon)
    {
        string spriteURL = pokemon.Sprites.Other.OfficialArtwork.FrontDefault;
        Texture2D texture = await PokeAPIController.instance.GetPokemonTexture(spriteURL);
        Sprite sprite = texture.GetSprite();
        PokemonCompactData pokemonData = new PokemonCompactData();
        pokemonData.InitPokemonBaseData(pokemon, sprite);
        pokemonCompactDatas.Add(pokemonData);
        //await InitPokemonExtendedData(pokemonData);
        return pokemonData;
    }

    async UniTask<PokemonCompactData> InitPokemonExtendedData(PokemonCompactData pokemonData)
    {
        var pokemonCompactData = pokemonCompactDatas.Find(p => p == pokemonData);
        pokemonCompactData.pokemonExtendedData = new PokemonExtendedData();
        pokemonCompactData.pokemonExtendedData.abilities = new List<AbilityCompactData>();
        int pokemonId = pokemonData.pokemonBaseData.id;
        Pokemon pokemon = PokeAPIController.instance.pokemons.First(p => p.Id == pokemonId);
        if (pokemon == null)
        {
            pokemon = await PokeAPIController.instance.GetPokemonData(pokemonId);
        }
        foreach (var ability in pokemon.Abilities)
        {
            string descriptionURL = await PokeAPIController.instance.GetAbilityDescription(ability.Ability.Name);
            pokemonCompactData.pokemonExtendedData.abilities.Add(new() {
                name = ability.Ability.Name,
                description = descriptionURL
            });
        }
        var evolutionChain = await PokeAPIController.instance.GetPokemonEvolutionChain(pokemon.Species.Url);
        pokemonData.InitPokemonExtendedData(evolutionChain.evolutionId, pokemonCompactData.pokemonExtendedData.abilities);
        return pokemonData;
    }

    void InstantiatePokemonElements()
    {
        pokemonCompactDatas.Sort((x, y) => x.pokemonBaseData.id.CompareTo(y.pokemonBaseData.id));
        foreach (var pokemonData in pokemonCompactDatas)
        {
            PokemonGridElement pokemonElement = Instantiate(pokemonPrefab, pokemonsContainer.transform);
            pokemonElement.Init(pokemonData);
            pokemonElement.InitDetails = async () => await InitPokemonExtendedData(pokemonData);
            pokemonGridElements.Add(pokemonElement);
        }
    }

    void ClearPokemonElements()
    {
        foreach (var pokemonElement in pokemonGridElements)
        {
            Destroy(pokemonElement.gameObject);
        }
        pokemonGridElements.Clear();
    }
}
