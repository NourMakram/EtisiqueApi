using EcommercePro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtisiqueApi.DTO
{
    public class QuestionnaireDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Enter RequestCode")]
        public int RequestCode { get; set; }

        [Required(ErrorMessage = "Enter RequestId")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "Enter CustomerId")]
        public string CustomerId { set; get; }

        [Required(ErrorMessage = "Enter ServiceType")]
        public int ServiceType { get; set; } //خدمات منازل ام غسيل سيارات - اجزاء مشتركة
       
        [Required(ErrorMessage = "Enter TechnicianId")]
        public string TechnicianId { get; set; }

        public string? TechnicianNote { get; set; }
        public string? ServiceNote { get; set; }
        [Required(ErrorMessage = "Enter Rating")]
        public int Rating { get; set; }
        [Required(ErrorMessage = "Enter Answers")]
        public List<int> Answers { get; set; }
    }
    public class EvelautionServiceDto
    {
        public int Id { get; set; }
        public int RequestCode { get; set; }
        public int RequestId { set; get; }
        public string CustomerName { set; get; }
        public string CustomerPhone { set; get; }
        public int ServiceType { get; set; } //خدمات منازل ام غسيل سيارات - اجزاء مشتركة
        public string TechnicianName { get; set; }
        public string TechnicianNote { get; set; }
        public string ServiceNote { get; set; }
        public string ManagerNote { get; set; }
        public string ProjectName { get; set; } 
        public string date { set; get; }
        public int Rating { get; set; }
        public List<int> Answers { get; set; }

    }
    public class EvelautionTechnicianAVgDto
    {
        
        public string question { set; get; }
        public double average20 { get; set; }
        public double average40 { get; set; }
        public double average80 { get; set; }
        public double average100 { get; set; }


    }
    public class EvelautionTechnicianDto
    {

        public List<EvelautionTechnicianAVgDto> AvarageRating { get; set; }
        public int Count { get; set; }

    }


    public class EvelautionServiceNoteDto
    {
        [Required(ErrorMessage ="Enter the EvelautionService Id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter the Note ")]
        public string Note { set; get; }
    }



    }
