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


namespace image_analysis
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

                // Get image
                string imageFile = "images/building.jpg";
                if (args.Length > 0)
                {
                    imageFile = args[0];
                }


                // Authenticate Computer Vision client
                cvClient = new ComputerVisionClient(new ApiKeyServiceClientCredentials(cogSvcKey)){
                    Endpoint = cogSvcEndpoint
                };

                // Analyze image
                await AnalyzeImage(imageFile);

                // Get thumbnail
                await GetThumbnail(imageFile);

               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static async Task AnalyzeImage(string imageFile)
        {
            Console.WriteLine($"Analyzing {imageFile}");
            using var reader = new System.IO.FileStream(imageFile,FileMode.Open);
            // Specify features to be retrieved
            var result = await cvClient.AnalyzeImageInStreamAsync(
                reader, 
                visualFeatures: new List<VisualFeatureTypes?>{ 
                    VisualFeatureTypes.Description,
                    VisualFeatureTypes.Adult    
                }
            );
            
            
            // Get image analysis
            var textResult = string.Join(Environment.NewLine,
                result.Description.Captions.ToList().Select(c=>c.Text)
            );
            Console.WriteLine(textResult);
            //Console.ReadLine();
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
        }

        static async Task GetThumbnail(string imageFile)
        {
            Console.WriteLine("Generating thumbnail");
            using var reader = new System.IO.FileStream(imageFile,FileMode.Open);
            // using var reader2 = new System.IO.FileStream(imageFile,FileMode.Open);
            var thumbnail = await cvClient.GenerateThumbnailInStreamAsync(100,100,reader,smartCropping: true);
            
            
            try{
                

                using var fs = new FileStream(".\\t2.jpg",FileMode.OpenOrCreate);
                thumbnail.CopyTo(fs);
                // using var fs2 = new FileStream(".\\t2.jpg",FileMode.CreateNew);
                // thumbnail2.CopyTo(fs2);
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            // Generate a thumbnail

        }


    }
}
