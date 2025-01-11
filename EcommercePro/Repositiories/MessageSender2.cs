using EcommercePro.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using static EtisiqueApi.Repositiories.MessageSender2;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EtisiqueApi.Repositiories
{
    public class MessageSender2
    {

        public class SMSConfig
        {
            public string ApiKey { get; set; }
            public string UserSender { get; set; }
            public string UserName { get; set; }
            public string MsgEncoding { get; set; }
            public string BaseAddress { get; set; }
            public string PostUrl { get; set; }
        }
        public class AppSettings
        {
           
            public SMSConfig SMSConfig { get; set; }
           
        }
        public interface IMessageSender
        {
             Task<bool> Send2Async(string phoneNumber, string msg, DateTime? exactTime);
            Task<bool> Send3Async(string phoneNumber, string msg, DateTime? exactTime);
        }
        public class MessageSender : IMessageSender
        {
            readonly Context _context;
            //readonly SMSConfig _config;
            readonly ILogger _logger;
            private IConfiguration configuration;
            private SMSConfig settings;
            private readonly IHttpContextAccessor _httpContextAccessor;


            public MessageSender(IConfiguration _configuration,Context context, ILogger<MessageSender> logger, IHttpContextAccessor httpContextAccessor)
            {
                //_config = config.Value.SMSConfig;
                _logger = logger;
                _httpContextAccessor = httpContextAccessor;
                _context = context;

                configuration = _configuration;
                settings = new SMSConfig();
                this.configuration.GetSection("SMSConfig").Bind(settings);
            }
            public async Task<bool> Send2Async(string phoneNumber, string msg, DateTime? exactTime)
            {
                if (Regex.IsMatch(phoneNumber, "^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$"))
                {
                    phoneNumber = "9665" + phoneNumber.Substring(2);
                }
                var baseAddress = new Uri(settings.BaseAddress);
                string strResponse = null;
                string exactTimeString = exactTime.HasValue ? @""",
                       ""exactTime"": """ + exactTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

                string timeToSend = exactTime.HasValue ? @""",
                       ""timeToSend"": """ + "later" : "";
                _logger.LogInformation("SMSLog:" + "test2");
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.ConnectionClose = true;

                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    string username = settings.UserName;
                    string password = settings.ApiKey;
                    string sender = settings.UserSender;
                    string[] gsm = { phoneNumber };
                    string text = msg;
                    string type = "4";
                    var payload = $@"{{""campaign"":{{""username"":""{username}"",""password"":""{password}"",""format"":""1"",""sender"":""{sender}"",""gsm"":{JsonConvert.SerializeObject(gsm)},""text"":""{text}"",""type"": ""{type}""}}}}";
                    using (var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json"))
                    {
                        try
                        {
                            using (var response = await httpClient.PostAsync(settings.PostUrl, content))
                            {

                                string responseHeaders = response.Headers.ToString();
                                string responseData = await response.Content.ReadAsStringAsync();
                                strResponse = ((int)response.StatusCode).ToString();
                                var responseDataObj = JsonConvert.DeserializeObject<dynamic>(responseData);
                                _logger.LogInformation("SMSLog:" + phoneNumber + ": " + msg + ":" + responseData);
                                string code = responseDataObj?.response?.status;
                                if (strResponse == "200" && code == "000")
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string host = httpClient.BaseAddress.Host;
                            IPAddress ipAddress = Dns.GetHostAddresses(host)[0]; // Assuming the first IP address in the list is the one you want
                            int providerPort = httpClient.BaseAddress.Port;

                            string providerIP = ipAddress.ToString();

                            _logger.LogInformation($"SMS Provider IP: {providerIP}, Port: {providerPort}");

                            // Handle the exception here, log it or perform any necessary actions.
                            _logger.LogError($"cant send sms, an error occurred: {ex.Message}");
                            // You might want to return false here to indicate that the operation failed due to an exception.
                            return false;
                        }
                    }

                }
                return false;

            }

            public async Task<bool> Send3Async(string phoneNumber, string msg, DateTime? exactTime)
            {
                List<string> phoneNumbers = new List<string>();
                phoneNumber.Split(",")?.ToList()?.ForEach(x =>
                {
                    if (Regex.IsMatch(x, "^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$"))
                    {
                        x = "9665" + x.Substring(2);
                    }
                    phoneNumbers.Add(x);
                });

                //if (Regex.IsMatch(phoneNumber, "^(05)(5|0|3|6|4|9|1|8|7)([0-9]{7})$"))
                //{
                //    phoneNumber = "9665" + phoneNumber.Substring(2);
                //}
                var baseAddress = new Uri(settings.BaseAddress);
                string strResponse = null;
                string exactTimeString = exactTime.HasValue ? @""",
                       ""exactTime"": """ + exactTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";

                string timeToSend = exactTime.HasValue ? @""",
                                   ""timeToSend"": """ + "later" : "";
                _logger.LogInformation("SMSLog:" + "test2");
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.ConnectionClose = true;

                    System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    string username = settings.UserName;
                    string password = settings.ApiKey;
                    string sender = settings.UserSender;
                    string[] gsm = phoneNumbers.ToArray();
                    string text = msg;
                    string type = "4";
                    var payload = $@"{{""campaign"":{{""accesskey"":""{password}"",""format"":""1"",""sender"":""{sender}"",""gsm"":{JsonConvert.SerializeObject(gsm)},""text"":""{text}"",""type"": ""{type}""}}}}";
                    string host = httpClient.BaseAddress.Host;
                    IPAddress ipAddress = Dns.GetHostAddresses(host)[0]; // Assuming the first IP address in the list is the one you want
                    int providerPort = httpClient.BaseAddress.Port;

                    string providerIP = ipAddress.ToString();
                    using (var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json"))
                    {
                        try
                        {
                            using (var response = await httpClient.PostAsync(settings.PostUrl, content))
                            {

                                string responseHeaders = response.Headers.ToString();
                                string responseData = await response.Content.ReadAsStringAsync();
                                strResponse = ((int)response.StatusCode).ToString();
                                var responseDataObj = JsonConvert.DeserializeObject<dynamic>(responseData);
                                _logger.LogInformation("SMSLog:" + phoneNumber + ": " + msg + ":" + responseData);
                                string code = responseDataObj?.response?.status;
                                if (strResponse == "200" && code == "000")
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {


                            _logger.LogInformation($"SMS Provider IP: {providerIP}, Port: {providerPort}");

                            // Handle the exception here, log it or perform any necessary actions.
                            _logger.LogError($"cant send sms, an error occurred: {ex.Message}");
                            // You might want to return false here to indicate that the operation failed due to an exception.
                            return false;
                        }
                        var ipAddress2 = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress;

                        _logger.LogInformation($"SMS Provider IP: {providerIP}, Port: {providerPort}");
                        _logger.LogInformation($"Current IP: {ipAddress2}");

                    }

                }
                return false;

            }
        }
    

        public  static class Messages
        {

            public static string NewRequestMsg(string clientName , int RequestCode=0 , string CloseCode=null)
            {

                string[] nameParts = clientName.Split(' ');

                // Extract the first name
                string firstName = nameParts[0];

                if (nameParts.Count() > 1)
                {
                  firstName = nameParts[0] + " " + nameParts[1];

                }
                var Message = $"هلا ومرحباً، {firstName} 👋" +
                                 $"لقد تم استلام طلبك الجديد بنجاح!" +
                                 $" رقم الطلب  {RequestCode} " +
                                 $" كود الإغلاق  {CloseCode}  ";

                return Message;

            }
            public static string NewEmergencyMsg( int RequestCode = 0, string CloseCode = null)
            {
                var Message = $"لقد تم استلام طلب الطوارىْ بنجاح!" +
                                 $" رقم الطلب  {RequestCode} " +
                                 $" كود الإغلاق  {CloseCode}  ";

                return Message;

            }
            public static string NewEmergencyMsgToAdmin( int RequestCode = 0)
            {
                var Message = $"لقد تم استلام طلب طوارىْ !" +
                                 $" رقم الطلب  {RequestCode} ";
 
                return Message;

            }
            public static string ApproveRequest(int RequestCode = 0)
            {
                var Message = $"تم ارسال طلب تعميد " +
                                 $" رقم الطلب  {RequestCode} ";

                return Message;

            }
            public static string ApprovedRequest(int RequestCode , string Approver)
            {
                var Message = $" تم تعميد الطلب" +
                                 $" رقم   {RequestCode} " +
                                 $"بواسطة {Approver}";

                return Message;

            }
            public static string RefusedRequest(int RequestCode , string Approver)
            {
                var Message = $" تم  رفض تعميد الطلب " +
                                 $" رقم   {RequestCode} " +
                                 $"بواسطة {Approver}";

                return Message;

            }
            

            public static string ReceiveMsg(string clientName,  string Code , string project,string building , string unitNo )
            {

                string[] nameParts = clientName.Split(' ');

                // Extract the first name
                string firstName = nameParts[0];

                if (nameParts.Count() > 1)
                {
                    firstName = nameParts[0] + " " + nameParts[1];

                }
                var Message = $"كود اقرار استلام وحدتى السكنية" +
                   $"رقم {unitNo} مبنى {building} مشروع {project} " +
                   $" هو  {Code}";


                return Message;

            }
            public static string StartRequest(string clientName, string Techincan,
                string TechincanNumber,int RequestCode)
            {

                string[] nameParts = clientName.Split(' ');

                // Extract the first name
                string firstName = nameParts[0];

                if (nameParts.Count() > 1)
                {
                    firstName = nameParts[0] + " " + nameParts[1];

                }
                 

                var Message = $"هلا ، {firstName} 👋" +
                    $" معك  {Techincan} فنى اتساق للصيانة سأكون معك اليوم لمعالجة طلبكم رقم {RequestCode} " +
                    $"الرقم الخاص بى  {TechincanNumber} " +
                                "يسعدنى خدمتك";

                return Message;

 
            }

            public static string ConfrimEmail( string code)
            {
                string message = "تحية طيبة،" +
                                 $"كود تفعيل الحساب  {code} ";
                

                return message;
            }
            public static string Up(string code)
            {
                string message = "تحية طيبة،" +
                                 $" تم تصعيد الطلب رقم {code} ";


                return message;
            }
            public static string ResetPassword(string userName, string code)
            {
                string[] nameParts = userName.Split(' ');

                // Extract the first name
                string firstName = nameParts[0];

                if (nameParts.Count() > 1)
                {
                    firstName = nameParts[0] + " " + nameParts[1];

                }
                string message = $"مرحبًا {firstName}،" +
                                "يُرجى استخدام الرمز التالي لإعادة تعيين كلمة المرور" +
                                $"كود إعادة تعيين كلمة المرور  {code} ";
                return message;
            }
            public static string ResetPasswordBySMS(string userName, string code)
            {
                string[] nameParts = userName.Split(' ');

                // Extract the first name
                string firstName = nameParts[0];

                if (nameParts.Count() > 1)
                {
                    firstName = nameParts[0] + " " + nameParts[1];

                }
                string message = $"مرحبًا {firstName}،" +
                                $"كود إعادة تعيين كلمة المرور   {code}";
                return message;
            }
            public static string Close(string customerName,string Id , string Code)
            {
                string[] nameParts = customerName.Split(' ');

                // Extract the first name
                string firstName = nameParts[0];

                if (nameParts.Count() > 1)
                {
                    firstName = nameParts[0] + " " + nameParts[1];

                }

                string message = $"عزيزى العميل تم اغلاق الطلب  رقم {Code} بنجاح" +
                    $" ولان رايك يفرق معنا نسعد بتقييم الخدمة المقدمة لكم "+
                    $"https://etsaq/app/clientPage/shrt/{Id}";
           
                return message;

            }


        }

}
}
