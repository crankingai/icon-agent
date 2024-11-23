using System;
using System.Configuration; // for ConfigurationErrorsException: dotnet add package System.Configuration.ConfigurationManager

namespace SemanticKernelDev.Config;

public static class EnvVarReader
{
    public static bool debug = false;


    // intended use:
    // if (EnvVarReader.GetVerbose()) Console.WriteLine("blah blah");
    public static bool GetVerbose()
    {
        var verbose = Environment.GetEnvironmentVariable("VERBOSE");
        if (verbose is not null &&
            verbose.ToLower() != "false" &&
            verbose.ToLower() != "off" &&
            verbose.ToLower() != "no" &&
            verbose.ToLower() != "0")
        {
            return true; // let's be verbose
        }

        return false; // let's be quiet
    }

    public static (string, string, string) GetAzureOpenAIConfig()
    {
        var modelId = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

        if (modelId is null || endpoint is null || apiKey is null)
        {
            throw new ConfigurationErrorsException("Please set the environment variables for the Azure OpenAI deployment name, endpoint, and API key.");
        }

        if (debug)
        {
            Console.WriteLine($"Model ID: {modelId}");
            Console.WriteLine($"Endpoint: {endpoint}");
            Console.WriteLine($"API Key: {apiKey.Substring(0, 2)}...{apiKey.Substring(apiKey.Length - 2)}");
        }

        return (modelId, endpoint, apiKey);
    }

    public static (string, string) GetOpenAIConfig()
    {
        var modelIdEnvVar = "OPENAI_MODEL_ID";
        var apiKeyEnvVar = "OPENAI_API_KEY";
        var modelId = Environment.GetEnvironmentVariable(modelIdEnvVar);
        var apiKey = Environment.GetEnvironmentVariable(apiKeyEnvVar);

        if (modelId is null || apiKey is null)
        {
            throw new System.Configuration.ConfigurationErrorsException($"Did not find one or both of {modelIdEnvVar} and {apiKeyEnvVar} environment variable for OpenAI API access.");
        }

        if (debug)
        {
            Console.WriteLine($"Model ID: {modelId}");
            Console.WriteLine($"API Key: {apiKey.Substring(0, 12)}...{apiKey.Substring(apiKey.Length - 2)}");
        }

        return (modelId, apiKey);
    }


    /// <summary>
    /// Get the Bing Search API key and endpoint from the environment variables.
    /// </summary>
    /// <returns>(BING_SEARCH_ENDPOINT, BING_SEARCH_KEY)</returns>
    /// <exception cref="System.Configuration.ConfigurationErrorsException"></exception>
    public static (string, string) GetBingSearchConfig()
    {
        var bingSearchEndpointEnvVar = "BING_SEARCH_ENDPOINT";
        var bingSearchKeyEnvVar = "BING_SEARCH_KEY";
        var bingSearchEndpoint = Environment.GetEnvironmentVariable(bingSearchEndpointEnvVar);
        var bingSearchKey = Environment.GetEnvironmentVariable(bingSearchKeyEnvVar);

        if (bingSearchKey is null || bingSearchEndpoint is null)
        {
            throw new System.Configuration.ConfigurationErrorsException($"Did not find one or both of {bingSearchKeyEnvVar} and {bingSearchEndpointEnvVar} environment variable for Bing Web Search API access.");
        }

        if (debug)
        {
            Console.WriteLine($"Bing Search Endpoint: {bingSearchEndpoint}");
            Console.WriteLine($"Bing Search API Key: {bingSearchKey.Substring(0, 5)}...{bingSearchKey.Substring(bingSearchKey.Length - 2)}");
        }

        return (bingSearchEndpoint, bingSearchKey);
    }
}
