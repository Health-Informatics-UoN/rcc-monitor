using System.Net.Http.Headers;
using Functions.Models;
using Functions.Services.Contracts;
using Newtonsoft.Json;

namespace Functions.Services;

public class RedCapSitesService : IDataService
{
    
    private readonly HttpClient _client;
    private const string SitesUrl = "/rest/v2/sites";

    public RedCapSitesService(
        IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient();
    }
    
    public async Task<List<SiteView>> List(string url, string token)
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("token", token);
        
        var response = await _client.GetAsync(url + SitesUrl);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<SiteView>>(responseString) ?? new List<SiteView>();
    }
    
    public async Task<Site> Get(string url, string id, string token)
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Add("token", token);
        
        var response = await _client.GetAsync(url + SitesUrl + "/" + id);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Site>(responseString) ?? new Site();
    }
    
    public async Task<List<Site>> ListDetail(string url, string token)
    {
        var sites = await List(url, token);

        var siteDetails = new List<Site>();
        foreach (var site in sites)
        {
            var detail = await Get(url, site.Id.ToString(), token);
            siteDetails.Add(detail);
        }

        return siteDetails;
    }
}