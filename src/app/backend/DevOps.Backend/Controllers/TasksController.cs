using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOps.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskModel = DevOps.Backend.Models.Task;

namespace DevOps.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class TasksController : ControllerBase
    {
        private readonly TaskContext _context;

        public TasksController(TaskContext context)
        {
            this._context = context;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TaskModel>> DeleteTask(int id)
        {
            var task = await this._context.Tasks.FindAsync(id);
            if (task == null)
            {
                return this.NotFound();
            }

            this._context.Tasks.Remove(task);
            await this._context.SaveChangesAsync();

            return task;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTask(int id)
        {
            var task = await this._context.Tasks.FindAsync(id);

            if (task == null)
            {
                return this.NotFound();
            }

            return task;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTasks()
        {
            return await this._context.Tasks.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<TaskModel>> PostTask(TaskModel task)
        {
            this._context.Tasks.Add(task);
            await this._context.SaveChangesAsync();

            return this.CreatedAtAction("GetTask", new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTask(int id, TaskModel task)
        {
            if (id != task.Id)
            {
                return this.BadRequest();
            }

            this._context.Entry(task).State = EntityState.Modified;

            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!this.TaskExists(id))
                {
                    return this.NotFound();
                }
                else
                {
                    throw;
                }
            }

            return this.NoContent();
        }

        private bool TaskExists(int id)
        {
            return this._context.Tasks.Any(e => e.Id == id);
        }
    }
}