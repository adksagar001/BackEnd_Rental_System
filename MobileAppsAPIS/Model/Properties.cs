using MobileAppsAPIS.Classes;

namespace MobileAppsAPIS.Model
{
    public class Property
    {
        public int PropertyId { get; set; }
        public string City { get; set; }
        public int WardNo { get; set; }
        public string Area { get; set; }
        public string ContactNo { get; set; }
        public string PropertyType { get; set; }
        public double EstimatedPrice { get; set; }
        public int TotalRooms { get; set; }
        public int Bedroom { get; set; }
        public int LivingRoom { get; set; }
        public int Kitchen { get; set; }
        public string Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int OwnerId { get; set; }
        public int Booked { get; set; }
        public string Status { get; set; }
        public int Active { get; set; }

        public int StreetNo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Ptype { get; set; }

        public List<string> Services { get; set; }

        public List<string> ImagePaths { get; set; }
    }

}


