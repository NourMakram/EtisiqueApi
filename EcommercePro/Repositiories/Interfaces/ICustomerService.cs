﻿using EcommercePro.Models;
using EtisiqueApi.DTO;
using EtisiqueApi.Models;

namespace EtisiqueApi.Repositiories.Interfaces
{
    public interface ICustomerService :IGenaricService<Customer>
    {
        public Task<(bool Succeeded, string[] Errors)> Update(string id, RegisterDto Entirty);
        public Customer GetCustomerByUserID(string userId);

        public List<unit> GetUnitsByUserId(string userId);

        public Task<(bool Succeeded, string[] Errors)> SendRecervationCode (int id);

        public Task<(bool Succeeded, string[] Errors)> ConfirmReservation(int id, string Code);

         public   Task<Client> GetUnitDetails(int UnitId);

        public bool IsHajarPRojct(int unitId);

        public List<unit> UnitsWithGuarantee(string userId);

        public IQueryable<Client> ClientsReservation(string projectName = null, string buildingName = null, string ClientName = null, string ClientPhone = null, DateOnly from = default, DateOnly to = default);
        public IQueryable<Client> UnitsManagement(int projectId, string buidingName, string ClientName,
            string ClientPhone, DateOnly from, DateOnly to);
        public bool IsGuaranteeAvailable(string userId, int projectId, int unitNo, string buidingName);
        public bool IsGuaranteeAvailable(int UnitId);

        public Task<(bool Succeeded, string[] Errors)> AddUnit(NewUnit newUnit);
        public Task<(bool Succeeded, string[] Errors)> UnpdateUnit(NewUnit newUnit);

        public Task<(bool Succeeded, string[] Errors)> UpdateUnitsManagement(UnitsManagmentDto unitsManagment);
        public bool delete(int Id);


    }
}
