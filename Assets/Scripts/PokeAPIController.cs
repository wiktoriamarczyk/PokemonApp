using UnityEngine;
using Cysharp.Threading.Tasks;
using PokeApiNet;
using System.Collections.Generic;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;
using System;

public class PokeAPIController : Singleton<PokeAPIController>
{
    public HashSet<Pokemon> pokemons = new HashSet<Pokemon>();
    public List<EvolutionChainCompactData> evolutionChains = new List<EvolutionChainCompactData>();

    CancellationTokenSource cancellationTokenSource;
    CancellationToken cancelToken;
    PokeAPIBackend pokeAPIBackend = new PokeAPIBackend();

    int maxPokemonCount;

    const string language = "en";

    void Awake()
    {
        cancellationTokenSource = new CancellationTokenSource();
        cancelToken = cancellationTokenSource.Token;
    }

    void OnDestroy()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }

    public async UniTask<int> GetPokemonCount()
    {
        var data = await pokeAPIBackend.GetApiResourcePageAsync<PokemonApiResource>(1, 0, cancelToken);
        return data.Count;
    }

    public async UniTask<List<Pokemon>> GetPokemonsData(int startIndex, int endIndex)
    {
        bool isRangeValid = await ValidateRange(startIndex, endIndex);
        if (!isRangeValid)
        {
            return null;
        }

        List<Pokemon> pokemons = new List<Pokemon>();
        for (int id = startIndex; id < endIndex; ++id)
        {
            Pokemon pokemon = await GetPokemonData(id);
            if (pokemon != null)
            {
                pokemons.Add(pokemon);
                Debug.Log($"Pokemon [{pokemon.Id}]: {pokemon.Name}");
            }
        }
        return pokemons;
    }

    public async UniTask<Pokemon> GetPokemonData(int id)
    {
        Pokemon pokemon = pokemons.FirstOrDefault(p => p.Id == id);
        if (pokemon != null)
        {
            return pokemon;
        }
        pokemon = await pokeAPIBackend.GetResourceAsync<Pokemon>(id, cancelToken);
        if (pokemon != null)
        {
            pokemons.Add(pokemon);
        }
        return pokemon;
    }

    public async UniTask<Pokemon> GetPokemonData(string name)
    {
        Pokemon pokemon = pokemons.FirstOrDefault(p => p.Name == name);
        if (pokemon != null)
        {
            return pokemon;
        }
        pokemon = await pokeAPIBackend.GetResourceAsync<Pokemon>(name, cancelToken);
        if (pokemon != null)
        {
            pokemons.Add(pokemon);
        }
        return pokemon;
    }

    public async UniTask<Texture2D> GetPokemonTexture(string spriteURL)
    {
        string urlHash = spriteURL.Sha256();
        Texture2D texture = Extensions.LoadTextureFromDisk(urlHash);
        if (texture != null)
        {
            return texture;
        }
        texture = await pokeAPIBackend.GetTexture2DAsync(spriteURL, cancelToken);
        if (texture != null)
        {
            texture.WriteTextureOnDisk(urlHash);
        }
        return texture;
    }

    public async UniTask<string> GetAbilityDescription(string abilityName)
    {
        string result = string.Empty;
        Ability ability = await pokeAPIBackend.GetResourceAsync<Ability>(abilityName, cancelToken);
        return ability.EffectEntries.Find(e => e.Language.Name == language)?.Effect ?? string.Empty;
    }

    public async UniTask<EvolutionChainCompactData> GetPokemonEvolutionChain(string speciesUrl)
    {
        PokemonSpecies species = await pokeAPIBackend.GetResourceByUrlAsync<PokemonSpecies>(speciesUrl, cancelToken);
        if (species == null)
        {
            return null;
        }

        EvolutionChain evolutionChain = await pokeAPIBackend.GetResourceByUrlAsync<EvolutionChain>(species.EvolutionChain.Url, cancelToken);
        if (evolutionChain == null)
        {
            return null;
        }

        int evolutionId = evolutionChain.Id;
        EvolutionChainCompactData evolutionChainData = evolutionChains.FirstOrDefault(e => e.evolutionId == evolutionId);
        if (evolutionChainData != null)
        {
            return evolutionChainData;
        }

        evolutionChainData = new EvolutionChainCompactData();
        evolutionChainData.evolutionElementsIds = new List<int>();
        evolutionChainData.evolutionId = evolutionId;

        ChainLink chain = evolutionChain.Chain;
        string pokemonName = chain.Species.Name;
        Pokemon pokemon = await GetPokemonData(pokemonName);
        if (pokemon == null)
        {
               return null;
        }
        evolutionChainData.evolutionElementsIds.Add(pokemon.Id);

        if (chain.EvolvesTo == null || chain.EvolvesTo.Count == 0)
        {
            evolutionChains.Add(evolutionChainData);
            return evolutionChainData;
        }

        do
        {
            pokemonName = chain.EvolvesTo[0].Species.Name;
            pokemon = await GetPokemonData(pokemonName);
            evolutionChainData.evolutionElementsIds.Add(pokemon.Id);
            chain = chain.EvolvesTo[0];

        } while (chain.EvolvesTo.Count > 0);

        evolutionChains.Add(evolutionChainData);

        return evolutionChainData;
    }

    public async UniTask<EvolutionChainCompactData> GetPokemonEvolutionChainByPokemonId(int pokemonId)
    {
        EvolutionChainCompactData evolutionData = evolutionChains.FirstOrDefault(e => e.evolutionElementsIds.Contains(pokemonId));
        if (evolutionData != null)
        {
            return evolutionData;
        }
        Pokemon pokemon = await GetPokemonData(pokemonId);
        return await GetPokemonEvolutionChain(pokemon.Species.Url);

    }

    async UniTask<bool> ValidateRange(int startIndex, int endIndex)
    {
        if (maxPokemonCount == default)
        {
            maxPokemonCount = await GetPokemonCount();
        }

        if (startIndex < 1 || endIndex > maxPokemonCount)
        {
            Debug.LogError("Invalid range");
            return false;
        }
        return true;
    }
}
