using GymTracker.Models;
using GymTracker.Services;
using Microsoft.Maui.Controls.Shapes;
using System.Globalization;
namespace GymTracker
{
    public partial class MainPage : ContentPage
    {
        private WorkoutHistoryModel vm;

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (vm.HasRoutinesChanged == true)
            {
                collectionView.ItemsSource = null;
                collectionView.ItemTemplate = null;
                CreateLayout();
                vm.HasRoutinesChanged = false;
            }
        }
        private void CreateLayout()
        {
            if (vm.Routines.Count() < 1)
                DataLabel.IsVisible = true;
            else
                DataLabel.IsVisible = false;
            if (AppState.Workouts.Count() > 20)
                loadMoreButton.IsVisible = true;
            else
                loadMoreButton.IsVisible = false;
            loadMoreButton.SetBinding(Button.CommandProperty, "LoadMoreCommand");
            collectionView.ItemsSource = vm.Routines;
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var outerStack = new VerticalStackLayout 
                { 
                    Spacing = 20, 
                    Padding = 10, 
                    BackgroundColor = Colors.Black 
                };

                var nameLabel = new Label 
                { 
                    FontSize = 20, 
                    FontAttributes = FontAttributes.Bold, 
                    TextColor = Color.FromArgb("#008cff"), 
                    HorizontalOptions = LayoutOptions.Center 
                };
                nameLabel.SetBinding(Label.TextProperty, "Name");
                outerStack.Children.Add(nameLabel);

                var scrollView = new ScrollView 
                { 
                    Orientation = ScrollOrientation.Horizontal, 
                    HorizontalOptions = LayoutOptions.Center 
                };

                var statsGrid = new Grid
                {
                    ColumnSpacing = 40,
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    }
                };

                AddStatColumn("VOLUME", new Binding("VolumeDisplay"), statsGrid, 0);
                AddStatColumn("SETS", new Binding("Sets"), statsGrid, 1);
                AddStatColumn("REPS", new Binding("Reps"), statsGrid, 2);
                AddStatColumn("TIME", new Binding("DurationString"), statsGrid, 3);

                scrollView.Content = statsGrid;
                outerStack.Children.Add(scrollView);

            
              var splitsButton = new Button
                {
                    BackgroundColor = Color.FromArgb("#2b2b2b"),
                    TextColor = Colors.White,
                    WidthRequest = 200,
                    HorizontalOptions = LayoutOptions.Center
                };
                splitsButton.SetBinding(Button.TextProperty, "SplitsButtonText");
                
                var splitsStack = new VerticalStackLayout { Spacing = 10, Margin = new Thickness(0, 10, 0, 0) };
                splitsStack.SetBinding(VerticalStackLayout.IsVisibleProperty, "IsSplitsVisible");
                
                splitsButton.Clicked += (s, e) =>
                {
                    if (splitsButton.BindingContext is Routine routine)
                    {
                        routine.IsSplitsVisible = !routine.IsSplitsVisible;
                        routine.SplitsButtonText = routine.IsSplitsVisible ? "Hide Splits" : "Show Splits";
                
                        if (routine.IsSplitsVisible)
                        {
                            UpdateSplits(AppState.MuscleFunctionsList, (exercise, split) => exercise.Function == split, splitsStack, routine);
                            
                            var groupLabel = new Label
                            {
                                Text = "Group",
                                FontSize = 20,
                                HorizontalOptions = LayoutOptions.Center,
                                TextColor = Colors.AliceBlue,
                                FontAttributes = FontAttributes.Bold,
                                Margin = new Thickness(0, 10, 0, 0)
                            };
                            splitsStack.Children.Add(groupLabel);
                
                            UpdateSplits(AppState.MuscleGroupsList, (exercise, group) => exercise.MuscleGroup == group, splitsStack, routine);
                        }
                        else
                        {
                            splitsStack.Children.Clear();
                        }
                    }
                };
                
                outerStack.Children.Add(splitsButton);
                outerStack.Children.Add(splitsStack);

            
                var exercisesButton = new Button
                {
                    BackgroundColor = Color.FromArgb("#2b2b2b"),
                    TextColor = Colors.White,
                    WidthRequest = 200,
                    HorizontalOptions = LayoutOptions.Center
                };
                exercisesButton.SetBinding(Button.TextProperty, "ExercisesButtonText");
            
                var exercisesCV = new CollectionView
                {
                    ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical) { ItemSpacing = 15 },
                    BackgroundColor = Colors.Transparent
                };
                exercisesCV.SetBinding(ItemsView.ItemsSourceProperty, "Exercises");
                exercisesCV.SetBinding(CollectionView.IsVisibleProperty, "IsExercisesVisible");
                exercisesButton.Clicked += (s, e) =>
                {
                    if (exercisesButton.BindingContext is Routine routine)
                    {
                        routine.IsExercisesVisible = !routine.IsExercisesVisible;
                        routine.ExercisesButtonText = routine.IsExercisesVisible ? "Hide Exercises" : "Show Exercises";
                    }
                };
            
                exercisesCV.ItemTemplate = new DataTemplate(() =>
                {

                    var container = AppState.Helper_CreateContainer();

                    var exerciseStack = new VerticalStackLayout { Spacing = 10, Padding = 5 };
                
                    var exLabel = new Label
                    {
                        FontSize = 20,
                        TextColor = Color.FromArgb("#008cff"),
                        FontAttributes = FontAttributes.Bold,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    exLabel.SetBinding(Label.TextProperty, "Name");
                    exerciseStack.Children.Add(exLabel);

                    var nameseparator = AppState.Helper_CreateSeparator();
                    exerciseStack.Children.Add(nameseparator);
                
                    var descLabel = new Label
                    {
                        FontSize = 18,
                        TextColor = Colors.LightGray,
                        FontAttributes = FontAttributes.None,
                        HorizontalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center
                    };
                    descLabel.SetBinding(Label.TextProperty, "Description");
                    descLabel.SetBinding(Label.IsVisibleProperty, "Description", converter: new StringToVisibilityConverter());
                    exerciseStack.Children.Add(descLabel);

                    var descseparator = AppState.Helper_CreateSeparator();
                    descseparator.SetBinding(BoxView.IsVisibleProperty, "Description", converter: new StringToVisibilityConverter());
                    exerciseStack.Children.Add(descseparator);
                
                    exerciseStack.BindingContextChanged += (s, e) =>
                    {
                        var stack = (VerticalStackLayout)s;
                        if (stack.BindingContext is Exercise ex)
                        {
                            while (stack.Children.Count > 2)
                                stack.Children.RemoveAt(2);
                
                            if (ex.IsUnilateral)
                            {
                                var leftStack = new VerticalStackLayout { Spacing = 20 };
                                SetLayout(ex.CheckedSets.Where(s => s.Side == SideType.Left), ex, leftStack, "SET (L)");
                                stack.Children.Add(leftStack);

                
                                var rightStack = new VerticalStackLayout { Spacing = 20 };
                                SetLayout(ex.CheckedSets.Where(s => s.Side == SideType.Right), ex, rightStack, "SET (R)");
                                stack.Children.Add(rightStack);
                            }
                            else
                            {
                                var setVStack = new VerticalStackLayout { Spacing = 20, Margin = new Thickness(0, 10, 0, 0) };
                                SetLayout(ex.CheckedSets, ex, setVStack, "SET");
                                stack.Children.Add(setVStack);
                            }
                        }
                    };
                    container.Content = exerciseStack;
                    return container;
                });

                outerStack.Children.Add(exercisesButton);
                outerStack.Children.Add(exercisesCV);
                var deleteButton = new Button
                {
                    Text = "Delete",
                    TextColor = Colors.Red,
                    BackgroundColor = Color.FromArgb("#2b2b2b"),
                    WidthRequest = 150,
                    HorizontalOptions = LayoutOptions.Center
                };
                deleteButton.Clicked += (s, e) =>
                {
                    if (deleteButton.BindingContext is Routine routine)
                        vm.OnDeleteWorkout(routine);
                };
                outerStack.Children.Add(deleteButton);
            
                var separator = AppState.Helper_CreateSeparator();
                outerStack.Children.Add(separator);

                return outerStack;
            });


        }
        public MainPage()
        {
            InitializeComponent();
            vm = new WorkoutHistoryModel();
            BindingContext = vm;

            CreateLayout();
        }

        private void AddStatColumn(string header, Binding valueBinding, Grid grid, int column)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        
            var headerLabel = new Label
            {
                Text = header,
                FontSize = 15,
                TextColor = Color.FromArgb("#008cff"),
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };
        
            var valueLabel = new Label
            {
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.NoWrap
            };
            valueLabel.SetBinding(Label.TextProperty, valueBinding);
        
            grid.Add(headerLabel, column, 0);
            grid.Add(valueLabel, column, 1);
        }

        private void SetLayout(IEnumerable<Set> sets, Exercise exercise, VerticalStackLayout stack, string settext)
        {
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions = { },
                HorizontalOptions = LayoutOptions.Fill,
                ColumnSpacing = 0,
                RowSpacing = 0, BackgroundColor = Colors.Transparent
            };

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Label setlabel = new Label { Text = settext, BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#008cff"), FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            grid.Add(setlabel, 0, 0);
            Label replabel = new Label { Text = "REPS", BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#008cff"), FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            grid.Add(replabel, 1, 0);
            Label weightlabel = new Label { Text = "WEIGHT", BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#008cff"), FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            grid.Add(weightlabel, 2, 0);

            int row = 1;
            foreach (Set set in sets)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var setseparator = AppState.Helper_CreateSeparator();
                Grid.SetRow(setseparator, row);
                Grid.SetColumn(setseparator, 0);
                Grid.SetColumnSpan(setseparator, 3);
                grid.Children.Add(setseparator);

                row++;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                Label svalue = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Colors.Transparent
                };
                switch (set.Type)
                {
                    case SetType.Normal:
                        svalue.Text = set.ID.ToString();
                        svalue.TextColor = Colors.White;
                        break;
                    case SetType.Warmup:
                        svalue.Text = "W";
                        svalue.TextColor = Colors.Orange;
                        break;
                    case SetType.Failure:
                        svalue.Text = "F";
                        svalue.TextColor = Colors.Red;
                        break;
                    case SetType.Drop:
                        svalue.Text = "D";
                        svalue.TextColor = Color.FromRgba("#008cff");
                        break;
                }

                Label rvalue = new Label
                {
                    Text = set.Reps.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.LightGray,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Colors.Transparent
                };
                Label wvalue = new Label
                {
                    Text = set.Weight.ToString("N2") + AppState.Profile.WeightUnit,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.LightGray,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Colors.Transparent
                };
                grid.Add(svalue, 0, row);
                grid.Add(rvalue, 1, row);
                grid.Add(wvalue, 2, row);
                row++;
            }
            stack.Children.Add(grid);
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
        public class StringToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var str = value as string;
                return !string.IsNullOrWhiteSpace(str);
            }
        
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
    }
}
