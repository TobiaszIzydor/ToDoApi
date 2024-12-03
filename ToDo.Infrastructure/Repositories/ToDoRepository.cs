using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Persistence;

namespace ToDo.Infrastructure.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoDbContext _dbContext;
        public ToDoRepository(ToDoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Domain.Entities.ToDo>> GetAllToDos() //Return all todo in list
        {
            return await _dbContext.ToDos.ToListAsync();
        }
        public async Task AddToDo(Domain.Entities.ToDo newToDo) //Add new todo
        {
            _dbContext.ToDos.Add(newToDo);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Domain.Entities.ToDo> GetToDoById(int id) //Return specific todo by id
        {
            return await _dbContext.ToDos.FirstAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Domain.Entities.ToDo>> GetIncomingToDos(DateTime? startDate, DateTime? endDate) //Return list of incoming todos
        {
            return await _dbContext.ToDos
                    .Where(todo => todo.ExpiryDate >= startDate && todo.ExpiryDate <= endDate)
                    .ToListAsync();
        }

        public async Task DeleteToDoById(int id) //Delete todo by id
        {
            _dbContext.ToDos.Remove(await GetToDoById(id));
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateToDo(Domain.Entities.ToDo updatedToDo) //Update todo
        {
            Domain.Entities.ToDo todo = await _dbContext.ToDos.FirstAsync(x => x.Id == updatedToDo.Id);
            //Apply changes to specific todo
            todo.ExpiryDate = updatedToDo.ExpiryDate;
            todo.Description = updatedToDo.Description;
            todo.Name = updatedToDo.Name;
            todo.PercentComplete = updatedToDo.PercentComplete;
            todo.IsCompleted = updatedToDo.IsCompleted;
            await _dbContext.SaveChangesAsync(); //Save in database
        }

        public async Task MarkAsDone(int id) //Mark todo as done
        {
            Domain.Entities.ToDo todo = await _dbContext.ToDos.FirstAsync(x =>x.Id == id);
            todo.IsCompleted = true;
            todo.PercentComplete = 100;
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetToDoPercentComplete(int id, int percent) //Set new completion percentage to todo
        {
            Domain.Entities.ToDo todo = await _dbContext.ToDos.FirstAsync(x => x.Id == id);
            todo.PercentComplete = percent;
            if (percent == 100)
            {
                todo.IsCompleted = true;
            }
            else
            {
                todo.IsCompleted = false;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
