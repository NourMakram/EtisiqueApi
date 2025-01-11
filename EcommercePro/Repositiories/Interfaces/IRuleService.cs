using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface IRuleService:IGenaricService<Rule>
    {
        public List<String> GetpublicRules();
        public List<String> GetRulesByProject(int ProjectId);
        public (bool Succeeded, string[] Errors) AddRules(string Rule, List<int>Projects);
        public Rule getLastRule();
        public (bool Succeeded, string[] Errors) UpdateRules(int TextId, string Rule);
        public (bool Succeeded, string[] Errors) Delete(int TextId);


        public List<RuleDto.RuleForShow> GetAllRules();


    }
}
