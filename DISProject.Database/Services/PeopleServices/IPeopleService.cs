using DISProject.Database.Entities;

namespace DISProject.Database.Services.PeopleServices;

public interface IPeopleService
{
    Task<(bool IsSuccess, string ErrorMessage, List<People> PeopleList)> GetAllPeopleAsync();
}