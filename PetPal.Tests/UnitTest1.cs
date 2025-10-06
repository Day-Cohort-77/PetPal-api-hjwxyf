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
  public async Task GetEmergencyServices_ReturnsExpectedData()
  {
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await _authenticated_client.GetAsync("/api/emergency-services?latitude=37.7749&longitude=-122.4194");
    
    var content = await response.Content.ReadAsStringAsync();
    output.WriteLine($"Response content: {content}");


    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var result = JsonConvert.DeserializeObject<EmergencyServicesResponse>(content);
    Assert.NotNull(result);

    Assert.NotNull(result.EmergencyServices);
    Assert.NotNull(result.EmergencyGuidelines);
    Assert.NotNull(result.Pagination);

  
    Assert.True(result.EmergencyServices.Count > 0);

    
    var firstClinic = result.EmergencyServices[0];
    Assert.Equal("clinic123", firstClinic.Id);
    Assert.Equal("24/7 Pet Emergency Hospital", firstClinic.Name);
    Assert.Equal("emergency", firstClinic.Type);
  }
  [Fact]
  public async Task GetEmergencyServices_WithCoordinates_CalculatesDistancesCorrectly()
  { 
  
    var login = new LoginDto
    {
      Email = "admin@petpal.com",
      Password = "Admin123!"
    };
    double latitude = 37.7749;
    double longitude = -122.4194;

    _authenticated_client = await GetAuthenticatedClientAsync(login);
    var response = await _authenticated_client.GetAsync("/api/emergency-services?latitude=37.7749&longitude=-122.4194");
    var content = await response.Content.ReadAsStringAsync();
    output.WriteLine($"Response content: {content}");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var result = JsonConvert.DeserializeObject<EmergencyServicesResponse>(content);
    Assert.NotNull(result);
    Assert.NotNull(result.EmergencyServices);

    foreach (var clinic in result.EmergencyServices)
    {
      // Each clinic should have distance information
      Assert.NotNull(clinic.Distance);
      Assert.True(clinic.Distance.Miles > 0);
      Assert.True(clinic.Distance.EstimatedDriveTimeMinutes > 0);

      // Verify the math roughly matches expected distance
      double calculatedDistance = CalculateDistance(
          latitude,
          longitude,
          clinic.Location.Latitude,
          clinic.Location.Longitude);

      Assert.Equal(Math.Round(calculatedDistance, 1), clinic.Distance.Miles);
    }

    // Verify sorting - closest first
    for (int i = 0; i < result.EmergencyServices.Count - 1; i++)
    {
      Assert.True(
          result.EmergencyServices[i].Distance.Miles <=
          result.EmergencyServices[i + 1].Distance.Miles,
          "Clinics should be sorted by distance (closest first)"
      );
    }
  }

  // Add this helper to your test class to verify distances
  private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
  {
    const double earthRadiusMiles = 3959;

    var dLat = ToRadians(lat2 - lat1);
    var dLon = ToRadians(lon2 - lon1);

    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    return earthRadiusMiles * c;
  }

  private double ToRadians(double degrees)
  {
    return degrees * Math.PI / 180;
  }


  public void Dispose()
  {
    _client.Dispose();
  }
}
