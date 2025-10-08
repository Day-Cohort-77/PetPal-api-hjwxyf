namespace PetPal.API.Models;

public class EmergencyServicesResponse
 {
    public List<EmergencyVetClinic> EmergencyServices { get; set; }
    public EmergencyGuidelines EmergencyGuidelines { get; set; }
    public Pagination Pagination { get; set; }
}
public class EmergencyVetClinic
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }


    public Address Address { get; set; }
    public Contact Contact { get; set; }
    public Hours Hours { get; set; }
    public Location Location { get; set; }
    public List<string> Services { get; set; }
    public WaitTime WaitTime { get; set; }
    public Distance Distance { get; set; }
    public Directions Directions { get; set; }


}
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

    }
    public class Contact
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
    }
    public class Hours
    {
        public bool Is24Hour { get; set; }
        public bool IsOpen { get; set; }
        public List<RegularHour> RegularHours { get; set; } // List of regular hours for each day of the week

    }


    public class RegularHour
    {
        public string Day { get; set; }
        public string Open { get; set; }
        public string Close { get; set; }

    }
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
    public class WaitTime
    {
        public int EstimatedMinutes { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    public class Distance
    {
        public double Miles { get; set; }
        public int EstimatedDriveTimeMinutes { get; set; }

    }

    public class Directions
    {
        public string GoogleMapsUrl { get; set; }
        public string AppleMapsUrl { get; set; }
    }


 

public class EmergencyGuidelines
{
    public List<string> CommonEmergencySigns { get; set; }
    public List<string> ImmediateActions { get; set; }
}
public class Pagination
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems{ get; set; }
    public int TotalPages { get; set; }
}

