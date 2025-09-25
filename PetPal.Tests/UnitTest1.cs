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

public class UnitTests : IClassFixture<TestWebApplicationFactory>, IDisposable
{
  private readonly TestWebApplicationFactory _factory;
  private protected HttpClient _client;
  private HttpClient _authenticated_client;

  private readonly ITestOutputHelper output;

  public UnitTests(ITestOutputHelper output, TestWebApplicationFactory factory)
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

    [Fact]
  public async Task UpdateUserProfile_ReturnsUserUpdateResponseDto()
  {
    // Arrange
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    var userUpdateDto = new UserUpdateDto
    {
      FirstName = "Admina",
      LastName = "Straytor",
      Email = "admin@petpal.com",
      Phone = "777-123-4567",
      Address = "125 Admin St"
    };
    var id = 1;

    // Act
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await TestHelper.UpdateUserProfileAsync(_authenticated_client, userUpdateDto, id);

    // Assert
    Assert.NotNull(response);
    // We don't check for a specific count as the number of items can vary from 0 to n
    Assert.True(response.Success);
    Assert.Equal("Profile updated successfully", response.Message);
    Assert.Equal("Admina", response.User.FirstName);
    Assert.Equal("Straytor", response.User.LastName);
    Assert.Equal("admin@petpal.com", response.User.Email);
    Assert.Equal("777-123-4567", response.User.Phone);
    Assert.Equal("125 Admin St", response.User.Address);

  }
  public void Dispose()
  {
    _client.Dispose();
  }
}
