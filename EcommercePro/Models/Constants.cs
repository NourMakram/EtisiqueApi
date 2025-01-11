using static Azure.Core.HttpHeader;
using static EcommercePro.Models.Constants;

namespace EcommercePro.Models
{
	public static class ClaimConstants
	{
		///<summary>A claim that specifies the subject of an entity</summary>
		public const string Subject = "sub";

		///<summary>A claim that specifies the permission of an entity</summary>
		public const string Permission = "permission";
	}


	public static class Constants
	{
		public  class Emergency
		{
            public int Id { get; set; }
            public string Name { get; set; }
        }
		public static List<Emergency> Emergencies = new List<Emergency>()
		{
			new Emergency(){Id=1,Name="انقطاع ماء"},
			new Emergency(){Id=2,Name="توقف مصعد"},
            new Emergency(){Id=3,Name="انقطاع كهرباء"},
            new Emergency(){Id=4,Name="التماس كهرباء"},

        };

        public class Answer
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
		public class Question
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
		public static List<Answer> Answers = new List<Answer>()
		{
			new Answer { Id = 1, Name = "ضعيف" },
			new Answer { Id = 2, Name = "جيد" },
			new Answer { Id = 3, Name = "جيد جدا" },
			new Answer { Id = 4, Name = "ممتاز" },
		};

		public static List<Service> ServiceTypes = new List<Service>()
		{
			new Service { Id = 1, Name = "خدمات المنازل" },
			new Service { Id = 4, Name = "خدمات المطابخ" },

		};
		public class Service
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}
		public class Questeion
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		public enum BuildingCode : byte
		{
			A = 1, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
		}
		public enum RequestStuatus
		{
			Closed = 1,
			transfer = 2,
			Delay = 3,
			New = 0

		}
		public enum ServiceTypeEnum
		{
			ApartmentService = 1,
			CarWashService = 2,
			CommonParts = 3,
			KitchenService = 4,
            ComplaintService = 5,
            emergencyRequest=6,

        }



		public class ServiceRequest
		{
			public int id { set; get; }
			public string Name { set; get; }
		};











	}

	 public static class ConsQuesteion
    {
       
		public static List<Questeion> Questeions = new List<Questeion>()
        {
            new Questeion { Id = 1, Name = "جودة اداء الفنى" },
            new Questeion { Id = 2, Name = "سرعة اداء الفنى" },
            new Questeion { Id = 3, Name = "نظافة المكان بعد الصيانة" },
            new Questeion { Id = 4, Name = "حضور الفنى فى الوقت المحدد" },
            new Questeion { Id = 5, Name = "المظهر العام للفنى" },

        };

    }
}
