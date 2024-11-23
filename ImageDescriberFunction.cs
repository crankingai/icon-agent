#pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0050, SKEXP0052
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelDev.Plugins;

public class ImageDescriberFunction
{
   static int describeTryCount = 0;

   [KernelFunction("describe_image_at_url")]
   [Description("Return a concise text description of the image.")]
   public async Task<string> DescribeImageAtUrlAsync(
       Kernel kernel,
       string image_url
   )
   {
      Console.Write($"########### [try #{++describeTryCount}] *> Describing URL: {image_url} *> ");

      if (string.IsNullOrWhiteSpace(image_url))
      {
         Console.WriteLine("❌ ##### FAIL no image_url passed in!");
         throw new InvalidOperationException("❌ ##### Failed to retrieve image from URL: URL is empty.");
      }

      // var builder = Kernel.CreateBuilder();

      var chat = kernel.GetRequiredService<IChatCompletionService>();

      var history = new ChatHistory();

      history.AddSystemMessage("You concisely and coherently describe images when asked.");

      var message = new ChatMessageContentItemCollection
      {
         new TextContent("Describe what is depicted in this image:"),
         new ImageContent(new Uri(image_url))
      };

      history.AddUserMessage(message);

      var responseText = await chat.GetChatMessageContentAsync(history);
      Console.WriteLine($"✅ ##### image be like: {responseText}");

      return responseText.ToString();
   }
}
