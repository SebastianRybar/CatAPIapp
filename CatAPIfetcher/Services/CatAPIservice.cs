using System.Net.Http.Json;
using CatAPIfetcher.Model;

namespace CatAPIfetcher.Services
{
    public class CatAPIservice
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.thecatapi.com/v1";

        public CatAPIservice(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<List<Cat>> GetRandomCatsAsync(int limit = 20)
        {
            try
            {
                var cats = await _httpClient.GetFromJsonAsync<List<Cat>>($"/images/search?limit={limit}&has_breeds=1");

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
                        }
                    }
                }

                return cats ?? new List<Cat>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching cats: {ex.Message}");
                return new List<Cat>();
            }
        }
    }
}