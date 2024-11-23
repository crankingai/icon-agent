using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelDev.Plugins;

public class ImageUrlValidatorFunction
{
    static int tryCount = 0;

    [KernelFunction("validate_url")]
    [Description("Validates an image URL. Return true if and only if URL resolves (exists) to a valid PNG or JPEG file. If you encounter errors, retry up to 20 times.")]
    public async Task<bool> ValidateUrlAsync(
        Kernel kernel,
        string image_url
    )
    {
        Console.Write($"*********** [try #{++tryCount}] *> Validating URL: {image_url} *> ");

        // Retrieve URL data to local storage as a temp file
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(image_url);
        // if not successful (including 404), return false
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ ***** FAIL 0! - http status code: {response.StatusCode}");
            return false;
        }
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        await File.WriteAllBytesAsync(tempFile, await response.Content.ReadAsByteArrayAsync());

        // Check the file type
        var fileType = Path.GetExtension(image_url);
        Console.WriteLine($"***** File type: {fileType}");
        // if not a RASTER image, return false
        if (fileType != ".png" && fileType != ".jpg" && fileType != ".jpeg")
        {
            Console.WriteLine("❌ ***** FAIL 1!");
            return false;
        }

        // if has .png extension, verify initial contents match PNG format
        if (fileType == ".png")
        {
            var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
            var fileHeader = new byte[8];
            using (var fileStream = File.OpenRead(tempFile))
            {
                var nbytes = await fileStream.ReadAsync(fileHeader, 0, 8);
                if (nbytes != 8)
                {
                    Console.WriteLine("❌ ***** FAIL 2!");
                    return false;
                }
            }

            if (!fileHeader.SequenceEqual(pngHeader))
            {
                Console.WriteLine("❌ ***** FAIL 3!");
                return false;
            }
        }

        Console.WriteLine("✅ ***** <<success>> :-)");
        return true;
    }
}
