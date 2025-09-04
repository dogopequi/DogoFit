using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Controls;
using GymTracker.Services;
namespace GymTracker
{
    public partial class App : Application
    {
        public static AppDbContext Db;
        public App()
        {
            InitializeComponent();
            DbHelper.DeleteDatabase("gymtracker.db");

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "gymtracker.db");
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            Db = new AppDbContext(options);
            Db.Database.EnsureCreated();
            AppState.Init();
            Application.Current.UserAppTheme = AppTheme.Dark;
            MainPage = new AppShell();
        }


        protected override void OnSleep()
        {
            AppState.SaveWorkoutInProgress();
        }
    }
}
