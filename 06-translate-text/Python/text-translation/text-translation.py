import uuid
from dotenv import load_dotenv
from azure.core.credentials import AzureKeyCredential
from azure.ai.textanalytics import TextAnalyticsClient
import os
import requests, json

def main():
    global translator_endpoint
    global cog_key
    global cog_region

    try:
        # Get Configuration Settings
        load_dotenv()
        cog_key = os.getenv('COG_SERVICE_KEY')
        cog_region = os.getenv('COG_SERVICE_REGION')
        translator_endpoint = 'https://api.cognitive.microsofttranslator.com'

        constructed_url = translator_endpoint + "/transliterate?api-version=3.0&language=ja&fromScript=Jpan&toScript=Latn"

        headers = {
            'Ocp-Apim-Subscription-Key': cog_key,
            'Ocp-Apim-Subscription-Region': cog_region,
            'Content-type': 'application/json',
            'X-ClientTraceId': str(uuid.uuid4())
        }
        body = [{'text': "こんにちは"}]
        request = requests.post(constructed_url, headers=headers, json=body)
        print(request.json())

        GetLanguage("спасибо")
        # Analyze each text file in the reviews folder
        # reviews_folder = 'reviews'
        # for file_name in os.listdir(reviews_folder):
        #     # Read the file contents
        #     print('\n-------------\n' + file_name)
        #     text = open(os.path.join(reviews_folder, file_name), encoding='utf8').read()
        #     print('\n' + text)

        #     # Detect the language
        #     language = GetLanguage(text)
        #     print('Language:',language)

        #     # Translate if not already English
        #     if language != 'en':
        #         translation = Translate(text, language)
        #         print("\nTranslation:\n{}".format(translation))
                
    except Exception as ex:
        print(ex)

def GetLanguage(text):
    # Default language is English
    language = 'en'

    # Use the Translator detect function
    # If you encounter any issues with the base_url or path, make sure
    # that you are using the latest endpoint: https://docs.microsoft.com/azure/cognitive-services/translator/reference/v3-0-detect
    path = '/detect?api-version=3.0'
    constructed_url = translator_endpoint + path

    headers = {
        'Ocp-Apim-Subscription-Key': cog_key,
        'Ocp-Apim-Subscription-Region': cog_region,
        'Content-type': 'application/json',
        'X-ClientTraceId': str(uuid.uuid4())
    }
    body = [{'text': text}]
    request = requests.post(constructed_url, headers=headers, json=body)
    print(request.json())
    ## extract language and score from every object in request.json()
    for obj in request.json():
        language = obj['language']
        score = obj['score']
        print('Language:',language,'score:',score)

    # Return the language
    return language

def Translate(text, source_language):
    translation = ''

    # Use the Translator translate function
    constructed_url = translator_endpoint + "/translate?api-version=3.0&from{}&to=en".format(source_language)

    headers = {
        'Ocp-Apim-Subscription-Key': cog_key,
        'Ocp-Apim-Subscription-Region': cog_region,
        'Content-type': 'application/json',
        'X-ClientTraceId': str(uuid.uuid4())
    }
    body = [{'text': text}]
    request = requests.post(constructed_url, headers=headers, json=body)
    translation = request.json()[0]['translations'][0]['text']
    # Return the translation
    return translation

if __name__ == "__main__":
    main()