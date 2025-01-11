using System.ComponentModel.DataAnnotations;

namespace EtisiqueApi.DTO
{
    public class RequestManagment
    {

    }
    public class ReplyDto
    {
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }

        [Required(ErrorMessage = "Message is Required")]
        public string Message { get; set; }



    }
    public class AgreemantFileDto
    {
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "File is Required")]
        public IFormFile file { set; get; }
        public string? filePath { set; get; }
    }
    public class StartDto
    {
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { get; set; }

        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }


    }

    public class TransferDto
    {
        [Required (ErrorMessage = "techiancalId is Required")]
       public  string techiancalId { set; get; }
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { set; get; }
        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }
    }
    public class CloseDto
    {
        [Required(ErrorMessage = "Code is Required")]
        public string Code { set; get; }
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { set; get; }
        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }
        [Required(ErrorMessage = "File is Required")]
        public List<IFormFile>? formFiles { get; set; }
        //public string? filePath { set; get; }
    }
    public class PostponeDto
    {
        [Required(ErrorMessage = "PostponeDate is Required")]
        public string PostponeDate { set; get; }

        [Required(ErrorMessage = "Note is Required")]
        public string Note { set; get; }
        [Required(ErrorMessage = "File is Required")]
        public List<IFormFile>? formFiles { get; set; }
        //public IFormFile? file { set; get; }
        //public string? filePath { set; get; }
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { set; get; }
        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }
    }
    public class CommonPartsmanagmentDto
    {
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { set; get; }
        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }
        public string? Note { set; get; }
        //[Required(ErrorMessage = "File is Required")]
        //public IFormFile file { set; get; }
        //public string? filePath { set; get; }
        [Required(ErrorMessage = "File is Required")]
        public List<IFormFile>? formFiles { get; set; }

    }
    public class ApproveRequestDto
    {
        public int RequestId { set; get; }
        public string userId { set; get; }
        public string ApproverId { set; get; }
        public string? Note { set; get; }
        public IFormFile? FormFile { set; get; }
        public decimal Fees { set; get; }
       
 
    }

    public class CloseByNoteDto
    {
        [Required(ErrorMessage = "RequestId is Required")]
        public int RequestId { set; get; }
        [Required(ErrorMessage = "userId is Required")]
        public string userId { set; get; }
        [Required(ErrorMessage = "Note is Required")]

        public string Note { set; get; }
    }

}
