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
            Routing.RegisterRoute("profileexercisestats", typeof(ProfileExerciseStats));
            Routing.RegisterRoute("profilemorestatistics", typeof(ProfileMoreStatistics));
            Routing.RegisterRoute("profilemusclegroup", typeof(ProfileMuscleGroup));
            Routing.RegisterRoute("profilemusclefunction", typeof(ProfileMuscleFunction));
            Routing.RegisterRoute("profileindimuscle", typeof(profileindimuscles));
            Routing.RegisterRoute("profilemonthlyreport", typeof(profilemonthlyreport));
            Routing.RegisterRoute("profileaddexercise", typeof(ProfileAddExercise));
            Routing.RegisterRoute("profileeditdeleteexercises", typeof(ProfileEditDeleteExercises));
        }
    }
}