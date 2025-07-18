using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FoodShelterAppC_.Services
{
    public class OpenStreetMapService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenStreetMapService> _logger;

        public OpenStreetMapService(ILogger<OpenStreetMapService> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "FoodShelterApp/1.0 (your.email@example.com)");
            _logger = logger;
        }

        private class NominatimResult
        {
            public long place_id { get; set; }
            public string osm_type { get; set; }
            public long osm_id { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
        }

        public async Task<(float? Latitude, float? Longitude)> GeocodeAddress(string address)
        {
            try
            {
                var encodedAddress = Uri.EscapeDataString(address);
                var url = $"https://nominatim.openstreetmap.org/search?format=json&q={encodedAddress}&limit=1";

                await Task.Delay(1000); // Respect rate limiting

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Nominatim response: {content}");
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var results = JsonSerializer.Deserialize<List<NominatimResult>>(content, options);

                if (results != null && results.Any() && 
                    !string.IsNullOrEmpty(results[0].lat) && 
                    !string.IsNullOrEmpty(results[0].lon))
                {
                    var lat = float.Parse(results[0].lat, CultureInfo.InvariantCulture);
                    var lon = float.Parse(results[0].lon, CultureInfo.InvariantCulture);
                    
                    _logger.LogInformation($"Found coordinates: Lat={lat}, Lon={lon}");
                    return (lat, lon);
                }

                _logger.LogWarning($"No valid coordinates found for address: {address}");
                return (null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error geocoding address: {Address}", address);
                return (null, null);
            }
        }
    }
}
