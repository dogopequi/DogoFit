using CommunityToolkit.Maui.Views;
namespace GymTracker.Popups;

public partial class ExercisePopup : Popup
{
	private TaskCompletionSource<string?> _taskCompletionSource;
	public ExercisePopup()
	{
		InitializeComponent();
		_taskCompletionSource = new TaskCompletionSource<string?>();

        BindingContext = this;
	}

	public Command AddCommand => new Command(() => SetResult("Add"));
    public Command EditDeleteCommand => new Command(() => SetResult("EditDelete"));
    public Command CancelCommand => new Command(() => SetResult(null));

	private void SetResult(string? result)
    {
        _taskCompletionSource.TrySetResult(result);

        this.CloseAsync();
    }

    public Task<string?> WaitForResultAsync() => _taskCompletionSource.Task;
}