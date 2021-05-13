﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TranslationsBuilder.Services {

  class TranslatorService {

    private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";
    private static readonly string location = AppSettings.AZURE_TRANSLATOR_SERVICE_LOCATION;
    private static readonly string subscriptionKey = AppSettings.AZURE_TRANSLATOR_SERVICE_KEY;

    private class TranslatedText {
      public string text { get; set; }
      public string to { get; set; }
    }

    private class TranslatedTextResult {
      public List<TranslatedText> translations { get; set; }
    }

    static private List<TranslatedText> GetTranslations(string textToTranslate, string[] languages) {

      string targetLanguages = "";
      foreach (string language in languages) {
        targetLanguages += "&to=" + language;
      }

      string route = "/translate?api-version=3.0&from=en" + targetLanguages;
      object[] body = new object[] { new { Text = textToTranslate } };
      var requestBody = JsonConvert.SerializeObject(body);

      using (var client = new HttpClient())
      using (var request = new HttpRequestMessage()) {
        // Build the request.
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(endpoint + route);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.Headers.Add("Ocp-Apim-Subscription-Region", location);

        // Send the request and get response.
        HttpResponseMessage response = client.Send(request);
        // Read response as a string.
        string result = response.Content.ReadAsStringAsync().Result;

        List<TranslatedTextResult> convertedResult = JsonConvert.DeserializeObject<List<TranslatedTextResult>>(result);

        return convertedResult[0].translations;

      }

    }

    public static string TranslateContent(string textToTranslate, string language) {
      string[] languages = { language };
      var translationsResult = GetTranslations(textToTranslate, languages);
      return translationsResult[0].text;
    }

  }
}