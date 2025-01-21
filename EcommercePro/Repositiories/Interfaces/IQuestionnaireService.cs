using EtisiqueApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IQuestionnaireService
    {
        public Task<(bool Succeeded, string[] Errors)> Add(QuestionnaireDto questionnaireDto);
        public Task<(bool Succeeded, string[] Errors)> AddTechnicianQusetions(int Id ,string TechnicianId, List<int> Answers);
        public IQueryable<EvelautionServiceDto> GetEvelautionsService(int RequestCode = 0, int Week = 0, int Year = 0,
            int Month = 0, string ClientName = null, string ProjectName = null, int rating = 0);
        public IQueryable<EvelautionServiceDto> GetRequestToProjectsManager(List<int> projects, int RequestCode = 0, int Week = 0, int Year = 0,
            int Month = 0, string ClientName = null, string ProjectName = null, int rating = 0);
        public EvelautionTechnicianDto GetEvelautionTechnician(string TechnicianId = null, int Week = 0,
        int Year = 0, int Month = 0, DateOnly from = default, DateOnly to = default, string ProjectName = null);
        public EvelautionTechnicianDto GetEvelautionTechnicianToProjectManager(List<int>projects, string TechnicianId = null, int Week = 0,
        int Year = 0, int Month = 0, DateOnly from = default, DateOnly to = default, string ProjectName = null);
        public Task<(bool Succeeded, string[] Errors)> AddNote(int id, string Note);

        public bool IsRated(int requestId, int typeservice);
        public bool IsRequester(int requestId, int typeservice, string userId);
        public Task<(bool Succeeded, string[] Errors)> Delete(int id);

    }
}
