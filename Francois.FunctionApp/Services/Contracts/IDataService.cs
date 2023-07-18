using System.Collections.Generic;
using System.Threading.Tasks;
using Francois.FunctionApp.Models;

namespace Francois.FunctionApp.Services.Contracts;

public interface IDataService
{
    /// <summary>
    /// List sites at a tenant level.
    /// </summary>
    /// <param name="url">URL of the redCAP instance.</param>
    /// <param name="token">Tenant token to use.</param>
    /// <returns>A list of site view models.</returns>
    Task<List<SiteView>> List(string url, string token);
    
    /// <summary>
    /// Get a site detail at a tenant level.
    /// </summary>
    /// <param name="url">URL of the redCAP instance</param>
    /// <param name="id">Id of the site to retrieve.</param>
    /// <param name="token">Tenant token to use.</param>
    /// <returns>The relevant site model.</returns>
    Task<Site> Get(string url, string id, string token);
    
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
    Task<List<Site>> ListDetail(string url, string token);
}