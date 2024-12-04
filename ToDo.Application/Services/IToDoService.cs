using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Application.Services
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDo.Domain.Entities.ToDo>> GetAllToDos();
        Task AddToDo(Domain.Entities.ToDo newToDo);
        Task<Domain.Entities.ToDo?> GetToDoById(int id);
        Task<IEnumerable<Domain.Entities.ToDo>> GetIncomingToDos(DateTime? startDate, DateTime? endDate);
        Task DeleteToDoById(int id);
        Task UpdateToDo(Domain.Entities.ToDo updatedToDo);
        Task MarkAsDone(int id);
        Task SetToDoPercentComplete(int id, int percent);
    }
}
