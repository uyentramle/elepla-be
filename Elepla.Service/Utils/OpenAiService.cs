using Elepla.Service.Common;
using Elepla.Service.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Utils
{
    public class OpenAiService : IOpenAiService
    {
        private readonly AppConfiguration _openAISettings;

        public OpenAiService(AppConfiguration openAISettings)
        {
            _openAISettings = openAISettings;
        }

        public async Task<string> GeneratePlanbookField(string prompt)
        {
            var client = new HttpClient();
            // Set the authorization header with your API key from AppConfiguration
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAISettings.OpenAI.ApiKey}");

            // Create the request payload according to OpenAI's chat completion format
            var requestBody = new
            {
                model = "gpt-4o-mini", // Make sure you're using the right model (e.g., gpt-4, gpt-3.5-turbo)
                messages = new[]
                {
                    new { role = "system", content = "You are an assistant that helps with generating educational content." },
                    new { role = "user", content = prompt }  // The user input prompt
                },
                max_tokens = 500,
                temperature = 0.7
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                // Handle error responses from OpenAI
                var errorResult = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error from OpenAI API: {errorResult}");
            }

            var result = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(result);

            // Extract the generated response text from the API response
            return jsonResponse.choices[0].message.content.ToString();
        }
    }
}
