using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;
using EtisiqueApi.Repositiories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EtisiqueApi.Repositiories
{
    public class RuleService : GenaricService<Rule>, IRuleService
    {
        Context _dbContext;
       public RuleService (Context dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public (bool Succeeded, string[] Errors) AddRules(string Rule, List<int> Projects)
        {
            try
            {
                List<Rule> rules = new List<Rule>();
                var currentDate = DateTime.UtcNow;
                //    // Increase the hour by 3
                //genetate Code
                Random generator = new Random();
                string UniqueNumber = generator.Next(0, 100).ToString("D2");


                var dateAfter3Hours = currentDate.AddHours(3);
                foreach (int ProjectId in Projects)
                {
                    rules.Add(new Rule()
                    {
                        ProjectId = ProjectId != 0 ? ProjectId : null,
                        Text = Rule,
                        LastUpdate = dateAfter3Hours,
                        TextId=int.Parse(UniqueNumber),

                    });


                }
                _dbContext.Rules.AddRange(rules);
                _dbContext.SaveChanges();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false,new string[] { ex.Message });

            }

        }
        public (bool Succeeded, string[] Errors) UpdateRules(int TextId,string Rule)
        {
            try
            {
               List<Rule> rules= _dbContext.Rules.Where(R => R.TextId == TextId).ToList();

                var currentDate = DateTime.UtcNow;
                var dateAfter3Hours = currentDate.AddHours(3);

                foreach (Rule rule in rules)
                {
                    rule.Text = Rule;
                    rule.LastUpdate =dateAfter3Hours;
                }
                _dbContext.Rules.UpdateRange(rules);
                _dbContext.SaveChanges();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });

            }

        }
        public (bool Succeeded, string[] Errors) Delete(int TextId)
        {
            try
            {
                List<Rule> rules = _dbContext.Rules.Where(R => R.TextId == TextId).ToList();
                _dbContext.Rules.RemoveRange(rules);
                _dbContext.SaveChanges();
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new string[] { ex.Message });

            }

        }
        public List<string> GetpublicRules()
        {
            return _dbContext.Rules.Where(R=>R.ProjectId == null).Select(R=>R.Text).ToList();
        }
        public async override Task<Rule> GetByIdAsync(int id)
        {
            return await _dbContext.Rules.Include(R=>R.Project).FirstOrDefaultAsync(R=>R.Id == id);
        }

        public List<string> GetRulesByProject(int ProjectId)
        {
            return _dbContext.Rules.Where(R => R.ProjectId == null || R.ProjectId == ProjectId).Select(R => R.Text).ToList();

        }
        public Rule getLastRule()
        {
            return _dbContext.Rules.LastOrDefault();
        }
        public List<RuleDto.RuleForShow> GetAllRules()
        {
           return  _dbContext.Rules.Select(n =>new RuleDto.RuleForShow(){
               id = n.Id,
               Text=n.Text,
              

            }).ToList();
            //var results = _dbContext.Rules.Include(R => R.Project).GroupBy(rule => rule.TextId);
            //foreach (var group in results)
            //{
               

            //    foreach (var rule in group)
            //    {
            //        Console.WriteLine($"Rule Id: {rule.RuleId}, Text: {rule.RuleText}, Project Id: {rule.ProjectId}");
            //    }

            //    Console.WriteLine(); // Add an empty line between groups for better readability
            //}


        }
    }
}
