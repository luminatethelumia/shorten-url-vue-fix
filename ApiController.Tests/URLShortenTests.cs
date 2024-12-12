using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using URLshorten.Data;
using URLshorten.Models;

namespace ApiController.Tests
{
    public class URLShortenTests : IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly URLshortenContext _context;

        public URLShortenTests()
        {
            _factory = new CustomWebApplicationFactory();

            var opts = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };
            _client = _factory.CreateClient(opts);

            _client.BaseAddress = new Uri("https://localhost:3000");
            _context = _factory.Services.GetRequiredService<URLshortenContext>();
        }

        [Fact]
        public async Task Test_Get_All_Urls()
        {
            // Arrange: Seed the database with a specific code and URL.

            // Act: Make a GET request to the redirect endpoint.
            var response = await _client.GetAsync("/api");            

            // Assert: Check if the response is a redirection to the correct URL.

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();            
            Assert.Contains("https://localhost:3000/api/", content);
        }

        [Fact]
        public async Task Test_Get_Redirect_Url()
        {
            // Arrange: Seed the database with the specific code and URL.
            var code = "abc123";
            var expectedRedirectUrl = "https://www.youtube.com/watch?v=zaRM0iIhJvs";

            // Act: Make a GET request to the redirect endpoint with automatic redirect disabled.
            
            var response = await _client.GetAsync($"/api/{code}");

            // Assert: Check if the response is a redirection and matches the correct URL.
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            Assert.Equal(expectedRedirectUrl, response.Headers.Location.ToString());
        }

        [Fact]
        public async Task Test_Create_Shortened_URL()
        {
            // Arrange: Define the input payload for the POST request.
            var payload = new { Url = "https://www.infoworld.com/article/2336587/how-to-use-ef-core-as-an-in-memory-database-in-asp-net-core-6.html" };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Act: Make a POST request to create a new shortened URL.
            var response = await _client.PostAsync("/api/Shorten", content);

            // Assert: Check if the response is successful and the returned data is as expected.
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("https://localhost:3000/api/", responseContent);
        }

        [Fact]
        public async Task Test_Delete_Shortened_URL()
        {
            // Arrange: Seed the database with a specific shortened URL to delete.
            var payload = new { Url = "https://localhost:3000/api/abc123" };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            // Create the DELETE request with the payload
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/delete-shorten")
            {
                Content = content
            };

            // Act: Send the DELETE request
            var response = await _client.SendAsync(request);            

            // Assert: Check if the response is successful and returns the expected message.
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal("Removed Url Succesfully", responseContent);
        }

        [Fact]
        public async Task Test_Edit_Shortened_URL()
        {
            // Arrange: Prepare the test data
            var existingUrl = "https://localhost:3000/api/abc123";
            var newCode = "newCode123";

            var url = await _context.UrlShortenModel.FirstOrDefaultAsync(u => u.ShortUrl == existingUrl);

            Assert.NotNull(url);         

            // Create the payload to update the shortened URL
            var urlDto = new UrlDto
            {
                Url = existingUrl
            };

            // Act: Send the PUT request to update the shortened URL
            var content = new StringContent(JsonSerializer.Serialize(urlDto), Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"/api/edit-shorten?code={newCode}", content);

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            // deserialize the JSON string to JSONelement to access the key : value
            var toJsonResponseContent = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var urlKey = toJsonResponseContent.GetProperty("url");

            Assert.Equal($"https://localhost:3000/api/{newCode}", urlKey.GetString());
        }

        [Fact]
        public async Task Test_Edit_Origin_URL()
        {
            // Arrange: Prepare the test data
            var newUrl = "https://github.com/";
            var code = "abc123";

            var url = await _context.UrlShortenModel.FirstOrDefaultAsync(u => u.Code == code);

            Assert.NotNull(url);

            // Create the payload to update the shortened URL
            var urlDto = new UrlDto
            {
                Url = newUrl
            };

            // Act: Send the PUT request to update the shortened URL
            var content = new StringContent(JsonSerializer.Serialize(urlDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/edit-origin?code={code}")
            {
                Content = content
            };
            var response = await _client.SendAsync(request);

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            // deserialize the JSON string to JSONelement to access the key : value
            var toJsonResponseContent = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var urlKey = toJsonResponseContent.GetProperty("url");

            Assert.Equal(newUrl, urlKey.GetString());
        }


        [Fact]
        public async Task Test_QRCode_For_Shortened_URL()
        {
            // Arrange: Prepare the test data
            var code = "abc123";
            var url = await _context.UrlShortenModel.FirstOrDefaultAsync(u => u.Code == code);

            Assert.NotNull(url);
            var qrImage = url.QRImage;

            // Act: Send the GET request to generate the QR code
            var response = await _client.GetAsync($"/api/qrcode/{code}");

            // Assert: Verify the response
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify the QR code is returned as an image (application/octet-stream or image/png)
            var contentType = response.Content.Headers.ContentType.MediaType;
            Assert.NotNull(contentType);
            Assert.True(contentType.StartsWith("image/"), "The content type must be image");

            // Optionally, check the response body to ensure it's not empty (you could verify the QR code size)
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(qrImage, imageBytes);
            
        }


        /// <summary>
        /// Dispose the disposable instances after used.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
            _context.Dispose();
        }
    }
}