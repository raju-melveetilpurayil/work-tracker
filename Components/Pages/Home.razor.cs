using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WorkTracker.Models;
using WorkTracker.Services;

namespace WorkTracker.Components.Pages
{
    public partial class Home
    {
        public string UserId { get; set; } = string.Empty;

        [Inject]
        private IConfiguration Configuration { get; set; } = default!;
        [Inject]
        private WorkLogService workLogService { get; set; } = default!;
        public async Task SaveWorkItem(WorkItem workItem)
        {
            workItem.Id = await workLogService.AddAsync(workItem);

        }

        
    }
}
