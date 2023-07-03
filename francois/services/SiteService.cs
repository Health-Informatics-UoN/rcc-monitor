using System.Net.Http.Headers;
using francois.models;
using Newtonsoft.Json;

namespace francois.services;

public class SiteService
{
    private readonly HttpClient _client;
    private const string SitesUrl = "/rest/v2/sites";

    public SiteService(
        IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient();
    }
    
    /// <summary>
    /// List sites at a tenant level.
    /// </summary>
    /// <param name="url">URL of the redCAP instance.</param>
    /// <param name="token">Tenant token to use.</param>
    /// <returns>A list of site view models.</returns>
    private async Task<List<SiteView>> List(string url, string token)
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("token", token);
    
        var response = await _client.GetAsync(url + SitesUrl);
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<SiteView>>(responseString);
        }
        else
        {
            Console.WriteLine("Failed to retrieve sites. Status code: " + response.StatusCode);
            return new List<SiteView>();
        }
    }    
    
    /// <summary>
    /// Get a site detail at a tenant level.
    /// </summary>
    /// <param name="url">URL of the redCAP instance</param>
    /// <param name="id">Id of the site to retrieve.</param>
    /// <param name="token">Tenant token to use.</param>
    /// <returns>The relevant site model.</returns>
    private async Task<Site> Get(string url, string id, string token)
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("token", token);

        var response = await _client.GetAsync(url + SitesUrl + "/" + id);
        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Site>(responseString);
        }
        else
        {
            Console.WriteLine("Failed to retrieve site. Status code: " + response.StatusCode);
            return new Site() { };
        }
    }

    /// <summary>
    /// Get detailed lists of a Sites at a tenant level.
    /// This method fetches the initial list of all sites, with
    /// <see cref="List"/>.
    /// This only returns a list of <see cref="SiteView"/> model which is partial information.
    /// To fetch detailed site info, we <see cref="Get"/> it for each site.
    /// The redCAP API does not make it possible for this not to be 0n. 
    /// </summary>
    /// <param name="url">URL of the redCAP instance</param>
    /// <param name="token">Tenant token to use.</param>
    /// <returns>List of site models.</returns>
    public async Task<List<Site>> ListDetail(string url, string token)
    {
        var sites = await List(url, token);

        var siteDetails = new List<Site>();

        var i = sites.GetRange(0, 100);

        foreach (var site in sites)
        {
            var detail = await Get(url, site.Id.ToString(), token);
            siteDetails?.Add(detail);
        }

        return siteDetails;
    }

}