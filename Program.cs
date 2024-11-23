// Import packages
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SemanticKernelDev.Config;

using SemanticKernelDev.Plugins;

#region Load configuration (including secrets) from environment variables
// Load the Azure OpenAI keys and config from the environment variables
string azureOpenAIModelId, azureOpenAIEndpoint, azureOpenAIAPIKey;
(azureOpenAIModelId, azureOpenAIEndpoint, azureOpenAIAPIKey) = EnvVarReader.GetAzureOpenAIConfig();

// Load the OpenAI API keys and config from the environment variables
string openAIModelId, openAIAPIKey;
(openAIModelId, openAIAPIKey) = EnvVarReader.GetOpenAIConfig();
#endregion

#region Commandline arguments
// At least one commandline parameter is expected
if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run <brand>");
    return 1;
}

string brand = string.Join(" ", args);
Console.WriteLine($"Hello, Icon Agent here! Beginning processing for: ❰{brand}❱");
#endregion

var builder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(azureOpenAIModelId, azureOpenAIEndpoint, azureOpenAIAPIKey, serviceId: "AzureOpenAI")
    .AddOpenAIChatCompletion(openAIModelId, openAIAPIKey, serviceId: "OpenAI")
    ;

var resourceBuilder = ResourceBuilder.CreateDefault().AddService("ResponsibleIconUse");

#region OpenTelemetry
resourceBuilder = OTelEnabler.EnableOTelSK(resourceBuilder);

AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddConsoleExporter();

        // Format log messages. This is default to false.
        // options.IncludeFormattedMessage = true;
        // options.IncludeScopes = true;
        // options.IncludeFormattedMessage = false;
        // options.IncludeScopes = false;
    });
    // builder.SetMinimumLevel(LogLevel.Information);
    // builder.SetMinimumLevel(LogLevel.Trace);
    builder.SetMinimumLevel(LogLevel.Warning);
    // builder.SetMinimumLevel(LogLevel.None);
});
#endregion

Kernel kernel = builder.Build();
builder.Services.AddSingleton(loggerFactory);

var logger = loggerFactory.CreateLogger<Program>();

//builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

logger.LogInformation($"✅ Starting the Icon Agent for ❰{brand}❱");

var chat = kernel.GetRequiredService<IChatCompletionService>();
// Which is chat completion service type (OpenAI or AzureOpenAI)?
logger.LogWarning($"Chat completion service: {chat.GetType().Name}");


var history = new ChatHistory();

#region Basic AI Chat app
#if true

PromptExecutionSettings promptExecutionSettings = new()
{
};

history.AddUserMessage($"Hello, Icon Agent here! Let's talk about an interesting technical detail for: \"{brand}\"");

var result = await chat.GetChatMessageContentAsync(
        history,
        executionSettings: promptExecutionSettings,
        kernel: kernel);

logger.LogInformation($"✅ Icon Agent says ❰❰begin❱❱\n{result}\n❰❰end❱❱");

history.Clear();

#endif
#endregion



#region Basic AI Agent
#if true

#region Functions
#region Plugins
// Add plugins
#pragma warning disable SKEXP0040
var func1 = kernel.CreateFunctionFromPromptyFile("./IconFinder.prompty");
#pragma warning restore SKEXP0040
var func2 = kernel.Plugins.AddFromType<ImageUrlValidatorFunction>("url_validator"); // ***
var func3 = kernel.Plugins.AddFromType<ImageDescriberFunction>("describe_image_at_url"); // ###
// var func4 = kernel.Plugins.AddFromType<ImageWebSearchFunction>("search_for_image_for_brand"); // ###
#endregion
#endregion

#region AI Agent Prompt Execution Settings
// PromptExecutionSettings → https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.promptexecutionsettings?view=semantic-kernel-dotnet
//    ↑
// OpenAIPromptExecutionSettings → https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.connectors.openai.openaipromptexecutionsettings?view=semantic-kernel-dotnet
//    ↑
// AzureOpenAIPromptExecutionSettings → https://learn.microsoft.com/en-us/dotnet/api/microsoft.semantickernel.connectors.azureopenai.azureopenaipromptexecutionsettings?view=semantic-kernel-dotnet

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
#pragma warning disable SKEXP0001
    // ★☆★ 💖💖💖 ☆★☆ 💖💖💖 ★☆★
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
#pragma warning restore SKEXP0001

    MaxTokens = 1500,
    // Temperature = 0.70, // higher values make the model more creative
    Temperature = 0.10, // lower values make the model more precise
};
#endregion

// represents the GOAL
var promptTemplate = $"Get a valid URL for the logo of {brand}, " +
        "ideally it is the official logo as published " +
        "by the organization that owns the brand, but other sources can work. " +
        "Once sure URL is valid, return a reference to it in an HTML 'img' tag " +
        "using a concise text description of the logo image " +
        "for the 'title' attribute and " +
        "referencing the brand name in the 'alt' attribute " +
        "saying something like 'XYZ logo'. " +
        "If you encounter errors along the way, retry up to 10 times.";

// var response = ((await kernel.InvokeAsync(recall_facts, new() { ["input"] = contextFreeQuery })).ToString().Trim());

//var res = await kernel.InvokeAsync(promptTemplate, new() { ["brand"] = brand });

// history.AddMessage("system",  "Hello, Icon Agent here! Let's talk about an interesting technical detail for: \"{brand}\"");
// history.AddUserMessage(promptTemplate);
// history.AddUserMessage(new() { ["brand"] = brand });

// var response = await chat.GetChatMessageContentAsync(
//         history,
//         executionSettings: openAIPromptExecutionSettings,
//         kernel: kernel);



// var response = await kernel.InvokePromptAsync(
//     promptTemplate,
//     new() { ["brand"] = brand },
//     executionSettings: openAIPromptExecutionSettings
// );

var response = await kernel.InvokePromptAsync(promptTemplate, new(openAIPromptExecutionSettings));


// var response = await kernel.InvokePromptAsync(promptTemplate, new() { ["brand"] = brand }, new(openAIPromptExecutionSettings));

Console.WriteLine(response); // Outputs: Good morning, John Doe!


// var response = await kernel.InvokePromptAsync(promptTemplate,
// Parameters = new Dictionary<string, string> { { "brand", brand } } }
// new(openAIPromptExecutionSettings));

Console.WriteLine("---------------------------------------------------------------");
Console.WriteLine(response);

#endif
#endregion



























#if false

// Add enterprise components
// builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Build the kernel
Kernel kernel = builder.Build();

// Add a plugin (the LightsPlugin class is defined below)
kernel.Plugins.AddFromType<LightsPlugin>("Lights");

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create a history store the conversation
var history = new ChatHistory();

// Initiate a back-and-forth chat
string? userInput;
do {
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    // Add user input
    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    Console.WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not null);
#endif

return 0;
