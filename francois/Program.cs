using francois.services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Problem:
// could you get an export from prod, then write some code that can give us a report of any differences
// like a delta report

var buildKey = "";
var prodKey = "";

// Hit build endpoint
var buildUrl = "http://eubuild.redcapcloud.com";

// Hit prod endpoint
var prodUrl = "http://nuh.eulogin.redcapcloud.com";

var services = new ServiceCollection()
    .AddHttpClient()
    .AddTransient<SiteService>()
    .BuildServiceProvider();

var sites = services.GetRequiredService<SiteService>();

Console.WriteLine("Fetching Data...");

// Comment out which data to fetch :)

// Fetch real data.
// var buildSites = await sites.ListDetail(buildUrl, buildKey); 
// var prodSites = await sites.ListDetail(prodUrl, prodKey); 

// Fetch test data.
var buildSites = new TestDataGenerator().GenerateSitesTestData();
var prodSites = new TestDataGenerator().GenerateSitesTestDataProd();

// Generate reporting lists...
var buildSiteIds = buildSites.Select(s => s.SiteId).ToList();
var prodSiteIds = prodSites.Select(s => s.SiteId).ToList();

var sitesOnlyInBuild = buildSites.Where(s => !prodSiteIds.Contains(s.SiteId)).ToList();

var sitesWithDifferentNames = buildSites
    .Join(prodSites,
        buildSite => buildSite.SiteId,
        prodSite => prodSite.SiteId,
        (buildSite, prodSite) => new { BuildSite = buildSite, ProdSite = prodSite })
    .Where(sites => sites.BuildSite.Name != sites.ProdSite.Name)
    .ToList();

// compare the parent of each site
var sitesWithDifferentParentSiteId = buildSites
    .Where(buildSite => buildSite.ParentSiteId != 0) // Filter out sites without a parent
    .Join(prodSites,
        buildSite => buildSite.SiteId,
        prodSite => prodSite.SiteId,
        (buildSite, prodSite) => new { BuildSite = buildSite, ProdSite = prodSite })
    .Where(sites =>
    {
        // Get the parent site for the current site
        var buildParentSite = buildSites.FirstOrDefault(site => site.Id == sites.BuildSite.ParentSiteId);
        
        // Get its prod alternate
        var prodSite = prodSites.FirstOrDefault(site => site.SiteId == sites.BuildSite.SiteId);
        
        // Get prods parent and check if they match
        var prodParentSite = prodSites.FirstOrDefault(site => prodSite != null && site.Id == prodSite.ParentSiteId);
        
        // TODO: this filters out if build or prod dont have parent sites.
        return prodParentSite != null && buildParentSite != null && prodParentSite.SiteId != buildParentSite.SiteId;
    })
    .ToList();

Console.WriteLine("Sites missing in prod:");
foreach (var site in sitesOnlyInBuild)
{
    Console.WriteLine($"Id: {site.Id}, SiteId: {site.SiteId}, Name: {site.Name}");
}

Console.WriteLine();
Console.WriteLine("Sites with different names:");
foreach (var site in sitesWithDifferentNames)
{
    Console.WriteLine($"SiteId: {site.BuildSite.SiteId}, BuildSite: {site.BuildSite.Name}, ProdSite: {site.ProdSite.Name}");
}

Console.WriteLine();
Console.WriteLine("Sites with different parentSiteID:");
foreach (var sitesDiff in sitesWithDifferentParentSiteId)
{
    Console.WriteLine($"BuildSite Name: {sitesDiff.BuildSite.Name}, ProdSite Name: {sitesDiff.ProdSite.Name}");
    Console.WriteLine($"BuildSite: SiteId: {sitesDiff.BuildSite.SiteId}, ParentSiteId: {sitesDiff.BuildSite.ParentSiteId}");
    Console.WriteLine($"ProdSite: SiteId: {sitesDiff.ProdSite.SiteId}, ParentSiteId: {sitesDiff.ProdSite.ParentSiteId}");

    var buildParentSite = buildSites.FirstOrDefault(site => site.Id == sitesDiff.BuildSite.ParentSiteId);
    var prodMatchedSite = prodSites.FirstOrDefault(site => site.SiteId == sitesDiff.BuildSite.SiteId);
    var prodParentSite = prodSites.FirstOrDefault(site => prodMatchedSite != null && site.Id == prodMatchedSite.ParentSiteId);

    Console.WriteLine($"BuildParentSite: SiteId: {buildParentSite.SiteId}");
    Console.WriteLine($"ProdParentSite: SiteId: {prodParentSite.SiteId}");
    Console.WriteLine();
}

Console.WriteLine("Complete.");
