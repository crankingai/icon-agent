using System;
using System.Diagnostics;
// using Microsoft.SemanticKernel;
using OpenTelemetry; // dotnet add package OpenTelemetry.Exporter.Console
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SemanticKernelDev.Config;

public static class OTelEnabler
{
    public static ResourceBuilder EnableOTelSK(ResourceBuilder rb)
    {
        // Configure tracer just for chat completion calls
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(rb)
            .AddSource("Microsoft.SemanticKernel.Diagnostics")
            // .AddProcessor(new TokenMetricsProcessor())
            .AddConsoleExporter()
            .Build();

        return rb;
    }
}

