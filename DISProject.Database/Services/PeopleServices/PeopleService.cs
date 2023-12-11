using DISProject.Database.DatabaseContext;
using DISProject.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DISProject.Database.Services.PeopleServices;

public class PeopleService : IPeopleService
{
    private readonly DISProjectContext _context;
    public PeopleService(DISProjectContext context) => _context = context;
    
    public async Task<(bool IsSuccess, string ErrorMessage, List<People> PeopleList)> GetAllPeopleAsync()
    {
        try
        {
            var people = await _context.People
                .OrderBy(p => p.Id)
                .ToListAsync();
            if (people.Count == 0)
                return (false, "People List was empty", new List<People>());

            return (true, string.Empty, people);
        }
        catch (Exception e)
        {
            return (false, e.Message, new List<People>());
        }
    }
}
