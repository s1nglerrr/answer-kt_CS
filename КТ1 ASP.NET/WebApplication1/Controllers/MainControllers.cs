using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class MainControllers : ControllerBase
    {
        public static List<TaskObj> TodoList = new List<TaskObj>() {};

        [HttpGet]
        public async Task<ActionResult<List<TaskObjShort>>> GetTasks()
        {
            List<TaskObjShort> tasksShort = TodoList.Select(task => new TaskObjShort
            {
                Id = task.Id,
                Name = task.Name,
                isComplited = task.isComplited,
                Deadline = task.Deadline
            }).ToList();
            
            return Ok(tasksShort);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskObj>> GetTask(Guid id)
        {
            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound("Задача не найдена.");
            }
            
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<TaskObj>> CreateTask([FromBody] CreateTaskRequest request)
        {
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
            
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskObj>> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
        {
            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound("Задача не найдена.");
            }
            
            task.Name = request.Name;
            task.Description = request.Description;
            task.Deadline = request.Deadline;
            task.UpdatedAt = DateTime.UtcNow;
            
            return Ok(task);
        }

        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<TaskObj>> CompleteTask(Guid id)
        {
            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound("Задача не найдена.");
            }
            
            task.isComplited = true;
            task.UpdatedAt = DateTime.UtcNow;
            
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            TaskObj? task = TodoList.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound("Задача не найдена.");
            }
            
            TodoList.Remove(task);
            
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