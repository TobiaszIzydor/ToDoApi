using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Application.Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        //All tasks here call to repository, because service is between Api and repository 
        public async Task AddToDo(Domain.Entities.ToDo newToDo)
        {
            await _toDoRepository.AddToDo(newToDo);
        }

        public async Task DeleteToDoById(int id)
        {
            await _toDoRepository.DeleteToDoById(id);
        }

        public async Task<IEnumerable<ToDo.Domain.Entities.ToDo>> GetAllToDos()
        {
            return await _toDoRepository.GetAllToDos();
        }

        public async Task<IEnumerable<Domain.Entities.ToDo>> GetIncomingToDos(DateTime? startDate, DateTime? endDate)
        {
            return await _toDoRepository.GetIncomingToDos(startDate, endDate);
        }

        public async Task<Domain.Entities.ToDo> GetToDoById(int id)
        {
            return await _toDoRepository.GetToDoById(id);
        }

        public async Task MarkAsDone(int id)
        {
            await _toDoRepository.MarkAsDone(id);
        }

        public async Task SetToDoPercentComplete(int id, int percent)
        {
            await _toDoRepository.SetToDoPercentComplete(id, percent);
        }

        public async Task UpdateToDo(Domain.Entities.ToDo updatedToDo)
        {

            await _toDoRepository.UpdateToDo(updatedToDo);
        }
    }
}
