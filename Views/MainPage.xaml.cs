using GymTracker.Models;

namespace GymTracker
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new WorkoutHistoryModel();
        }
    }

}
