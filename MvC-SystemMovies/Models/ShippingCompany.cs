using System.ComponentModel.DataAnnotations;

namespace MvC_SystemMovies.Models
{
    public class ShippingCompany
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, 60)]
        public int EstimatedDays { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
