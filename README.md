# The 'Icon Agent'

An AI Agent to promote responsible use of tech icons

## Semantic Kernel Resources

1. https://learn.microsoft.com/en-us/semantic-kernel/get-started/quick-start-guide
2. [Web Search Plugins](https://learn.microsoft.com/en-us/semantic-kernel/concepts/text-search/) for RAG, for example

## Semantic Kernel Setup (before coding)

```bash
dotnet new console

dotnet add package Microsoft.SemanticKernel
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package OpenTelemetry.Exporter.Console

# using System.Configuration; // for ConfigurationErrorsException: 
dotnet add package System.Configuration.ConfigurationManager

# for *.prompty file support (no usings needed)
dotnet add package Microsoft.SemanticKernel.Prompty --prerelease

# RAG/web search
dotnet add package Microsoft.SemanticKernel.Plugins.Web --version 1.30.0-alpha

# see what you hath wrought
dotnet list package
```
