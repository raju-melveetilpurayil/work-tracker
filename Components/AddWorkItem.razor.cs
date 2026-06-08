using Microsoft.AspNetCore.Components;
using WorkTracker.Models;
using WorkTracker.Services;

namespace WorkTracker.Components
{
    public partial class AddWorkItem
    {
        [Parameter] 
        public string UserId { get; set; } = string.Empty;

        [Parameter] 
        public EventCallback<WorkItem> OnSaved { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;
        [Inject]
        private IConfiguration Configuration { get; set; } = default!;

        private WorkItemFormModel _model = new();
        private bool _isSaving;
        private string? _errorMessage;
        private string? _savedMessage;

        private string GetDuration()
        {
            var span = _model.EndTime - _model.StartTime;
            return span.TotalHours >= 1
                ? $"{(int)span.TotalHours}h {span.Minutes}m"
                : $"{span.Minutes}m";
        }

        private async Task HandleValidSubmit()
        {
            _isSaving = true;
            _errorMessage = null;
            _savedMessage = null;

            try
            {
                var workItem = new WorkItem
                {
                    Title = _model.Title,
                    Description = _model.Description,
                    StartTime = _model.StartTime.ToUniversalTime(),
                    EndTime = _model.EndTime.ToUniversalTime(),
                    DateCreated = DateTime.UtcNow,   
                    UserId = UserId
                };
               
                await OnSaved.InvokeAsync(workItem);

                _savedMessage = "Work item saved successfully.";
                _model = new();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Could not save: {ex.Message}";
            }
            finally
            {
                _isSaving = false;
            }

            Navigation.NavigateTo("/worklog", forceLoad: true);

        }
        protected override void OnInitialized()
        {
            // Use the config value (falls back to whatever was passed in as a parameter)
            if (string.IsNullOrEmpty(UserId))
                UserId = Configuration["AppUser:UserId"] ?? string.Empty;
        }
        private void ResetForm()
        {
            _model = new();
            _errorMessage = null;
            _savedMessage = null;
        }
    }
}
