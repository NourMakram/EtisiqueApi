using System.Collections.ObjectModel;
using System.Reflection;

namespace EcommercePro.Models
{
	public static class Utils
	{
		public static Dictionary<string, object> DictionaryOfPropertiesFromClass(Type TheType, Type? FieldType)
		{
			FieldInfo[] fields = TheType.GetFields(BindingFlags.Static | BindingFlags.Public);
			Dictionary<string, object> d = new Dictionary<string, object>();
			foreach (FieldInfo fi in fields)
			{
				try
				{
					if (FieldType == null || fi.FieldType.Name == FieldType.Name)
						d.Add(fi.Name, fi.GetValue(null));

				}
				catch
				{

				}

			}
			return d;
		}
	}
    public class ApplicationPermissions
    {
        public static ReadOnlyCollection<ApplicationPermission> AllPermissions;


         

        #region User Permission
        public const string UsersPermissionGroupName = "صلاحيات ادراة المستخدمين والوظائف";
        public static ApplicationPermission ViewUsers = new ApplicationPermission("عرض جميع المستخدمين", "users.view", UsersPermissionGroupName, "Permission to view other users account details");
        public static ApplicationPermission ViewClients = new ApplicationPermission("عرض جميع العملاء", "users.viewClients", UsersPermissionGroupName, "");
        public static ApplicationPermission AddClients = new ApplicationPermission("عرض جميع الفنيين", "users.viewTechnicians", UsersPermissionGroupName, "");
        public static ApplicationPermission ManageUsers = new ApplicationPermission("ادارة المستخدمين", "users.manage", UsersPermissionGroupName, "Permission to create,  modify other users account details");
        public static ApplicationPermission ViewRoles = new ApplicationPermission("عرض جميع الوظائف", "roles.view", UsersPermissionGroupName, "Permission to view available roles");
        public static ApplicationPermission ManageRoles = new ApplicationPermission("ادارة الوظائف", "roles.manage", UsersPermissionGroupName, "Permission to create, delete and modify roles");
        #endregion

        #region Project Permission 
        public const string ProjectsPermissionGroupName = "صلاحيات ادراة المشاريع ";
        public static ApplicationPermission ViewProjects = new ApplicationPermission("عرض جميع المشاريع", "projects.view", ProjectsPermissionGroupName, "Permission to view projects");
        public static ApplicationPermission ManageProjects = new ApplicationPermission("ادارة المشاريع ", "projects.manage", ProjectsPermissionGroupName, "Permission to create and modify projects");

        #endregion


        #region Maintenance requests 
        public const string MaintenanceRequestsGroupName = " صلاحيات خدمات المنازل ";

        public static ApplicationPermission ViewAllMaintenanceRequests = new ApplicationPermission("عرض جميع الطلبات", "maintenanceRequests.view", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission AddRequestMaintenanceRequests = new ApplicationPermission("اضافة", "maintenanceRequests.Add", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission ShowRequestMaintenanceRequests = new ApplicationPermission("عرض تفاصيل", "maintenanceRequests.Show", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission TransferMaintenanceRequests = new ApplicationPermission("تحويل", "maintenanceRequests.transfer", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission TrackMaintenanceRequests = new ApplicationPermission("متابعة", "maintenanceRequests.Track", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission CloseMaintenanceRequests = new ApplicationPermission("اغلاق", "maintenanceRequests.close", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission DelayMaintenanceRequests = new ApplicationPermission("تأجيل", "maintenanceRequests.Delay", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission ReplyMaintenanceRequests = new ApplicationPermission("رد", "maintenanceRequests.Reply", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission StartMaintenanceRequests = new ApplicationPermission("بدء", "maintenanceRequests.Start", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission DeleteMaintenanceRequests = new ApplicationPermission("حذف", "maintenanceRequests.Delete", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission ApproveMaintenanceRequests = new ApplicationPermission("تحويل تعميد", "maintenanceRequests.Transfer2", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission AcceptMaintenanceRequests = new ApplicationPermission("قبول تعميد", "maintenanceRequests.Accept", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission RefuseMaintenanceRequests = new ApplicationPermission("رفض تعميد", "maintenanceRequests.Refuse", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission ClientsMaintenanceRequests = new ApplicationPermission("طلبات العميل", "maintenanceRequests.Clients", MaintenanceRequestsGroupName, "");
        public static ApplicationPermission TechniciansMaintenanceRequests = new ApplicationPermission("طلبات الفنى", "maintenanceRequests.Technicians", MaintenanceRequestsGroupName, "");
        
        #endregion

        #region EmergencyRequests requests 
        public const string EmergencyRequestsGroupName = " صلاحيات طلبات الطوارىْ ";

        public static ApplicationPermission ViewAllEmergencyRequests = new ApplicationPermission("عرض جميع الطلبات", "EmergencyRequests.view", EmergencyRequestsGroupName, "");
        public static ApplicationPermission AddRequestEmergencyRequests = new ApplicationPermission("اضافة", "EmergencyRequests.Add", EmergencyRequestsGroupName, "");
        public static ApplicationPermission ShowRequestEmergencyRequests = new ApplicationPermission("عرض تفاصيل", "EmergencyRequests.Show", EmergencyRequestsGroupName, "");
        public static ApplicationPermission TransferEmergencyRequests = new ApplicationPermission("تحويل", "EmergencyRequests.transfer", EmergencyRequestsGroupName, "");
        public static ApplicationPermission TrackEmergencyRequests = new ApplicationPermission("متابعة", "EmergencyRequests.Track", EmergencyRequestsGroupName, "");
        public static ApplicationPermission CloseEmergencyRequests = new ApplicationPermission("اغلاق", "EmergencyRequests.close", EmergencyRequestsGroupName, "");
         public static ApplicationPermission ReplyEmergencyRequests = new ApplicationPermission("رد", "EmergencyRequests.Reply", EmergencyRequestsGroupName, "");
        public static ApplicationPermission StartEmergencyRequests = new ApplicationPermission("بدء", "EmergencyRequests.Start", EmergencyRequestsGroupName, "");
        public static ApplicationPermission DeleteEmergencyRequests = new ApplicationPermission("حذف", "EmergencyRequests.Delete", EmergencyRequestsGroupName, "");
        public static ApplicationPermission ClientsEmergencyRequests = new ApplicationPermission("طلبات العميل", "EmergencyRequests.Clients", EmergencyRequestsGroupName, "");
        public static ApplicationPermission TechniciansEmergencyRequests = new ApplicationPermission("طلبات الفنى", "EmergencyRequests.Technicians", EmergencyRequestsGroupName, "");
        // public static ApplicationPermission ProjectManagerMaintenanceRequests = new ApplicationPermission("طلبات مدير مشروع", "maintenanceRequests.projectManager", MaintenanceRequestsGroupName, "");

        #endregion 
        #region CommonPartsrequests 
        public const string CommonPartsrequestsGroupName = "صلاحيات الأجزاء المشتركة ";
        public static ApplicationPermission ViewAllCommonPartsrequests = new ApplicationPermission("عرض جميع الطلبات", "commonPartsRequest.view", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission AddCommonPartsrequest = new ApplicationPermission("اضافة", "commonPartsRequest.Add", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission ShowCommonPartsrequest = new ApplicationPermission("عرض", "commonPartsRequest.Show", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission TransferCommonPartsrequest = new ApplicationPermission("تحويل", "commonPartsRequest.transfer", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission TrackCommonPartsrequest = new ApplicationPermission("متابعة", "commonPartsRequest.Track", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission CommentCommonPartsrequest = new ApplicationPermission("تعليق", "commonPartsRequest.Comment", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission CloseCommonPartsrequest = new ApplicationPermission("اغلاق", "commonPartsRequest.close", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission PartialCloseCommonPartsrequest = new ApplicationPermission("اقفال", "commonPartsRequest.PartialClose", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission PartialClose1CommonPartsrequest = new ApplicationPermission("اقفال اول", "commonPartsRequest.PartialClose1", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission PartialClose2CommonPartsrequest = new ApplicationPermission("اقفال ثانى", "commonPartsRequest.PartialClose2", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission NoteCommonPartsrequest = new ApplicationPermission("ملاحظات", "commonPartsRequest.Note", CommonPartsrequestsGroupName, "");
		public static ApplicationPermission ApproveCommonPartsrequest = new ApplicationPermission("تعميد", "commonPartsRequest.transfer2", CommonPartsrequestsGroupName, "");
		public static ApplicationPermission AcceptBaptismCommonPartsrequest = new ApplicationPermission("قبول تعميد", "commonPartsRequest.AcceptBaptism", CommonPartsrequestsGroupName, "");
		public static ApplicationPermission RefuseBaptismCommonPartsrequest = new ApplicationPermission("رفض تعميد", "commonPartsRequest.RefuseBaptism", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission DeleteCommonPartsrequest = new ApplicationPermission("حذف", "commonPartsRequest.Delete", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission UpCommonPartsrequest = new ApplicationPermission("تصعيد", "commonPartsRequest.Up", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission confirmCommonPartsrequest = new ApplicationPermission("اعتماد", "commonPartsRequest.Confirm", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission refuseCommonPartsrequest = new ApplicationPermission("رفض", "commonPartsRequest.Refuse", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission TechnicianCommonPartsrequest = new ApplicationPermission("طلبات الفنى", "commonPartsRequest.Technician", CommonPartsrequestsGroupName, "");
        public static ApplicationPermission ManagerCommonPartsrequest = new ApplicationPermission("طلبات المشرف", "commonPartsRequest.Manager", CommonPartsrequestsGroupName, "");
 
        #endregion

        #region KitchenServicesrequests 
        public const string KitchenServicesGroupName = "صلاحيات  خدمات المطابخ ";
        public static ApplicationPermission ViewAllKitchenServicesrequests = new ApplicationPermission("عرض جميع الطلبات", "KitchenServices.view", KitchenServicesGroupName, "");
        public static ApplicationPermission AddKitchenServicesrequest = new ApplicationPermission("اضافة", "KitchenServices.Add", KitchenServicesGroupName, "");
        public static ApplicationPermission ShowKitchenServicesrequest = new ApplicationPermission("عرض", "KitchenServices.Show", KitchenServicesGroupName, "");
        public static ApplicationPermission TransferKitchenServicesrequest = new ApplicationPermission("تحويل", "KitchenServices.transfer", KitchenServicesGroupName, "");
        public static ApplicationPermission TrackKitchenServicesrequest = new ApplicationPermission("متابعة", "KitchenServices.Track", KitchenServicesGroupName, "");
        public static ApplicationPermission StartKitchenServicesrequest = new ApplicationPermission("بدء", "KitchenServices.Start", KitchenServicesGroupName, "");
        public static ApplicationPermission RepleyKitchenServicesrequest = new ApplicationPermission("رد", "KitchenServices.Repley", KitchenServicesGroupName, "");
        public static ApplicationPermission CloseByCodeKitchenServicesrequest = new ApplicationPermission("اغلاق", "KitchenServices.CloseM1", KitchenServicesGroupName, "");
        public static ApplicationPermission DeleteKitchenServicesrequest = new ApplicationPermission("حذف", "KitchenServices.Delete", KitchenServicesGroupName, "");
        public static ApplicationPermission TechnicianKitchenServicesrequest = new ApplicationPermission("طلبات الفنى", "KitchenServices.Technician", KitchenServicesGroupName, "");
        public static ApplicationPermission ClientKitchenServicesrequest = new ApplicationPermission("طلبات العميل", "KitchenServices.Client", KitchenServicesGroupName, "");
        public static ApplicationPermission AgreemantKitchenServicesrequest = new ApplicationPermission("اضافة ملف الأتفاقية", "KitchenServices.Agreemant", KitchenServicesGroupName, "");
       // public static ApplicationPermission ProjectManagerKitchenServicesrequest = new ApplicationPermission("طلبات مدير مشروع", "KitchenServices.projectManager", KitchenServicesGroupName, "");

        #endregion

        #region Questionnaire 
        public const string QuestionnaireGroupName = "صلاحيات الأستبيانات ";
        //public static ApplicationPermission QuestionnairesManage = new ApplicationPermission("ادارة الأستبيانات", "Questionnaires.manage", QuestionnaireGroupName, "");
       // public static ApplicationPermission QuestionnairesProjectManager = new ApplicationPermission("استبيانات مدير مشروع", "Questionnaires.ProjectManager", QuestionnaireGroupName, "");
        public static ApplicationPermission QuestionnairesAddNote = new ApplicationPermission("اضافة ملاحظات", "Questionnaires.Add", QuestionnaireGroupName, "");
        public static ApplicationPermission QuestionnairesServiceView = new ApplicationPermission("عرض تقييمات الخدمات", "Questionnaires.ServicesView", QuestionnaireGroupName, "");
        public static ApplicationPermission QuestionnairesTechnicianView = new ApplicationPermission("عرض تقييمات الفنيين", "Questionnaires.TechniciansView", QuestionnaireGroupName, "");
        public static ApplicationPermission QuestionnairesTechnicianDelete = new ApplicationPermission("حذف تقييم", "Questionnaires.Delete", QuestionnaireGroupName, "");

        #endregion



        #region ComplaintsPermissions
        public const string ComplaintsGroupName = "صلاحيات الشكاوى ";
        public static ApplicationPermission ViewAllComplaints = new ApplicationPermission("عرض جميع الشكاوى", "Complaints.view", ComplaintsGroupName, "");
        public static ApplicationPermission AddComplaints = new ApplicationPermission("اضافة", "Complaints.Add", ComplaintsGroupName, "");
        public static ApplicationPermission ShowComplaints = new ApplicationPermission("عرض", "Complaints.Show", ComplaintsGroupName, "");
        public static ApplicationPermission TransferComplaints = new ApplicationPermission("تحويل", "Complaints.transfer", ComplaintsGroupName, "");
        public static ApplicationPermission TrackComplaints = new ApplicationPermission("متابعة", "Complaints.Track", ComplaintsGroupName, "");
        public static ApplicationPermission CloseComplaints = new ApplicationPermission("اغلاق", "Complaints.close", ComplaintsGroupName, "");
        public static ApplicationPermission PartialCloseComplaints = new ApplicationPermission("اقفال", "Complaints.PartialClose", ComplaintsGroupName, "");
        public static ApplicationPermission DeleteComplaint = new ApplicationPermission("حذف", "Complaints.Delete", ComplaintsGroupName, "");
        public static ApplicationPermission TechnicianComplaints = new ApplicationPermission("شكاوى الفنى", "Complaints.Technician", ComplaintsGroupName, "");
       // public static ApplicationPermission ProjectManagerComplaints = new ApplicationPermission("شكاوى مدير مشروع", "Complaints.ProjectManager", ComplaintsGroupName, "");

        #endregion

        #region ٍSettingPermissions
        public const string SettingGroupName = "صلاحيات الأعدادات";
        //public static ApplicationPermission ViewAllDaysOff = new ApplicationPermission("View all", "DaysOff.view", DaysOffGroupName, "");
        public static ApplicationPermission manageDaysOff = new ApplicationPermission("ادارة الأجازات", "DaysOff.manage", SettingGroupName, "");
        public static ApplicationPermission manageRulles = new ApplicationPermission("ادارة الشروط والأحكام", "Rules.manage", SettingGroupName, "");
        public static ApplicationPermission ReClientsReceiving = new ApplicationPermission("ادارة العملاء المستلمين للوحدات", "ClientsReceiving.manage", SettingGroupName, "");

        #endregion


        static ApplicationPermissions()
		{

			var allPermissions = Utils.DictionaryOfPropertiesFromClass(typeof(ApplicationPermissions), typeof(ApplicationPermission)).Select(x => (ApplicationPermission)x.Value)?.ToList();
			AllPermissions = allPermissions.AsReadOnly();
		}

		public static ApplicationPermission GetPermissionByName(string permissionName)
		{
			return AllPermissions.Where(p => p.Name == permissionName).SingleOrDefault();
		}

		public static ApplicationPermission GetPermissionByValue(string permissionValue)
		{
			return AllPermissions.FirstOrDefault(p => p.Value == permissionValue);
		}

		public static string[] GetAllPermissionValues()
		{
			return AllPermissions.Select(p => p.Value).ToArray();
		}

		public static string[] GetAdministrativePermissionValues()
		{
			return new string[] { ManageUsers, ManageRoles };
		}
		public static ApplicationRole DefaultClientRole = new ApplicationRole("عميل");

	}




    public class ApplicationPermission
    {
        public ApplicationPermission()
        { }

        public ApplicationPermission(string name, string value, string groupName, string description = null, bool IsChecked = false)
        {
            Name = name;
            Value = value;
            GroupName = groupName;
            Description = description;
            Checked = IsChecked;

        }



        public string Name { get; set; }
        public string Value { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool Checked {set;get;}

		public override string ToString()
		{
			return Value;
		}


		public static implicit operator string(ApplicationPermission permission)
		{
			return permission.Value;
		}
	}
}
