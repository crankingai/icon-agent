using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Plugins.Web.Bing;
using System.Linq;


#pragma warning disable SKEXP0050 // BingTextSearch is not yet available in the public API
// Create an ITextSearch instance using Bing search

namespace SemanticKernelDev.Plugins;

public class ImageWebSearchFunction
{
    static int tryCount = 0;


    [KernelFunction("search_for_image_for_brand")]
    [Description("Return list of candidates for image search for a brand.")]
    public async Task<List<string>> SearchImageAsync(
        Kernel kernel,
        string search_string)
    {
        Console.Write($"¿¿¿¿¿¿¿¿¿¿¿ [try #{++tryCount}] *> Bing Web Searching: {search_string} *> ");

        string bingSearchEndpoint, bingSearchKey;
        (bingSearchEndpoint, bingSearchKey) = Config.EnvVarReader.GetBingSearchConfig();

        var textSearch = new BingTextSearch(apiKey: bingSearchKey);
        var query = "What is the Semantic Kernel?";
        if (!string.IsNullOrWhiteSpace(search_string))
        {
            // flip a coin
            if (new Random().Next(2) == 0)
                query = $"Link to {search_string} png or jpg image";
            else
                query = $"href to {search_string} file";
        }

#pragma warning disable SKEXP0001 // 'KernelSearchResults<string>' is for evaluation purposes only and is subject to change or removal in future updates.
        KernelSearchResults<string> searchResults = await textSearch.SearchAsync(query, new() { Top = 4 });
#pragma warning restore SKEXP0001

        var resultsList = new List<string>();
        await foreach (string result in searchResults.Results)
        {
            Console.WriteLine($"¿¿¿¿¿ - one of the bing search results: {result}");
            resultsList.Add(result);
        }

        return resultsList;
    }
}
