using Moq;
using ToDoApi.Controllers;
using ToDo.Application.Services;
using ToDo.Domain.Entities;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoApi.Controllers.TiToDoApi.Controllers;
using ToDo.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using ToDo.Application.Validators;

public class ToDoControllerTests
{
    private readonly Mock<IToDoRepository> _toDoRepositoryMock;
    private readonly Mock<IToDoService> _toDoServiceMock;
    private readonly ToDoController _controller;

    public ToDoControllerTests()
    {
        _toDoRepositoryMock = new Mock<IToDoRepository>();
        _toDoServiceMock = new Mock<IToDoService>();
        _controller = new ToDoController(_toDoServiceMock.Object);
    }

    [Fact]
    public async Task GetAllToDos_ReturnsTodos()
    {
        // Arrange
        IEnumerable<ToDo.Domain.Entities.ToDo> todos = new[] //Adding new todos
       {
            new ToDo.Domain.Entities.ToDo { Id = 1, Name = "Test Todo 1", IsCompleted = false, PercentComplete = 20 },
            new ToDo.Domain.Entities.ToDo { Id = 2, Name = "Test Todo 2", IsCompleted = true, PercentComplete = 100},
            new ToDo.Domain.Entities.ToDo { Id = 3, Name = "Test Todo 3", IsCompleted = false, PercentComplete = 20 }
        };
        _toDoServiceMock.Setup(service => service.GetAllToDos()).ReturnsAsync(todos); // Sets up the mock service to return a predefined list of todos when GetAllToDos is called.

        // Act
        var result = await _controller.GetAllToDos(); //Get todos from controller

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result); //Asserts that the result is an OkObjectResult, indicating a 200 OK response with data.
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ToDo.Domain.Entities.ToDo>>(actionResult.Value); //Asserts that the result contains an IEnumerable of ToDo entities.
        Assert.Equal(3, returnValue.Count()); //Asserts that the returned collection contains exactly 3 todos.
    }

    [Fact]
    public async Task CreateToDo_ReturnsOk_WhenValidTodoProvided()
    {
        // Arrange
        var newToDo = new ToDo.Domain.Entities.ToDo { Name = "New Todo", IsCompleted = false };
        _toDoServiceMock.Setup(service => service.AddToDo(It.IsAny<ToDo.Domain.Entities.ToDo>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateToDo(newToDo);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ToDo.Domain.Entities.ToDo>> (result);
        Assert.IsType<OkResult>(actionResult.Result);
    }
    [Fact]
    public async Task CreateToDo_ReturnsBadRequest_WhenInvalidTodoProvided()
    {
        // Arrange
        var invalidTodo = new ToDo.Domain.Entities.ToDo
        {
            Id = 2, // Invalid for creation (should be empty)
            Name = "Test Todo",
            ExpiryDate = DateTime.UtcNow.AddDays(-1), // Invalid expiry date
            PercentComplete = 50,
            IsCompleted = true // Invalid: cannot be completed with percent < 100
        };

        // Mock HTTP Context for validation
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Method = HttpMethods.Post;

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Validator
        var validator = new ToDoValidator(httpContextAccessorMock.Object);
        var validationResult = await validator.ValidateAsync(invalidTodo);

        // Creates a new instance of the ToDoController and sets up the HttpContext for the controller to simulate an HTTP request.
        var _controller = new ToDoController(_toDoServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        // Act
        _controller.ModelState.Clear();
        foreach (var error in validationResult.Errors)
        {
            _controller.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        Assert.False(_controller.ModelState.IsValid); //Asserts that the ModelState is not valid.

        var result = await _controller.CreateToDo(invalidTodo);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var errors = Assert.IsType<SerializableError>(badRequestResult.Value);

        //Asserts that the ModelState contains errors for specific properties of the invalidToDo object.
        Assert.Contains(errors.Keys, key => key == nameof(invalidTodo.Id));
        Assert.Contains(errors.Keys, key => key == nameof(invalidTodo.ExpiryDate));
        Assert.Contains(errors.Keys, key => key == nameof(invalidTodo.IsCompleted));
    }

    [Fact]
    public async Task GetToDoById_ReturnsTodo_WhenExists()
    {
        // Arrange
        var todoId = 1;
        var todo = new ToDo.Domain.Entities.ToDo { Id = todoId, Name = "Test Todo", IsCompleted = false };
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync(todo);

        // Act
        var result = await _controller.GetToDoById(todoId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(todo, actionResult.Value);
    }

    [Fact]
    public async Task GetToDoById_ReturnsNotFound_WhenNotExists()
    {
        // Arrange
        var todoId = 1;
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync((ToDo.Domain.Entities.ToDo?)null);


        // Act
        var result = await _controller.GetToDoById(todoId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal($"Task with ID {todoId} not found.", actionResult.Value);
    }

    [Fact]
    public async Task GetIncomingToDos_ReturnsFilteredTodos()
    {
        // Arrange
        var filter = "today";
        DateTime endDate = DateTime.Today.AddDays(1).AddTicks(-1);
        var incomingTodos = new List<ToDo.Domain.Entities.ToDo>
        {
            new ToDo.Domain.Entities.ToDo { Id = 1, Name = "Incoming Todo", IsCompleted = false, ExpiryDate = DateTime.UtcNow }
        };
        _toDoServiceMock.Setup(service => service.GetIncomingToDos(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(incomingTodos);


        // Act
        var result = await _controller.GetIncomingToDos(filter);

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<ToDo.Domain.Entities.ToDo>>>(result);
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<ToDo.Domain.Entities.ToDo>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task DeleteToDo_ReturnsOk_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var todo = new ToDo.Domain.Entities.ToDo { Id = todoId, Name = "Test Todo", IsCompleted = false };
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync(todo);
        _toDoServiceMock.Setup(service => service.DeleteToDoById(todoId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteToDo(todoId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteToDo_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = 1;
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync((ToDo.Domain.Entities.ToDo?)null);

        // Act
        var result = await _controller.DeleteToDo(todoId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Task with ID {todoId} not found.", actionResult.Value);
    }

    [Fact]
    public async Task UpdateToDo_ReturnsOk_WhenTodoIsUpdated()
    {
        // Arrange
        var todoId = 1;
        var updatedToDo = new ToDo.Domain.Entities.ToDo { Id = todoId, Name = "Updated Todo", IsCompleted = true };
        var todo = new ToDo.Domain.Entities.ToDo { Id = todoId, Name = "Test Todo", IsCompleted = false };
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync(todo);
        _toDoServiceMock.Setup(service => service.UpdateToDo(updatedToDo)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateToDo(todoId, updatedToDo);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateToDo_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = 1;
        var updatedToDo = new ToDo.Domain.Entities.ToDo { Id = todoId, Name = "Updated Todo", IsCompleted = true };
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync((ToDo.Domain.Entities.ToDo?)null);

        // Act
        var result = await _controller.UpdateToDo(todoId, updatedToDo);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Task with ID {todoId} not found.", actionResult.Value);
    }

    [Fact]
    public async Task MarkToDoAsDone_ReturnsOk_WhenTodoExists()
    {
        // Arrange
        var todoId = 1;
        var todo = new ToDo.Domain.Entities.ToDo { Id = todoId, Name = "Test Todo", IsCompleted = false };
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync(todo);
        _toDoServiceMock.Setup(service => service.MarkAsDone(todoId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.MarkToDoAsDone(todoId);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task MarkToDoAsDone_ReturnsNotFound_WhenTodoDoesNotExist()
    {
        // Arrange
        var todoId = 1;
        _toDoServiceMock.Setup(service => service.GetToDoById(todoId)).ReturnsAsync(null as ToDo.Domain.Entities.ToDo);

        // Act
        var result = await _controller.MarkToDoAsDone(todoId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Task with ID {todoId} not found.", actionResult.Value);
    }
}
