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
  [Fact]
  public async Task GetThemeSettings_ReturnsThemePreferences()
  {

    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await _authenticated_client.GetAsync("/api/theme-settings");
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var content = await response.Content.ReadAsStringAsync();


    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var result = JsonConvert.DeserializeObject<dynamic>(content);
    Assert.NotNull(result);
    Assert.True((bool)result.success);
    Assert.Equal("Theme preferences retrieved successfully", (string)result.message);


    Assert.NotNull(result.preferences);
    Assert.Equal("light", (string)result.preferences.theme);
    Assert.Equal("#4a90e2", (string)result.preferences.colorAccent);
    Assert.Equal("medium", (string)result.preferences.fontSize);
    Assert.False((bool)result.preferences.useSystemPreference);
  }
  [Fact]
  public async Task UpdateThemeSettings_ReturnsUpdatedThemeSettingsDto()
  {
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    var updateThemeDto = new UpdateThemeDto
    {
      Theme = "light",
      ColorAccent = "#4a90e2",
      FontSize = "medium",
      UseSystemPreference = true
    };

    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await _authenticated_client.PutAsJsonAsync("/api/theme-settings", updateThemeDto);
    var content = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var result = JsonConvert.DeserializeObject<dynamic>(content);
    Assert.NotNull(result);


    Assert.Equal("light", (string)result.preferences.theme);
    Assert.Equal("#4a90e2", (string)result.preferences.colorAccent);
    Assert.Equal("medium", (string)result.preferences.fontSize);
    Assert.True((bool)result.preferences.useSystemPreference);
  }

  [Fact]
  public async Task CreateTheme_ReturnsCreateThemeDto()
  {
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    var createThemeDto = new CreateThemeDto
    {
      Theme = "light",
      ColorAccent = "#4a90e2",
      FontSize = "medium",
      UseSystemPreference = false
    };

    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await _authenticated_client.PostAsJsonAsync("/api/theme-settings", createThemeDto);
    var content = await response.Content.ReadAsStringAsync(); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var result = JsonConvert.DeserializeObject<dynamic>(content);
    Assert.NotNull(result);


    Assert.Equal("light", (string)result.preferences.theme);
    Assert.Equal("#4a90e2", (string)result.preferences.colorAccent);
    Assert.Equal("medium", (string)result.preferences.fontSize);
    Assert.False((bool)result.preferences.useSystemPreference);
  }
  [Fact]
  public async Task DeleteThemeSettings_ResetsToDefaults()
  {

    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };


    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await _authenticated_client.DeleteAsync("/api/theme-settings");
    var content = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var result = JsonConvert.DeserializeObject<dynamic>(content);
    Assert.NotNull(result);



    var getResponse = await _authenticated_client.GetAsync("/api/theme-settings");
    var getContent = await getResponse.Content.ReadAsStringAsync();
    var getResult = JsonConvert.DeserializeObject<dynamic>(getContent);

    Assert.Equal("light", (string)getResult.preferences.theme);
    Assert.Equal("#4a90e2", (string)getResult.preferences.colorAccent);
    Assert.Equal("medium", (string)getResult.preferences.fontSize);
    Assert.False((bool)getResult.preferences.useSystemPreference);
  }

  [Fact]
  public async Task GetTrainingProgress_ReturnsTrainingProgressDto()
  {
    // Arrange
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    var petId = 1;

    // Act
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await TestHelper.GetAllTrainingsAsync(_authenticated_client, petId);

    // Assert
    Assert.NotNull(response);
    // Assert.Contains(response, r => r.Name == "TrainingName1");

  }

  [Fact]
  public async Task CreateTrainingProgress_ReturnsTrainingProgressDto()
  {
    // Arrange
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    var trainingProgressDto = new TrainingProgressCreateDto
    {
      Name = "New Training Name",
      Goals = ["New Goal 1", "New Goal 2"],
      PetId = 2
    };

    // Act
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await TestHelper.CreateTrainingProgressAsync(_authenticated_client, trainingProgressDto);

    // Assert
    Assert.NotNull(response);
    Assert.Equal("New Training Name", response.Name);
    Assert.Equal(["New Goal 1", "New Goal 2"], response.Goals);
    Assert.Equal(2, response.PetId);

  }

  [Fact]
  public async Task UpdateTrainingProgress_ReturnsTrainingProgressDto()
  {
    // Arrange
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    var trainingProgressDto = new TrainingProgressUpdateDto
    {
      Goals = ["New Goal 1", "New Goal 2"],
      Hours = 6
    };
    var id = 1;

    // Act
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await TestHelper.UpdateTrainingProgressAsync(_authenticated_client, trainingProgressDto, id);

    // Assert
    Assert.NotNull(response);
    Assert.Equal(["New Goal 1", "New Goal 2"], response.Goals);
    Assert.Equal(6, response.Hours);

  }

  public void Dispose()
  {
    _client.Dispose();
  }
}
