using GymTracker.Models;
using GymTracker.Services;
namespace GymTracker
{
    public partial class MainPage : ContentPage
    {
        static int maxWorkouts = 20;
        private WorkoutHistoryModel vm;
        public MainPage()
        {
            InitializeComponent();
            vm = new WorkoutHistoryModel();
            BindingContext = vm;
            Draw();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Draw();
        }
        private void Draw()
        {
            vstack.Clear();
            if (vm.routines.Count() > 0)
            {
                CreateLayout();
                DataLabel.IsVisible = false;
                logo.IsVisible = false;
            }
            else
            {
                DataLabel.IsVisible = true;
                logo.IsVisible = true;
            }
        }
        private void LoadMore(object sender, EventArgs e)
        {
            maxWorkouts += 20;
            Draw();
        }
        private void CreateLayout()
        {
            int count = 0;
            foreach(Routine r in vm.routines)
            {
                if (count >= maxWorkouts)
                    break;
                count++;
                Label name = new Label { Text = r.Name, TextColor = Colors.White, FontSize = 20, FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center};
                vstack.Children.Add(name);
                var allSets = r.Exercises.SelectMany(w => w.CheckedSets);
                int sets = allSets.Count();
                int reps = allSets.Sum(s => s.Reps);
                double volume = allSets.Sum(s => s.Weight * s.Reps);

                HorizontalStackLayout statsHStack = new HorizontalStackLayout { Spacing = 30, HorizontalOptions = LayoutOptions.Center };

                CreateHeader("VOLUME", volume.ToString("N0") + AppState.Profile.WeightUnit, statsHStack);
                CreateHeader("SETS", sets.ToString(), statsHStack);
                CreateHeader("REPS", reps.ToString(), statsHStack);
                CreateHeader("TIME", r.DurationString, statsHStack);

                vstack.Children.Add(statsHStack);

                VerticalStackLayout splitsStack = new VerticalStackLayout { Spacing = 10, Margin = new Thickness(0,10,0,0),
                        HorizontalOptions = LayoutOptions.Fill, IsVisible = r.IsSplitsVisible};

                Button splitsButton = new Button { Text = r.SplitsButtonText, FontSize = 15, FontAttributes = FontAttributes.Bold,
                        BackgroundColor = Color.FromRgba("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, WidthRequest = 200,
                        HorizontalOptions = LayoutOptions.Center};
                splitsButton.Clicked += (s, e) => { 
                    r.IsSplitsVisible = !r.IsSplitsVisible;
                    r.SplitsButtonText = r.IsSplitsVisible ? "Hide Splits" : "Show Splits";
                    splitsButton.Text = r.SplitsButtonText;
                    splitsStack.IsVisible = r.IsSplitsVisible;
                };
                vstack.Children.Add(splitsButton);
                Label functionLabel = new Label { Text = "Split", FontSize = 20, HorizontalOptions = LayoutOptions.Center, TextColor = Colors.AliceBlue,
                        FontAttributes = FontAttributes.Bold,  Margin = new Thickness(0,10,0,0)};
                splitsStack.Children.Add(functionLabel);
                UpdateSplits(AppState.MuscleFunctionsList, (e, split) => e.Function == split, splitsStack, r);

                Label groupLabel = new Label { Text = "Group", FontSize = 20, HorizontalOptions = LayoutOptions.Center, TextColor = Colors.AliceBlue,
                        FontAttributes = FontAttributes.Bold,  Margin = new Thickness(0,10,0,0)};
                splitsStack.Children.Add(groupLabel);
                UpdateSplits(AppState.MuscleGroupsList, (e, group) => e.MuscleGroup == group, splitsStack, r);

                vstack.Children.Add(splitsStack);

                VerticalStackLayout exercisesStack = new VerticalStackLayout { Spacing = 20, Margin = new Thickness(0,10,0,0),
                        HorizontalOptions = LayoutOptions.Fill, IsVisible = r.IsExercisesVisible};

                Button exercisesButton = new Button { Text = r.ExercisesButtonText, FontSize = 15, FontAttributes = FontAttributes.Bold,
                        BackgroundColor = Color.FromRgba("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, WidthRequest = 200,
                        HorizontalOptions = LayoutOptions.Center};
                                exercisesButton.Clicked += (s, e) => { 
                    r.IsExercisesVisible = !r.IsExercisesVisible;
                    r.ExercisesButtonText = r.IsExercisesVisible ? "Hide Exercises" : "Show Exercises";
                    exercisesButton.Text = r.ExercisesButtonText;
                    exercisesStack.IsVisible = r.IsExercisesVisible;
                };
                vstack.Children.Add(exercisesButton);

                foreach (Exercise e in r.Exercises)
                {
                    if(e.CheckedSets.Count() > 0)
                        CreateExerciseLayout(e, exercisesStack);
                }
                vstack.Children.Add(exercisesStack);

                Button deleteButton = new Button { Text = "Delete", FontSize = 15, FontAttributes = FontAttributes.None,
                        BackgroundColor = Color.FromRgba("#2b2b2b"), BorderWidth = 0, TextColor = Colors.Red, WidthRequest = 150,
                        HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start};
                deleteButton.Clicked += async (s, e) =>
                {
                    var result = await Shell.Current.DisplayAlert("Delete Workout", $"Are you sure you want to delete the workout '{r.Name}'?", "Yes", "No");
                    if (result)
                    {
                        AppState.RemoveWorkout(r);
                        vstack.Clear();
                        CreateLayout();
                    }
                };

                vstack.Children.Add(deleteButton);

                BoxView box = new BoxView { HeightRequest = 5, BackgroundColor = Color.FromArgb("#2b2b2b"), HorizontalOptions = LayoutOptions.FillAndExpand,
                        Margin = new Thickness(0,20,0,0)};
                vstack.Children.Add(box);
            }

            if(count >= maxWorkouts)
            {
                Button moreButton = new Button { Text = "More", FontSize = 15, FontAttributes = FontAttributes.Bold,
                    BackgroundColor = Color.FromRgba("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, WidthRequest = 275,
                    HorizontalOptions = LayoutOptions.Center, Padding = 5 };
                moreButton.Clicked += (s, e) => { LoadMore(s, e); };
                vstack.Children.Add(moreButton);
            }
        }

        private void CreateExerciseLayout(Exercise exercise,VerticalStackLayout exercisesStack)
        {
            Label name = new Label { Text = exercise.Name, FontSize = 20, TextColor = Colors.DodgerBlue, FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.WordWrap, MaxLines = 5};
            exercisesStack.Children.Add(name);
            if(exercise.Description != null)
                if (exercise.Description != String.Empty || exercise.Description != "")
                {
                    Label desc = new Label{ Text = exercise.Description,FontSize = 17,
                        HorizontalTextAlignment = TextAlignment.Center,MaxLines = 5, LineBreakMode = LineBreakMode.WordWrap};
                    exercisesStack.Children.Add(desc);
                }
            if (exercise.IsUnilateral)
            {
                Label leftHeader = new Label { Text = "Left Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center};
                exercisesStack.Children.Add(leftHeader);
            
                var leftStack = new VerticalStackLayout { Spacing = 20 };
                foreach (Set set in exercise.Sets.Where(s => s.Side == SideType.Left))
                    SetLayout(set, exercise, leftStack);
                exercisesStack.Children.Add(leftStack);
            
                Label rightHeader = new Label { Text = "Right Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
                exercisesStack.Children.Add(rightHeader);
            
                var rightStack = new VerticalStackLayout { Spacing = 20 };
                foreach (Set set in exercise.Sets.Where(s => s.Side == SideType.Right))
                    SetLayout(set, exercise, rightStack);
                exercisesStack.Children.Add(rightStack);
            }
            else
            {
                var setVStack = new VerticalStackLayout { Spacing = 20, Margin = new Thickness(0,10,0,0) };
                foreach (Set set in exercise.CheckedSets)
                {
                    SetLayout(set, exercise, setVStack);
                }
                exercisesStack.Children.Add(setVStack);
            }

        }

        private void SetLayout(Set set, Exercise exercise, VerticalStackLayout stack)
        {
            HorizontalStackLayout setHStack = new HorizontalStackLayout { Spacing = 70, HorizontalOptions = LayoutOptions.Center };
            VerticalStackLayout setVStackinner0 = new VerticalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
            Label h = new Label
            {
                Text = "SET",
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.LightGray,
                FontAttributes = FontAttributes.None,
                FontSize = 15
            };
            setVStackinner0.Children.Add(h);
            switch (set.Type)
            {
                case SetType.Normal:
                    Label normal = new Label
                    {
                        Text = set.ID.ToString(),
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.White,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        BackgroundColor = Colors.Black
                    };
                    setVStackinner0.Children.Add(normal);
                    break;
                case SetType.Warmup:
                    Label warmup = new Label
                    {
                        Text = "W",
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.Orange,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        BackgroundColor = Colors.Black
                    };
                    setVStackinner0.Children.Add(warmup);
                    break;
                case SetType.Failure:
                    Label failure = new Label
                    {
                        Text = "F",
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Colors.Red,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        BackgroundColor = Colors.Black
                    };
                    setVStackinner0.Children.Add(failure);
                    break;
                case SetType.Drop:
                    Label drop = new Label
                    {
                        Text = "D",
                        HorizontalTextAlignment = TextAlignment.Center,
                        TextColor = Color.FromRgba("#008cff"),
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        BackgroundColor = Colors.Black
                    };
                    setVStackinner0.Children.Add(drop);
                    break;
            }
            setHStack.Children.Add(setVStackinner0);
                    VerticalStackLayout setVStackinner1 = new VerticalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
            Label r = new Label
            {
                Text = "REPS",
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.LightGray,
                FontAttributes = FontAttributes.None,
                FontSize = 15
            };
            Label rvalue = new Label
            {
                Text = set.Reps.ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.LightGray,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                BackgroundColor = Colors.Black
            };
                    setVStackinner1.Children.Add(r);
            setVStackinner1.Children.Add(rvalue);
            setHStack.Children.Add(setVStackinner1);
                    VerticalStackLayout setVStackinner2 = new VerticalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
            Label w = new Label
            {
                Text = "WEIGHT",
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.LightGray,
                FontAttributes = FontAttributes.None,
                FontSize = 15
            };
            Label wvalue = new Label
            {
                Text = set.Weight.ToString("N2"),
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.LightGray,
                FontAttributes = FontAttributes.Bold,
                FontSize = 20,
                BackgroundColor = Colors.Black
            };
                    setVStackinner2.Children.Add(w);
            setVStackinner2.Children.Add(wvalue);
            setHStack.Children.Add(setVStackinner2);
            stack.Children.Add(setHStack);
        }
        private void UpdateSplits<T>(IEnumerable<T> sourceList, Func<Exercise, T, bool> filter, VerticalStackLayout targetStack, Routine routine)
        {
            int totalSets = routine.Exercises
                .SelectMany(s => s.CheckedSets)
                .Count();

            targetStack.Children.Clear();

            foreach (var item in sourceList)
            {
                Grid grid = new Grid
                {
                    Padding = 0,HorizontalOptions = LayoutOptions.FillAndExpand, ColumnSpacing = 10
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

                var vstack = new VerticalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center, Spacing = 2
                };
                Grid.SetColumn(vstack, 0);

                var pbar = new ProgressBar
                {
                    ProgressColor = Color.FromArgb("#008cff"), BackgroundColor = Colors.Black, HeightRequest = 12, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(pbar, 1);

                var gridLabel = new Label
                {
                    Text = "", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center
                };
                Grid.SetColumn(gridLabel, 2);

                var itemLabel = new Label
                {
                    Text = "", HorizontalOptions = LayoutOptions.Start, FontSize = 15
                };
                var bindItemLabel = new Label
                {
                    Text = "", FontSize = 16, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center
                };

                vstack.Children.Add(itemLabel);
                vstack.Children.Add(bindItemLabel);

                int sets = routine.Exercises.Where(e => filter(e, item)).SelectMany(s => s.CheckedSets).Count();
                double percentage = totalSets != 0 ? (double)sets / totalSets : 0;

                if(item != null)
                    itemLabel.Text = AppState.MGMToString(item);
                else
                {
                    Console.WriteLine("ERROR ------------- MUSCLE ENUM IS NULL IF THAT'S EVEN POSSIBLE!!! WHY IS C# ANNOYING ME");
                }
                bindItemLabel.Text = sets.ToString();
                pbar.Progress = percentage;
                gridLabel.Text = (percentage * 100).ToString("N0") + "%";

                grid.Children.Add(vstack);
                grid.Children.Add(pbar);
                grid.Children.Add(gridLabel);

                targetStack.Children.Add(grid);
            }
        }


        private void CreateHeader(String first, String second, HorizontalStackLayout stack)
        {
                VerticalStackLayout volumeVStack = new VerticalStackLayout { Spacing = 15, HorizontalOptions = LayoutOptions.Center };
                Label volumeHeader = new Label { Text = first, HorizontalOptions = LayoutOptions.Center, TextColor = Colors.LightGray,
                        FontAttributes = FontAttributes.Bold, FontSize = 15 };
                Label volumeStat = new Label { Text = second, FontSize = 17, FontAttributes = FontAttributes.Bold, 
                        TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
                volumeVStack.Children.Add(volumeHeader);
                volumeVStack.Children.Add(volumeStat);
                stack.Children.Add(volumeVStack);
        }


    }
}
