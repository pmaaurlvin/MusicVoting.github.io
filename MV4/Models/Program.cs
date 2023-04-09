using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace SpotifyAPIExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Your Spotify API client ID and client secret
            string clientId = "0de4927c153046faa9c9b71dd370ea67";
            string clientSecret = "5a2c9e210bf04e0fbe9e29ae90e463db";

            // Spotify API endpoint for obtaining an access token
            string tokenEndpoint = "https://accounts.spotify.com/api/token";

            // Create a new HttpClient to send the POST request to the Spotify API
            using (var client = new HttpClient())
            {
                // Set the request parameters
                var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
                request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"))}");
                request.Content = new StringContent("grant_type=client_credentials", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

                // Send the POST request and retrieve the response
                var response = await client.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response to retrieve the access token
                var json = JsonDocument.Parse(responseBody);
                string accessToken = json.RootElement.GetProperty("access_token").GetString();

                Console.WriteLine($"Access token: {accessToken}");
            }
        }
    }
}
