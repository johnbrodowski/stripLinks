using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

class Program
{
    public static async Task Main(string[] args)
    {
        string linksFilePath = @"D:\links.txt";
        string pdfLinksFilePath = @"D:\Pdflinks.txt";

        // Ensure Chromium is downloaded
        await DownloadChromium();

        // Read all URLs from the file
        var urls = File.ReadAllLines(linksFilePath);

        // Prepare the file to store PDF links
        if (File.Exists(pdfLinksFilePath))
        {
            File.Delete(pdfLinksFilePath);
        }

        foreach (var url in urls)
        {
            try
            {
                var pdfLinks = await GetPdfLinks(url);
                // Append found PDF links to the file
                await File.AppendAllLinesAsync(pdfLinksFilePath, pdfLinks);
                Console.WriteLine($"Processed {url} successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing {url}: {ex.Message}");
                // Optionally, log the error or take other actions
                // Continue with the next URL
            }
        }

        Console.WriteLine("PDF links extraction completed.");
    }

    private static async Task DownloadChromium()
    {
        Console.WriteLine("Starting browser download and setup...");
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync(); // Download the latest version of Chromium
    }

    private static async Task<List<string>> GetPdfLinks(string url)
    {
        var pdfLinks = new List<string>();

        await using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
        {
            await using (var page = await browser.NewPageAsync())
            {
                await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);

                var links = await page.EvaluateFunctionAsync<string[]>(@"
                    () => {
                        const anchors = Array.from(document.querySelectorAll('a[href$=""\.pdf""]'));
                        return anchors.map(anchor => anchor.href);
                    }
                ");

                pdfLinks.AddRange(links.Where(link => !string.IsNullOrEmpty(link)));
            }
        }

        return pdfLinks;
    }
}



 


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using PuppeteerSharp;

//class Program
//{
//    public static async Task Main(string[] args)
//    {
//        await DownloadChromium();
//        string url = "https://catalog.archives.gov/id/296836434"; // Replace with the URL you're interested in
//        var pdfLinks = await GetPdfLinks(url);

//        foreach (var link in pdfLinks)
//        {
//            Console.WriteLine(link);
//        }
//    }

//    private static async Task DownloadChromium()
//    {
//         Console.WriteLine("Starting browser download and setup...");
//        var browserFetcher = new BrowserFetcher();
//        await browserFetcher.DownloadAsync(); // Download the latest version of Chromium
//    }



//    private static async Task<List<string>> GetPdfLinks(string url)
//    {
//        var pdfLinks = new List<string>();

//        // Launch the browser
//        await using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
//        {
//            // Create a new page
//            await using (var page = await browser.NewPageAsync())
//            {
//                // Navigate to the URL
//                await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);

//                // Use PuppeteerSharp's API to evaluate JavaScript in the context of the page to find PDF links
//                // Correctly format the JavaScript function as a verbatim string literal
//                var links = await page.EvaluateFunctionAsync<string[]>(@"
//                () => {
//                    const anchors = Array.from(document.querySelectorAll('a[href$=""\.pdf""]'));
//                    return anchors.map(anchor => anchor.href);
//                }
//            ");

//                pdfLinks.AddRange(links.Where(link => !string.IsNullOrEmpty(link)));
//            }
//        }

//        return pdfLinks;
//    }

//}



//using System;
//using System.IO;
//using System.Linq;
//using System.Xml;
//using HtmlAgilityPack;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Path to your HTML file
//        string htmlFilePath = @"D:\html.txt";
//        // Path where you want to save the links.txt file
//        string linksFilePath = @"D:\links.txt";

//        // Check if the HTML file exists
//        if (!File.Exists(htmlFilePath))
//        {
//            Console.WriteLine("The HTML file does not exist.");
//            return;
//        }

//        try
//        {
//            // Load the HTML document
//            var htmlDoc = new HtmlDocument();
//            htmlDoc.Load(htmlFilePath);

//            // Select all the <a> tags
//            var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

//            // Check if there are any <a> tags found
//            if (linkNodes != null)
//            {
//                // Extract the href attributes (URLs)
//                var links = linkNodes.Select(node => node.Attributes["href"].Value);

//                // Write all the links to the file
//                File.WriteAllLines(linksFilePath, links);

//                Console.WriteLine($"Links extracted and saved to {linksFilePath}");
//            }
//            else
//            {
//                Console.WriteLine("No links found in the HTML file.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"An error occurred: {ex.Message}");
//        }
//    }
//}
