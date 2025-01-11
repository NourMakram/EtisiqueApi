namespace EtisiqueApi.Models
{
    public class RequestsImages
    {
        public int Id { get; set; }
        public int ServiceType { get; set; } //خدمات منازل ام غسيل سيارات - اجزاء مشتركة
        public int RequestId { get; set; }

        public string imagepath { get; set; }

    }
}
