using System.Net.Http.Json;
using CatAPIfetcher.Model;
using System.Diagnostics;

namespace CatAPIfetcher.Services
{
    public class CatAPIservice
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.thecatapi.com/v1";
        private const string ApiKey = "live_68J2qQxlmPOtKt2Hn1OFJ6e91buk1FW75MXSN1HjaWZJ2OITU6ROTW5bDnV9LUoE";

        public CatAPIservice(HttpClient httpClient)
        {
            _httpClient = httpClient;

            if (!string.IsNullOrEmpty(ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("x-api-key", ApiKey);
            }
        }

        public async Task<List<Cat>> GetRandomCatsAsync(int limit = 20)
        {
            try
            {
                var url = $"{BaseUrl}/images/search?limit={limit}&has_breeds=1";
                Debug.WriteLine($"Fetching cats from: {url}");

                var response = await _httpClient.GetAsync(url);
                Debug.WriteLine($"Response status: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var cats = await response.Content.ReadFromJsonAsync<List<Cat>>();
                Debug.WriteLine($"Received {cats?.Count ?? 0} cats from API");

                if (cats != null)
                {
                    foreach (var cat in cats)
                    {
                        cat.CreatedAt = DateTime.Now;
                        cat.IsLocalOnly = false;

                        if (cat.Breeds != null && cat.Breeds.Any())
                        {
                            var breed = cat.Breeds.First();
                            cat.Name = breed.Name;
                            cat.Description = breed.Description;
                            cat.Origin = breed.Origin;
                            cat.Temperament = breed.Temperament;
                            cat.LifeSpan = breed.LifeSpan;
                        }
                        else
                        {
                            cat.Name = "Unknown Cat";
                            cat.Description = "No breed information available";
                            cat.Origin = "Unknown";
                            cat.Temperament = "Unknown";
                            cat.LifeSpan = "Unknown";
                        }
                    }
                }

                return cats ?? new List<Cat>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error fetching cats: {ex.Message}");
                throw new Exception($"Network error: {ex.Message}. Check internet connection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching cats: {ex.Message}");
                throw;
            }
        }
    }
}