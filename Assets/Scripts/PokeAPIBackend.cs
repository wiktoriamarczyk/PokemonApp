using PokeApiNet;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Reflection;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using System.Diagnostics;
using System;

public class PokeAPIBackend
{
    const string baseURL = "https://pokeapi.co/api/v2";

    public async UniTask<T> GetResourceAsync<T>(int id, CancellationToken cancellationToken)
           where T : ResourceBase
    {
        T resource = await GetResourcesWithParamsAsync<T>(id.ToString(), cancellationToken);
        return resource;
    }

    public async UniTask<T> GetResourceAsync<T>(string name, CancellationToken cancellationToken)
    where T : NamedApiResource
    {
        T resource = await GetResourcesWithParamsAsync<T>(name, cancellationToken);
        return resource;
    }

    public async UniTask<NamedApiResourceList<T>> GetNamedResourcePageAsync<T>(int limit, int offset, CancellationToken cancellationToken)
            where T : NamedApiResource
    {
        string url = GetApiEndpointString<T>();
        NamedApiResourceList<T> resource = await GetAsync<NamedApiResourceList<T>>(AddPaginationParamsToUrl(url, limit, offset), false, cancellationToken);
        return resource;
    }

    public async UniTask<ApiResourceList<T>> GetApiResourcePageAsync<T>(int limit, int offset, CancellationToken cancellationToken)
    where T : ApiResource
    {
        string url = GetApiEndpointString<T>();
        ApiResourceList<T> resource = await GetAsync<ApiResourceList<T>>(AddPaginationParamsToUrl(url, limit, offset), false, cancellationToken);
        return resource;
    }

    public async UniTask<T> GetResourceByUrlAsync<T>(string url, CancellationToken cancellationToken)
        where T : ResourceBase
    {
        T resource = await GetAsync<T>(url, true, cancellationToken);
        return resource;
    }

    public async UniTask<Texture2D> GetTexture2DAsync(string url, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return default;
        }

        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            await uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                return null;
            }
            return DownloadHandlerTexture.GetContent(uwr);
        }
    }

    private async UniTask<T> GetResourcesWithParamsAsync<T>(string apiParam, CancellationToken cancellationToken)
            where T : ResourceBase
    {
        string apiEndpoint = GetApiEndpointString<T>();
        return await GetAsync<T>($"{apiEndpoint}/{apiParam}/", false, cancellationToken);
    }

    private async UniTask<T?> GetAsync<T>(string url, bool fullPath, CancellationToken cancellationToken) where T : class
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return default;
        }

        string finalUrl = fullPath ? url : baseURL;
        if (!string.IsNullOrEmpty(url) && !fullPath)
        {
            finalUrl = $"{baseURL}/{url}";
        }
        UnityEngine.Debug.Log($"url: {finalUrl}");

        using (UnityWebRequest uwr = UnityWebRequest.Get(finalUrl))
        {
            await uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success)
            {
                return null;
            }
            string data = uwr.downloadHandler.text;
            T serializedObject = JsonConvert.DeserializeObject<T>(data);
            return serializedObject;
        }
    }

    private static string AddPaginationParamsToUrl(string uri, int? limit = null, int? offset = null)
    {
        if (string.IsNullOrEmpty(uri))
        {
            return string.Empty;
        }
        if (limit.HasValue)
        {
            uri += $"/?limit={limit}";
            if (offset.HasValue)
            {
                uri += $"&offset={offset}";
            }
        }
        else if (offset.HasValue)
        {
            uri += $"?offset={offset}";
        }
        return uri;
    }

    private static string GetApiEndpointString<T>()
    {
        PropertyInfo propertyInfo = typeof(T).GetProperty("ApiEndpoint", BindingFlags.Static | BindingFlags.NonPublic);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(null).ToString();
        }
        return string.Empty;
    }
}
