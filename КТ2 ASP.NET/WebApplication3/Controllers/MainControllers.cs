using Microsoft.AspNetCore.Mvc;
using WebApplication3.Services;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class MainControllers(ILoggerService logger) : ControllerBase
    {
        public static List<TaskObj> TodoList = new List<TaskObj>() {};
        private ILoggerService _logger = logger;

        [HttpGet]
        public async Task<ActionResult<List<TaskObjShort>>> GetTasks()
        {
            _logger.Write("Запрос на получение списка задач");

            List<TaskObjShort> tasksShort = TodoList.Select(task => new TaskObjShort
            {
                Id = task.Id,
                Name = task.Name,
                isComplited = task.isComplited,
                Deadline = task.Deadline
            }).ToList();
            
            _logger.Write("Возвращено задач: " + tasksShort.Count);
            return Ok(tasksShort);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskObj>> GetTask(Guid id)
        {
            _logger.Write("Запрос на получение задачи с ID: " + id);

            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                _logger.Write("Задача не найдена: " + id);
                return NotFound("Задача не найдена");
            }
            
            _logger.Write("Задача найдена: " + task.Name);
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskObj>> CreateTask([FromBody] CreateTaskRequest request)
        {
            _logger.Write("Создание новой задачи: " + request.Name);

            TaskObj task = new TaskObj()
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                isComplited = false,
                Deadline = request.Deadline,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Description = request.Description
            };
            
            TodoList.Add(task);
            
            _logger.Write("Задача создана с ID: " + task.Id);
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskObj>> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
        {
            _logger.Write("Обновление задачи с ID: " + id);

            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                _logger.Write("Задача не найдена для обновления: " + id);
                return NotFound("Задача не найдена");
            }
            
            task.Name = request.Name;
            task.Description = request.Description;
            task.Deadline = request.Deadline;
            task.UpdatedAt = DateTime.UtcNow;
            
            _logger.Write("Задача обновлена: " + task.Name);
            return Ok(task);
        }

        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<TaskObj>> CompleteTask(Guid id)
        {
            _logger.Write("Пометка задачи как выполненной: " + id);

            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                _logger.Write("Задача не найдена для завершения: " + id);
                return NotFound("Задача не найдена");
            }
            
            task.isComplited = true;
            task.UpdatedAt = DateTime.UtcNow;
            
            _logger.Write("Задача завершена: " + task.Name);
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            _logger.Write("Удаление задачи: " + id);

            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                _logger.Write("Задача не найдена для удаления: " + id);
                return NotFound("Задача не найдена");
            }
            
            TodoList.Remove(task);
            
            _logger.Write("Задача удалена: " + task.Name);
            return Ok(task);
        }
    }

    public class TaskObj
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool isComplited { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class TaskObjShort
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool isComplited { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class CreateTaskRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateTaskRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}