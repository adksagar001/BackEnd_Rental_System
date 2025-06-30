using System.ComponentModel.DataAnnotations;

namespace MobileAppsAPIS.Model
{
    public class RoomBooking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [MaxLength(500)]
        public string SpecialRequests { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    }
}
