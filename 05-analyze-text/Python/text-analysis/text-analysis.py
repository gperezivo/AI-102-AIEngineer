from azure.core.credentials import AzureKeyCredential
from azure.ai.textanalytics import TextAnalyticsClient
from dotenv import load_dotenv
import os

# Import namespaces


def main():
    try:
        # Get Configuration Settings
        load_dotenv()
        cog_endpoint = os.getenv('COG_SERVICE_ENDPOINT')
        cog_key = os.getenv('COG_SERVICE_KEY')

        # Create client using endpoint and key

        credential = AzureKeyCredential(cog_key)
        client = TextAnalyticsClient(endpoint=cog_endpoint, credential=credential)
        # Analyze each text file in the reviews folder
        reviews_folder = 'reviews'
        for file_name in os.listdir(reviews_folder):
            # Read the file contents
            print('\n-------------\n' + file_name)
            text = open(os.path.join(reviews_folder, file_name), encoding='utf8').read()
            print('\n' + text)

            # Get language
            documents = [text]
            result = client.detect_language(documents)
            
            successful_responses = [doc for doc in result if not doc.is_error]
            
            print(successful_responses)


            # Get sentiment
            lang = successful_responses[0].primary_language
            result = client.analyze_sentiment(documents)
            print(result)

            # Get key phrases


            # Get entities


            # Get linked entities



    except Exception as ex:
        print(ex)


if __name__ == "__main__":
    main()