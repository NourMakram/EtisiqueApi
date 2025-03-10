using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static EcommercePro.Models.Constants;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EtisiqueApi.Repositiories
{
    
    public class QuestionnaireService :GenaricService<Questionnaire> , IQuestionnaireService
    {
        private Context _Context;
        IAcountService _acountService;
        public QuestionnaireService(Context context, IAcountService
             acountService):base(context) { 
        
            _Context = context;
            _acountService = acountService;
        
        }
        public async Task<(bool Succeeded, string[] Errors)> Add(QuestionnaireDto questionnaireDto)
        {
            try
            {
                var trans = await BeginTransactionAsync();

                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);

                Questionnaire questionnaire = new Questionnaire()
                {
                    TechnicianId =questionnaireDto.TechnicianId,
                    TechnicianNote=questionnaireDto.TechnicianNote,
                    CustomerId=questionnaireDto.CustomerId,
                    CreatedDate =DateOnly.FromDateTime(dateAfter3Hours),
                    ServiceType = questionnaireDto.ServiceType,
                    RequestCode =questionnaireDto.RequestCode,
                    Rating=questionnaireDto.Rating,
                    ServiceNote=questionnaireDto.ServiceNote,
                    RequestId=questionnaireDto.RequestId

                };
             var result =await AddAsync(questionnaire);
                if (!result.Succeeded)
                {
                    return (false, result.Errors);
                }
                var result1 =await AddTechnicianQusetions(questionnaire.Id, questionnaire.TechnicianId, questionnaireDto.Answers);
                if (!result1.Succeeded)
                {
                    Rollback(trans);
                    return (false, result1.Errors);
                }
                Commit(trans);
                return (true, null);

            }
            catch (Exception ex)
            {

                return (false, null);

            }
        }

        public async Task<(bool Succeeded, string[] Errors)> AddNote(int id, string Note)
        {
            try
            {
               Questionnaire questionnaire =await GetByIdAsync(id);
                if (questionnaire == null)
                {
                    return (false, new string[] { "questionnaire Not Found" });
                }
                questionnaire.ManagerNote = Note;
               var result = Update(questionnaire);
                if (!result.Succeeded)
                {
                    return (false, new string[] { " Faild to Updated the questionnaire" });

                }
                return (true, null);


            }catch(Exception ex)
            {
                return (false,new string[] { ex.Message });

            }
        }

        public async Task<(bool Succeeded, string[] Errors)> AddTechnicianQusetions(int Id, string TechnicianId, List<int> Answers)
        {
            try
            {
                var currentDate = DateTime.UtcNow;

                // Increase the hour by 3
                var dateAfter3Hours = currentDate.AddHours(3);
                List<TechnicianQusetions> technicianQusetions = new List<TechnicianQusetions>();
                for (int i = 0; i < Answers.Count(); i++)
                {
                    technicianQusetions.Add(new TechnicianQusetions()
                    {
                        TechnicianId = TechnicianId,
                        questionId = i + 1,
                        questionnaireId = Id,
                        CreatedDate= DateOnly.FromDateTime(dateAfter3Hours),
                        AnswerId = Answers[i]

                    });

                }
                _Context.TechnicianQusetions.AddRange(technicianQusetions);
                _Context.SaveChanges();
                return (true, null);

            }
            catch (Exception ex)
            {
                return (false, null);

            }
        }

        public IQueryable<EvelautionServiceDto> GetEvelautionsService(int RequestCode = 0, int Week = 0, int Year = 0, 
            int Month = 0, string ClientName = null, string ProjectName=null, int rating=0)
        {
            var query = _Context.Questionnaires
                .Include(Q => Q.Technician)
                .Include(Q => Q.Customer)
                .ThenInclude(Q=>Q.Project)
                .OrderByDescending(Q => Q.Id).AsQueryable();
            if(RequestCode > 0)
            {
                query = query.Where(Q => Q.RequestCode.ToString().Contains(RequestCode.ToString()));
            }
           else if (ProjectName != null)
            {
                query = query.Where(Q => Q.Customer.Project.ProjectName.Contains(ProjectName));
            }
            else if(Week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                query = query.Where(request => request.CreatedDate >= startOfWeek && request.CreatedDate < endOfWeek);
 
            }
            else if (Year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                query = query.Where(Q => Q.CreatedDate.Year == CurrentYear);

            }
            else if (Month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;

                query = query.Where(Q => Q.CreatedDate.Month == CurrentMonth);

            }
            
            else if (ClientName!=null)
            {

                query = query.Where(Q => Q.Customer.FullName.Contains(ClientName));

            }
            if (rating != 0)
            {
                query = query.Where(Q => Q.Rating == rating);
            }
            var  query1 = query.Select(Q => new EvelautionServiceDto()
            {
                Id=Q.Id,
                CustomerName =Q.Customer.FullName,
                CustomerPhone=Q.Customer.PhoneNumber,
                TechnicianName=Q.Technician.FullName,
                Rating =Q.Rating,
                RequestCode=Q.RequestCode,
                ServiceNote =Q.ServiceNote,
                ServiceType=Q.ServiceType,
                date=Q.CreatedDate.ToString("dd-MM-yyyy"),
               //ServiceType=Constants.ServiceTypes.Where(T=>T.Id == Q.ServiceType).FirstOrDefault().Name,
                TechnicianNote=Q.TechnicianNote,
                ProjectName=Q.Customer.Project.ProjectName,
                ManagerNote=Q.ManagerNote,
                RequestId=Q.RequestId,
                

            }).AsQueryable();
            return query1;
         }

        public EvelautionTechnicianDto GetEvelautionTechnician(string TechnicianId = null, int Week = 0,
            int Year = 0, int Month = 0, DateOnly from=default ,DateOnly to=default,string ProjectName=null)
        {
            var query = _Context.TechnicianQusetions
                .Include(R=>R.questionnaire)
                .ThenInclude(R=>R.Customer).
                ThenInclude(R=>R.Project)
                .AsQueryable();
            int Count = _Context.Questionnaires.Count();
            if (TechnicianId != null)
            {
                query = query.Where(T => T.TechnicianId == TechnicianId).AsQueryable();
            }
            if (ProjectName != null)
            {
                query = query.Where(T => T.questionnaire.Customer.Project.ProjectName.Contains(ProjectName)).AsQueryable();
            }

            if (Week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                query = query.Where(request => request.CreatedDate >= startOfWeek && request.CreatedDate < endOfWeek);

            }
            else if (Year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                query = query.Where(Q => Q.CreatedDate.Year == CurrentYear);

            }
            else if (Month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;

                query = query.Where(Q => Q.CreatedDate.Month == CurrentMonth);

            }
            if (from != default)
            {
                query = query.Where(Q => Q.CreatedDate >= from);

            }
            if(to != default)
            {
                query = query.Where(Q => Q.CreatedDate <= to && Q.CreatedDate >= from);

            }
            List<EvelautionTechnicianAVgDto> AvarageRating = new List<EvelautionTechnicianAVgDto>();
            for(int i = 0; i < 4; i++)
            {
                var question1Ratings = query.Where(Q => Q.questionId == (i+1)).ToList();
                // 100, 80  ,حساب عدد الأشخاص الذين حصلوا على 20 و40
                int count20 = question1Ratings.Count(r => r.AnswerId == 20);
                int count40 = question1Ratings.Count(r => r.AnswerId == 40);
                int count80 = question1Ratings.Count(r => r.AnswerId == 80);
                int count100 = question1Ratings.Count(r => r.AnswerId == 100);

                // حساب المتوسط
                double average20 = count20 > 0 ? (double)count20 / question1Ratings.Count * 100 : 0;
                double average40 = count40 > 0 ? (double)count40 / question1Ratings.Count * 100 : 0;
                double average80 = count80 > 0 ? (double)count80 / question1Ratings.Count * 100 : 0;
                double average100 = count100 > 0 ? (double)count100 / question1Ratings.Count * 100 : 0;

                AvarageRating.Add(new EvelautionTechnicianAVgDto()
                {
                    question= ConsQuesteion.Questeions.FirstOrDefault(q=>q.Id == (i+1))?.Name,
                    average20 = average20,
                    average40 = average40,
                    average80 = average80,
                    average100=average100

                });
            }

            return  new EvelautionTechnicianDto() { AvarageRating = AvarageRating , Count = Count };
        }
        public EvelautionTechnicianDto GetEvelautionTechnicianToProjectManager(List<int> projects,string TechnicianId = null, int Week = 0,
            int Year = 0, int Month = 0, DateOnly from = default, DateOnly to = default, string ProjectName = null)
        {
            //List<int> projects = _acountService.GetUserProjects(userId);

            var query = _Context.TechnicianQusetions
                .Include(R => R.questionnaire)
                .ThenInclude(R => R.Customer).
                ThenInclude(R => R.Project)
               .Where(R => projects.Any(userProject => userProject == R.questionnaire.Customer.projectId))
               .AsQueryable();
            int Count = _Context.Questionnaires.Count();
            if (TechnicianId != null)
            {
                query = query.Where(T => T.TechnicianId == TechnicianId).AsQueryable();
            }
            if (ProjectName != null)
            {
                query = query.Where(T => T.questionnaire.Customer.Project.ProjectName.Contains(ProjectName)).AsQueryable();
            }

            if (Week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                query = query.Where(request => request.CreatedDate >= startOfWeek && request.CreatedDate < endOfWeek);

            }
            else if (Year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                query = query.Where(Q => Q.CreatedDate.Year == CurrentYear);

            }
            else if (Month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;

                query = query.Where(Q => Q.CreatedDate.Month == CurrentMonth);

            }
            if (from != default)
            {
                query = query.Where(Q => Q.CreatedDate >= from);

            }
            if (to != default)
            {
                query = query.Where(Q => Q.CreatedDate <= to && Q.CreatedDate >= from);

            }
            List<EvelautionTechnicianAVgDto> AvarageRating = new List<EvelautionTechnicianAVgDto>();
            for (int i = 0; i < 5; i++)
            {
                var question1Ratings = query.Where(Q => Q.questionId == (i + 1)).ToList();
                // 100, 80  ,حساب عدد الأشخاص الذين حصلوا على 20 و40
                int count20 = question1Ratings.Count(r => r.AnswerId == 20);
                int count40 = question1Ratings.Count(r => r.AnswerId == 40);
                int count80 = question1Ratings.Count(r => r.AnswerId == 80);
                int count100 = question1Ratings.Count(r => r.AnswerId == 100);

                // حساب المتوسط
                double average20 = count20 > 0 ? (double)count20 / question1Ratings.Count * 100 : 0;
                double average40 = count40 > 0 ? (double)count40 / question1Ratings.Count * 100 : 0;
                double average80 = count80 > 0 ? (double)count80 / question1Ratings.Count * 100 : 0;
                double average100 = count100 > 0 ? (double)count100 / question1Ratings.Count * 100 : 0;

                AvarageRating.Add(new EvelautionTechnicianAVgDto()
                {
                    question = ConsQuesteion.Questeions.FirstOrDefault(q => q.Id == (i + 1))?.Name,
                    average20 = average20,
                    average40 = average40,
                    average80 = average80,
                    average100 = average100

                });
            }

            return new EvelautionTechnicianDto() { AvarageRating = AvarageRating, Count = Count };
        }
        public IQueryable<EvelautionServiceDto> GetRequestToProjectsManager(List<int> projects, int RequestCode = 0, int Week = 0, int Year = 0,
            int Month = 0, string ClientName = null, string ProjectName = null, int rating = 0)
        {
           // List<int> projects = _acountService.GetUserProjects(UserId);


            var query = _Context.Questionnaires.AsNoTracking()
                .Include(Q => Q.Technician)
                .Include(Q => Q.Customer)
                .ThenInclude(Q => Q.Project)
                .Where(R => projects.Any(userProject => userProject == R.Customer.Project.Id));

            if (RequestCode > 0)
            {
                query = query.Where(Q => Q.RequestCode.ToString().Contains(RequestCode.ToString()));
            }
            else if (ProjectName != null)
            {
                query = query.Where(Q => Q.Customer.Project.ProjectName.Contains(ProjectName));
            }
            else if (Week > 0)
            {
                // تحديد بداية ونهاية الأسبوع الحالي
                DateOnly startOfWeek = DateOnly.FromDateTime(DateTime.Now.StartOfWeek(DayOfWeek.Sunday));
                DateOnly endOfWeek = startOfWeek.AddDays(7);

                // فلترة الطلبات خلال هذا الأسبوع
                query = query.Where(request => request.CreatedDate >= startOfWeek && request.CreatedDate < endOfWeek);

            }
            else if (Year > 0)
            {
                int CurrentYear = DateTime.Now.Year;

                query = query.Where(Q => Q.CreatedDate.Year == CurrentYear);

            }
            else if (Month > 0)
            {
                int CurrentMonth = DateTime.Now.Month;

                query = query.Where(Q => Q.CreatedDate.Month == CurrentMonth);

            }

            else if (ClientName != null)
            {

                query = query.Where(Q => Q.Customer.FullName.Contains(ClientName));

            }
             if (rating != 0)
            {

                query = query.Where(Q => Q.Rating==rating);

            }
            var query1 = query.Select(Q => new EvelautionServiceDto()
            {
                Id = Q.Id,
                CustomerName = Q.Customer.FullName,
                CustomerPhone = Q.Customer.PhoneNumber,
                TechnicianName = Q.Technician.FullName,
                Rating = Q.Rating,
                RequestCode = Q.RequestCode,
                ServiceNote = Q.ServiceNote,
                ServiceType = Q.ServiceType,
                date = Q.CreatedDate.ToString("dd-MM-yyyy"),
                //ServiceType=Constants.ServiceTypes.Where(T=>T.Id == Q.ServiceType).FirstOrDefault().Name,
                TechnicianNote = Q.TechnicianNote,
                ManagerNote = Q.ManagerNote,
                RequestId = Q.RequestId

            }).AsQueryable();
            return query1;

        }


        public bool IsRated(int requestId, int typeservice)
        {
            return _Context.Questionnaires.Any(q => q.RequestId == requestId && q.ServiceType == typeservice);
        }

        public bool IsRequester(int requestId, int typeservice, string userId)
        {
            var result = false;
            if(typeservice == (int)ServiceTypeEnum.ApartmentService)
            {
             result= _Context.ApartmentServicesRequests.Include(A=>A.Customer).Any(A=>A.id== requestId && A.Customer.UserId== userId); 
            }
            else if (typeservice == (int)ServiceTypeEnum.KitchenService)
            {
                result = _Context.KitchenServices.Include(A => A.Customer).Any(A => A.id == requestId && A.Customer.UserId == userId);

            }
            return result;
        }

        public async Task<(bool Succeeded, string[] Errors)> Delete(int id)
        {
            try
            {
                Questionnaire questionnaire = await GetByIdAsync(id);

                if (questionnaire != null)
                {
                   IQueryable<TechnicianQusetions> technicianQusetions = _Context.TechnicianQusetions.Where(T => T.questionnaireId == questionnaire.Id);
                    if (technicianQusetions != null)
                    {
                        _Context.TechnicianQusetions.RemoveRange(technicianQusetions);
                        //await _Context.SaveChangesAsync();

                    }
                    _Context.Questionnaires.Remove(questionnaire);
                    await _Context.SaveChangesAsync();
                    return (true, null);
                }
                return (false, new string[] { "can`t delete this Reuest" });

            }
            catch (Exception ex)
            {
                return (false, new string[] { "can`t delete this Reuest" });
            }

        }
        public EvelautionDetails GetEvelautionDetails(int id)
        {
            EvelautionDetails evelautionDetails = new EvelautionDetails();

           Questionnaire questionnairedb = _Context.Questionnaires.FirstOrDefault(Q => Q.Id == id);
            if (questionnairedb == null)
            {
                return null;
            }
            evelautionDetails.managerNote = questionnairedb.ManagerNote;
            evelautionDetails.ServiceNote = questionnairedb.ServiceNote;
            evelautionDetails.TechincalNote = questionnairedb.TechnicianNote;
            evelautionDetails.Ratings = new List<Rating>();
            var results  = _Context.TechnicianQusetions
               .Where(R => R.questionnaireId == id)
               .Select(Q => new
               {
                   questionid =Q.questionId,
                   answer = Q.AnswerId,
               }).ToList();
            foreach (var result in results)
            {
                var question = ConsQuesteion.Questeions.FirstOrDefault(q => q.Id == result.questionid);
                if (question != null) // تأكد من أن السؤال غير null
                {
                    evelautionDetails.Ratings.Add(new Rating()
                    {
                        question = question.Name,
                        answer = result.answer,
                    });
                }
            }


            return evelautionDetails;

              
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime date, DayOfWeek startOfWeek)
        {
            int diff = date.DayOfWeek - startOfWeek;
            if (diff < 0) diff += 7;
            return date.AddDays(-diff).Date;
        }
    }
}
