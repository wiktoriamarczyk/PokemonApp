using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionChainDisplay : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] EvolutionElementDisplay evolutionElementPrefab;
    [SerializeField] GameObject relationIndicator;

    List<EvolutionElementDisplay> evolutionElements = new List<EvolutionElementDisplay>();
    List<GameObject> arrows = new List<GameObject>();

    int pokemonId;

    public void Init(int pokemonId)
    {
        this.pokemonId = pokemonId;
        InitData().Forget();
    }

    async UniTask InitData()
    {
        EvolutionChainCompactData evolutionChain = await PokeAPIController.instance.GetPokemonEvolutionChainByPokemonId(pokemonId);
        foreach (var evolutionElementId in evolutionChain.evolutionElementsIds)
        {
            EvolutionElementDisplay evolutionElementDisplay = Instantiate(evolutionElementPrefab, container.transform);
            var pokemon = await PokeAPIController.instance.GetPokemonData(evolutionElementId);
            string pokemonSpriteURL = pokemon.Sprites.Other.OfficialArtwork.FrontDefault;
            Texture2D texture = await PokeAPIController.instance.GetPokemonTexture(pokemonSpriteURL);
            Sprite sprite = texture.GetSprite();
            evolutionElementDisplay.outlineImage.SetImage(sprite, evolutionElementId == pokemonId);
            evolutionElementDisplay.elementName = pokemon.Name;
            evolutionElements.Add(evolutionElementDisplay);
            GameObject arrowObject = Instantiate(this.relationIndicator, container.transform);
            arrows.Add(arrowObject);
        }

        Destroy(arrows[^1]);
        arrows.Remove(arrows[^1]);
    }

    public void Deinit()
    {
        for (int i = 0; i < evolutionElements.Count; ++i)
        {
            Destroy(evolutionElements[i].gameObject);
            if (i < arrows.Count)
            {
                Destroy(arrows[i]);
            }
        }
        evolutionElements.Clear();
        arrows.Clear();
    }

}
