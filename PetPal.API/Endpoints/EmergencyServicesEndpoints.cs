using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using PetPal.API.Models;
using System;
using System.Collections.Generic;




namespace PetPal.API.Endpoints;

public static class EmergencyServicesEndpoints
{
    public static void MapEmergencyServicesEndpoints(this WebApplication app)
    {
        app.MapGet("/api/emergency-services", (double? latitude, double? longitude) =>
        {
            // Create sample data matching your response schema
            var response = new EmergencyServicesResponse
            {
                EmergencyServices = new List<EmergencyVetClinic>
                {
                    new EmergencyVetClinic
                    {
                        Id = "clinic123",
                        Name = "24/7 Pet Emergency Hospital",
                        Type = "emergency",
                        Address = new Address
                        {
                            Street = "456 Emergency Lane",
                            City = "San Francisco",
                            State = "CA",
                            ZipCode = "94107",
                            Country = "USA"
                        },
                        Location = new Location
                        {
                            Latitude = 37.790,
                            Longitude = -122.4200
                        },
                        Contact = new Contact
                        {
                            Phone = "555-911-PETS",
                            Email = "info@petemergency.com",
                            Website = "https://petemergency.com"
                        },
                        Hours = new Hours
                        {
                            IsOpen = true,
                            Is24Hour = true,
                            RegularHours = new List<RegularHour>
                            {
                                new RegularHour { Day = "Monday", Open = "00:00", Close = "24:00" },
                                new RegularHour { Day = "Tuesday", Open = "00:00", Close = "24:00" }
                            }
                        },
                        Services = new List<string>
                        {
                            "Emergency Surgery",
                            "Critical Care",
                            "Diagnostic Imaging",
                            "Laboratory Services"
                        },
                        WaitTime = new WaitTime
                        {
                            EstimatedMinutes = 45,
                            LastUpdated = DateTime.Parse("2025-06-12T19:20:00Z")
                        },
                        Distance = new Distance
                        {
                            Miles = 1.2,
                            EstimatedDriveTimeMinutes = 8
                        },
                        Directions = new Directions
                        {
                            GoogleMapsUrl = "https://maps.google.com/?q=456+Emergency+Lane+San+Francisco+CA+94107",
                            AppleMapsUrl = "https://maps.apple.com/?address=456+Emergency+Lane,San+Francisco,CA,94107"
                        }
                    },
                    new EmergencyVetClinic
                    {
                        Id = "clinic456",
                        Name = "Bay Area Veterinary Emergency",
                        Type = "emergency",
                        Address = new Address
                        {
                            Street = "789 Urgent Ave",
                            City = "San Francisco",
                            State = "CA",
                            ZipCode = "94110",
                            Country = "USA"
                        },
                        Location = new Location
                        {
                            Latitude = 37.7590,
                            Longitude = -122.4150
                        },
                        Contact = new Contact
                        {
                            Phone = "555-VET-HELP",
                            Email = "help@bayareavet.com",
                            Website = "https://bayareavet.com"
                        },
                        Hours = new Hours
                        {
                            IsOpen = true,
                            Is24Hour = false,
                            RegularHours = new List<RegularHour>
                            {
                                new RegularHour { Day = "Monday", Open = "18:00", Close = "08:00" }
                            }
                        },
                        Services = new List<string>
                        {
                            "Emergency Surgery",
                            "Critical Care",
                            "Poison Control"
                        },
                        WaitTime = new WaitTime
                        {
                            EstimatedMinutes = 30,
                            LastUpdated = DateTime.Parse("2025-06-12T19:25:00Z")
                        },
                        Distance = new Distance
                        {
                            Miles = 2.8,
                            EstimatedDriveTimeMinutes = 15
                        },
                        Directions = new Directions
                        {
                            GoogleMapsUrl = "https://maps.google.com/?q=789+Urgent+Ave+San+Francisco+CA+94110",
                            AppleMapsUrl = "https://maps.apple.com/?address=789+Urgent+Ave,San+Francisco,CA,94110"
                        }
                    }
                },
                EmergencyGuidelines = new EmergencyGuidelines
                {
                    CommonEmergencySigns = new List<string>
                    {
                        "Difficulty breathing",
                        "Excessive bleeding",
                        "Inability to urinate",
                        "Seizures",
                        "Collapse or inability to stand",
                        "Severe vomiting or diarrhea",
                        "Suspected poisoning",
                        "Trauma (hit by car, fall, etc.)"
                    },
                    ImmediateActions = new List<string>
                    {
                        "Remain calm",
                        "Call the nearest emergency vet before arriving if possible",
                        "Safely transport your pet (use a carrier for cats, muzzle for dogs if necessary)",
                        "Bring medical records if available"
                    }
                },
                Pagination = new Pagination
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 2,
                    TotalPages = 1
                }
            };
            if (latitude.HasValue && longitude.HasValue)
            {
                foreach (var clinic in response.EmergencyServices)
                {
                    var distance = CalculateDistance(
                        latitude.Value,
                        longitude.Value,
                        clinic.Location.Latitude,
                        clinic.Location.Longitude
                    );

                    clinic.Distance = new Distance
                    {
                        Miles = Math.Round(distance, 1),
                        EstimatedDriveTimeMinutes = (int)(distance * 2) // Rough estimate: 2 min per mile
                    };
                }

                // Sort by distance
                response.EmergencyServices = response.EmergencyServices
                    .OrderBy(c => c.Distance.Miles)
                    .ToList();
            }

            return Results.Ok(response);
        });
    }

        
       private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula to calculate distance between two points
        const double earthRadiusMiles = 3959;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusMiles * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}



