using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PetPal.API.DTOs;
using PetPal.API.Models;
using Xunit.Abstractions;

namespace PetPal.Tests;

public class PetTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
  private readonly TestWebApplicationFactory _factory;
  private protected HttpClient _client;
  private HttpClient _authenticated_client;

  private readonly ITestOutputHelper output;

  public PetTests(ITestOutputHelper output, TestWebApplicationFactory factory)
  {
    this.output = output;
    _factory = factory;
    _client = _factory.CreateClient();
  }

  protected async Task<HttpClient> GetAuthenticatedClientAsync(LoginDto credentials)
  {
    var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });

    var loginResponse = await client.PostAsJsonAsync("/auth/login", credentials);

    // Extract cookies from login response
    if (loginResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
    {
      foreach (var cookie in cookies)
      {
        client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
      }
    }

    return client;
  }

  [Fact]
  public async Task GetAllPets_ReturnsAllPets()
  {
    // Arrange
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };

    // Act
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var pets = await TestHelper.GetAllPetsAsync(_authenticated_client);
    // output.WriteLine(JsonConvert.SerializeObject(pets));

    // Assert
    Assert.NotNull(pets);
    // We don't check for a specific count as the number of items can vary from 0 to n
    Assert.Contains(pets, p => p.Name == "Name1");
    Assert.Contains(pets, p => p.Name == "Name2");
    Assert.Contains(pets, p => p.Name == "Name3");

  }
  public void Dispose()
  {
    _client.Dispose();
  }
}
