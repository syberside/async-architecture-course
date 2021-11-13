using aTES.TaskTracker.Domain;
using aTES.TaskTracker.Models;
using aTES.TaskTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aTES.TaskTracker.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TasksService _tasksService;

        public TasksController(TasksService tasksService)
        {
            _tasksService = tasksService;
        }

        public async Task<IActionResult> IndexAsync(bool onlyMyTasks = false)
        {
            var user = onlyMyTasks ? CurrentUserPublicId() : null;
            var allTasks = await _tasksService.ListAll(user);
            var tasks = allTasks.Select(x => new TaskViewModel
            {
                CanComplete = !x.IsCompeleted && x.AssigneePublicId == CurrentUserPublicId(),
                Task = x
            }).ToArray();
            var vm = new IndexViewModel()
            {
                Tasks = tasks,
                OnlyMyTasks = onlyMyTasks,
                CanAssignTasks = await CanUserReassignTasks()
            };
            return View(vm);
        }

        private string CurrentUserPublicId()
        {
            return User.Claims.First(x => x.Type == "sub").Value;
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(CreateTaskModel model)
        {
            if (!ModelState.IsValid)
            {

                return await IndexAsync();
            }

            await _tasksService.Create(model.Description);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AssignOpenTickets()
        {
            if (!await CanUserReassignTasks())
            {
                return Forbid();
            }
            await _tasksService.DistributeOpenTasks();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CompleteTask(CompleteTaskModel model)
        {
            var result = await _tasksService.CompleteTask(model.TaskId, CurrentUserPublicId());
            if (!result)
            {
                return Forbid();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CanUserReassignTasks()
        {
            var role = await _tasksService.GetRole(CurrentUserPublicId());
            return role == Roles.Admin || role == Roles.Manager;
            //TODO: swith to claim passed via auth request
            //var roleString = User.Claims.First(x => x.Type == "PopugRole")?.Value;
            //var role = Enum.Parse<Roles>(roleString);
            //return role == Roles.Admin || role == Roles.Manager;
        }
    }
}
