using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDo.Domain.Interfaces
{
    public interface IToDoRepository
    {
        Task<IEnumerable<Entities.ToDo>> GetAllToDos();
        Task AddToDo(Entities.ToDo newToDo);
        Task<Entities.ToDo> GetToDoById(int id);
        Task<IEnumerable<Domain.Entities.ToDo>> GetIncomingToDos(DateTime? startDate, DateTime? endDate);
        Task DeleteToDoById(int id);
        Task UpdateToDo(Entities.ToDo updatedToDo);
        Task MarkAsDone(int id);
        Task SetToDoPercentComplete(int id, int percent);
    }
}
