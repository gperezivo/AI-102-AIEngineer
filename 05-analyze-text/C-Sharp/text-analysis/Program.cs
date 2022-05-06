using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Azure.AI.TextAnalytics;
using Azure;

// Import namespaces


namespace text_analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
                string cogSvcKey = configuration["CognitiveServiceKey"];

                // // Set console encoding to unicode
                // Console.InputEncoding = Encoding.Unicode;
                // Console.OutputEncoding = Encoding.Unicode;

                // Create client using endpoint and key
                var client = new TextAnalyticsClient(new Uri(cogSvcEndpoint), new AzureKeyCredential(cogSvcKey));

                // Analyze each text file in the reviews folder
                var folderPath = Path.GetFullPath("./reviews");  
                DirectoryInfo folder = new DirectoryInfo(folderPath);
                foreach (var file in folder.GetFiles("*.txt"))
                {
                    // Read the file contents
                    Console.WriteLine("\n-------------\n" + file.Name);
                    StreamReader sr = file.OpenText();
                    var text = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine("\n" + text);

                    // Get language
                    var resultLanguage = client.DetectLanguage(text);
                    Console.WriteLine($"El fichero {file} esta escrito en: {resultLanguage.Value.Name}{resultLanguage.Value.ConfidenceScore}");

                    // Get sentiment
                    var resultSentiment = client.AnalyzeSentiment(text);
                    Console.WriteLine($"Sentimiento: {resultSentiment.Value.Sentiment}");
                    Console.WriteLine($"Positivo: {resultSentiment.Value.ConfidenceScores.Positive}");
                    Console.WriteLine($"Negativo: {resultSentiment.Value.ConfidenceScores.Negative}");
                    Console.WriteLine($"Neutral: {resultSentiment.Value.ConfidenceScores.Neutral}");
                    


                    // Get key phrases


                    // Get entities
                    var resultEntities = client.RecognizeEntities(text);
                    Console.WriteLine($"Entidades: {resultEntities.Value.Count}");
                    foreach (var entity in resultEntities.Value)
                    {
                        Console.WriteLine($"\t{entity.Text} - {entity.Category}");
                    }
                    Console.ReadLine();

                    // Get linked entities


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



    }
}
