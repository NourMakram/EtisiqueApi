using EtisiqueApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommercePro.Models
{
	public class Location
	{
		public int Id { get; set; }

		[ForeignKey("Customer")]
		public int? customerId { get; set; }
		public Customer? Customer { get; set; }

		[Required(ErrorMessage = "Latitude value is Required")]
		public double Latitude { get; set; }
		[Required(ErrorMessage = "Longitude value is Required")]
		public double Longitude { get; set; }


	}
}
