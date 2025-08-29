using CommunityToolkit.Maui.Views;
namespace GymTracker.Popups;
public partial class SetOptionsPopup : Popup
{
    private TaskCompletionSource<string?> _taskCompletionSource;
    public SetOptionsPopup()
	{
		InitializeComponent();

        _taskCompletionSource = new TaskCompletionSource<string?>();

        BindingContext = this;
    }

    public Command NormalCommand => new Command(() => SetResult("Normal"));
    public Command RemoveCommand => new Command(() => SetResult("Remove"));
    public Command FailureCommand => new Command(() => SetResult("Failure"));
    public Command DropCommand => new Command(() => SetResult("Drop"));
    public Command WarmupCommand => new Command(() => SetResult("Warmup"));
    public Command CancelCommand => new Command(() => SetResult(null));

    private void SetResult(string? result)
    {
        _taskCompletionSource.TrySetResult(result);

        this.CloseAsync();
    }

    public Task<string?> WaitForResultAsync() => _taskCompletionSource.Task;
}