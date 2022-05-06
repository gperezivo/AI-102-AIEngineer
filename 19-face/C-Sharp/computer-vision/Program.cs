using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

// Import namespaces


namespace detect_faces
{
    class Program
    {

        private static ComputerVisionClient cvClient;
        static async Task Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
                string cogSvcKey = configuration["CognitiveServiceKey"];

                // Authenticate Computer Vision client
                var cred = new ApiKeyServiceClientCredentials(cogSvcKey);
                cvClient = new ComputerVisionClient(cred)
                {
                    Endpoint = cogSvcEndpoint
                };


                // Detect faces in an image
                string imageFile = "images/me.png";
                await AnalyzeFaces(imageFile);
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task AnalyzeFaces(string imageFile)
        {
            Console.WriteLine($"Analyzing {imageFile}");

            using var fileStream = File.OpenRead(imageFile);
            // Specify features to be retrieved (faces)
            var analyze = await cvClient.AnalyzeImageInStreamAsync(fileStream,
                new List<VisualFeatureTypes?>() {
                    VisualFeatureTypes.Faces
                });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(analyze));
            Console.WriteLine(analyze.Faces.Select(f => f.Age).FirstOrDefault());


            // Get image analysis

                
        }


    }
}
