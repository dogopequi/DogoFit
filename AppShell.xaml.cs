using GymTracker.Services;
using GymTracker.Views;

namespace GymTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("createroutine", typeof(CreateRoutine));
            Routing.RegisterRoute("addexercise", typeof(AddExercise));
            Routing.RegisterRoute("setselection", typeof(SetSelection));
            Routing.RegisterRoute("startroutine", typeof(StartRoutine));
            Routing.RegisterRoute("editroutine", typeof(EditRoutine));
            Routing.RegisterRoute("settings", typeof(Settings));
            Routing.RegisterRoute("profileselectexercise", typeof(ProfileSelectExercise));
        }
    }
}