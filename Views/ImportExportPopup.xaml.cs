using CommunityToolkit.Maui.Views;
namespace GymTracker.Popups;

public partial class ImportExportPopup : Popup
{
    private TaskCompletionSource<string?> _taskCompletionSource;
	public ImportExportPopup()
	{
		InitializeComponent();
        _taskCompletionSource = new TaskCompletionSource<string?>();

        BindingContext = this;
	}

	public Command WorkoutsCommand => new Command(() => SetResult("Workouts"));
    public Command RoutinesCommand => new Command(() => SetResult("Routines"));
    public Command ExercisesCommand => new Command(() => SetResult("Exercises"));
    public Command AllCommand => new Command(() => SetResult("All"));
    public Command CancelCommand => new Command(() => SetResult(null));


	private void SetResult(string? result)
    {
        _taskCompletionSource.TrySetResult(result);

        this.CloseAsync();
    }

    public Task<string?> WaitForResultAsync() => _taskCompletionSource.Task;
}