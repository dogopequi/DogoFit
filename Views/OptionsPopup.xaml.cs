using CommunityToolkit.Maui.Views;
using System.Threading.Tasks;

namespace GymTracker.Popups;

public partial class OptionsPopup : Popup
{
    private TaskCompletionSource<string?> _taskCompletionSource;

    public OptionsPopup()
    {
        InitializeComponent();

        _taskCompletionSource = new TaskCompletionSource<string?>();

        BindingContext = this;
    }

    public Command EditCommand => new Command(() => SetResult("Edit"));
    public Command DeleteCommand => new Command(() => SetResult("Delete"));
    public Command CancelCommand => new Command(() => SetResult(null));

    private void SetResult(string? result)
    {
        _taskCompletionSource.TrySetResult(result);

        this.CloseAsync();
    }

    public Task<string?> WaitForResultAsync() => _taskCompletionSource.Task;
}
