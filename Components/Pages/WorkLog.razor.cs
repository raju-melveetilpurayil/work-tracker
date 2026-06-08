using Microsoft.AspNetCore.Components;
using WorkTracker.Services;

namespace WorkTracker.Components.Pages
{
    public partial class WorkLog
    {

        [Parameter] 
        public string UserId { get; set; } = string.Empty;

        private IReadOnlyList<DayLog>? _days;
        [Inject]
        private IConfiguration Configuration { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                UserId = Configuration["AppUser:UserId"] ?? string.Empty;
            // Last 7 days; adjust the range as you like.
            var to = DateTime.Today.AddDays(1);
            var from = to.AddDays(-7);
            _days = await workLogService.GetDailyLogAsync(UserId, from, to);

            
        }

        private static string Format(TimeSpan t) =>
            t.TotalHours >= 1 ? $"{(int)t.TotalHours}h {t.Minutes}m" : $"{t.Minutes}m";
    }
}
