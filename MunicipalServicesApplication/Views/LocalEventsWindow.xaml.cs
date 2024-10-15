using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MunicipalServicesApplication.Models;
using MunicipalServicesApplication.Services;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Threading;

namespace MunicipalServicesApplication.Views
{
    public partial class LocalEventsWindow : Window, INotifyPropertyChanged
    {
        private EventService _eventService;
        private List<LocalEvent> _allEvents;
        private List<LocalEvent> _displayedEvents;
        private int _currentPage = 1;
        private const int EventsPerPage = 12;
        private bool _isLoading;

        private SortedDictionary<DateTime, List<LocalEvent>> eventsByDate;
        private Dictionary<string, HashSet<LocalEvent>> eventsByCategory;
        private Queue<LocalEvent> upcomingEvents;
        private Stack<LocalEvent> pastEvents;

        private UserProfileService _userProfileService;
        private CurrentUser _currentUser;
        private List<LocalEvent> _recommendedEvents;

        private AnnouncementService _announcementService;
        private List<Announcement> _announcements;
        private DispatcherTimer _announcementTimer;

        public List<LocalEvent> RecommendedEvents
        {
            get => _recommendedEvents;
            set
            {
                _recommendedEvents = value;
                OnPropertyChanged(nameof(RecommendedEvents));
            }
        }

        public ICommand OpenEventUrlCommand { get; private set; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private string _loadingStatus;
        public string LoadingStatus
        {
            get => _loadingStatus;
            set
            {
                _loadingStatus = value;
                OnPropertyChanged();
            }
        }

        public List<LocalEvent> DisplayedEvents
        {
            get => _displayedEvents;
            set
            {
                _displayedEvents = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                UpdateDisplayedEvents();
            }
        }

        public int TotalPages
        {
            get => Math.Max((_allEvents?.Count ?? 0 + EventsPerPage - 1) / EventsPerPage, 1);
            private set { } // Add a private setter to avoid the CS0200 error
        }

        public LocalEventsWindow()
        {
            InitializeComponent();
            DataContext = this;
            _eventService = ((App)Application.Current).EventService;
            _userProfileService = new UserProfileService();
            _currentUser = _userProfileService.GetOrCreateUser(App.CurrentUser.Id);
            _allEvents = new List<LocalEvent>();
            eventsByDate = new SortedDictionary<DateTime, List<LocalEvent>>();
            eventsByCategory = new Dictionary<string, HashSet<LocalEvent>>();
            upcomingEvents = new Queue<LocalEvent>();
            pastEvents = new Stack<LocalEvent>();
            OpenEventUrlCommand = new RelayCommand<string>(OpenEventUrl);
            _announcementService = new AnnouncementService();
            _announcements = new List<Announcement>();
            Loaded += LocalEventsWindow_Loaded;
        }

        private async void LocalEventsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadAnnouncements();
            StartAnnouncementAnimation();
            IsLoading = true;
            LoadingStatus = "Loading Events...";
            await _eventService.WaitForInitialLoadAsync();
            await LoadEvents();
            
        }

        private async Task LoadAnnouncements()
        {
            _announcements = await _announcementService.GetAnnouncementsAsync();

            foreach (var announcement in _announcements)
            {
                AddAnnouncementToUI(announcement);
            }
            // Duplicate announcements for seamless looping
            foreach (var announcement in _announcements)
            {
                AddAnnouncementToUI(announcement);
            }
        }


        private void AddAnnouncementToUI(Announcement announcement)
        {
            var textBlock = new TextBlock
            {
                Text = $"{announcement.Title} - {announcement.Date:d}",
                TextWrapping = TextWrapping.NoWrap,
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(5),
            };

            var button = new Button
            {
                Content = textBlock,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.LightGray,
                Cursor = Cursors.Hand,
                Padding = new Thickness(5),
                Margin = new Thickness(5)
            };

            button.Click += (sender, e) => OpenAnnouncementUrl(announcement.Url);

            AnnouncementsStackPanel.Children.Add(button);
        }

        private void OpenAnnouncementUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void StartAnnouncementAnimation()
        {
            CompositionTarget.Rendering += AnnouncementAnimation_Tick;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CompositionTarget.Rendering -= AnnouncementAnimation_Tick;
        }

        private void AnnouncementAnimation_Tick(object sender, EventArgs e)
        {
            if (AnnouncementsScrollViewer == null || AnnouncementsStackPanel == null) return;

            double offset = AnnouncementsScrollViewer.HorizontalOffset;
            double maxOffset = AnnouncementsStackPanel.ActualWidth / 2;

            if (offset >= maxOffset)
            {
                AnnouncementsScrollViewer.ScrollToHorizontalOffset(0);
            }
            else
            {
                // Reduce the increment from 0.5 to 0.3 to slow down the animation
                AnnouncementsScrollViewer.ScrollToHorizontalOffset(offset + 0.3);
            }
        }

        private async Task LoadEvents()
        {
            try
            {
                IsLoading = true;
                LoadingStatus = "Initializing...";
                _allEvents = new List<LocalEvent>(_eventService.AllEvents);

                if (_allEvents.Count == 0)
                {
                    LoadingStatus = "Fetching events...";
                    await _eventService.GetEventsAsync(
                        onEventProcessed: (localEvent) =>
                        {
                            _allEvents.Add(localEvent);
                            AddEvent(localEvent);

                            LoadingStatus = $"Loaded {_allEvents.Count} events...";

                            if (_allEvents.Count % EventsPerPage == 0 || _allEvents.Count == 1)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    PopulateCategories();
                                    UpdateDisplayedEvents();
                                });
                            }
                        },
                        batchSize: EventsPerPage
                    );
                }
                else
                {
                    LoadingStatus = "Processing events...";
                    foreach (var localEvent in _allEvents)
                    {
                        AddEvent(localEvent);
                    }
                }

                if (_allEvents.Count == 0)
                {
                    MessageBox.Show("No events found or there was an error fetching events.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    LoadingStatus = "Finalizing...";
                    PopulateCategories();
                    UpdateDisplayedEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load events: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                LoadingStatus = string.Empty;
            }

            RecommendedEvents = _eventService.GetRecommendedEvents(_currentUser, 5);
            //UpdateRecommendedEvents();
        }

        //private void UpdateRecommendedEvents()
        //{
        //    RecommendedEventsItemsControl.ItemsSource = null;
        //    RecommendedEvents = _eventService.GetRecommendedEvents(_currentUser, 5);
        //    RecommendedEventsItemsControl.ItemsSource = RecommendedEvents;
        //    Console.WriteLine($"UpdateRecommendedEvents called. Count: {RecommendedEvents?.Count ?? 0}");
        //    foreach (var evt in RecommendedEvents)
        //    {
        //        Console.WriteLine($"Event: {evt.Title}, {evt.DateString}, {evt.Category}");
        //    }


        //}

        private void AddEvent(LocalEvent evt)
        {
            // Add to eventsByDate
            evt.Description = TruncateDescription(evt.Description);
            if (!eventsByDate.ContainsKey(evt.Date.Date))
            {
                eventsByDate[evt.Date.Date] = new List<LocalEvent>();
            }
            eventsByDate[evt.Date.Date].Add(evt);

            // Add to eventsByCategory
            if (!eventsByCategory.ContainsKey(evt.Category))
            {
                eventsByCategory[evt.Category] = new HashSet<LocalEvent>();
            }
            eventsByCategory[evt.Category].Add(evt);

            // Add to upcomingEvents or pastEvents
            if (evt.Date >= DateTime.Now)
            {
                upcomingEvents.Enqueue(evt);
            }
            else
            {
                pastEvents.Push(evt);
            }
        }

        private void PopulateCategories()
        {
            CategoryFilter.ItemsSource = eventsByCategory.Keys.ToList();
        }

        private void UpdateDisplayedEvents()
        {
            var filteredEvents = _allEvents;

            // Apply category filter
            if (CategoryFilter.SelectedItem is string selectedCategory && selectedCategory != "All Categories")
            {
                filteredEvents = filteredEvents.Where(e => e.Category == selectedCategory).ToList();
            }

            // Apply date filter
            if (DateFilter.SelectedDate.HasValue)
            {
                var selectedDate = DateFilter.SelectedDate.Value.Date;
                filteredEvents = filteredEvents.Where(e => e.Date.Date == selectedDate).ToList();
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                var searchTerm = SearchBox.Text.ToLower();
                filteredEvents = filteredEvents.Where(e =>
                    e.Title.ToLower().Contains(searchTerm) ||
                    e.Description.ToLower().Contains(searchTerm) ||
                    e.Category.ToLower().Contains(searchTerm)
                ).ToList();
            }

            // Get recommended events
            var recommendedEvents = _eventService.GetRecommendedEvents(_currentUser, 5);
            foreach (var evt in recommendedEvents)
            {
                evt.IsRecommended = true;
            }

            // Combine recommended and filtered events, ensuring recommended events appear first
            var combinedEvents = recommendedEvents.Union(filteredEvents).ToList();

            TotalPages = (int)Math.Ceiling(combinedEvents.Count / (double)EventsPerPage);
            DisplayedEvents = combinedEvents.Skip((CurrentPage - 1) * EventsPerPage).Take(EventsPerPage).ToList();

            OnPropertyChanged(nameof(DisplayedEvents));
            OnPropertyChanged(nameof(TotalPages));
        }

        private IEnumerable<LocalEvent> ApplyFilters(IEnumerable<LocalEvent> events)
        {
            // Apply search filter
            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                events = events.Where(e => e.Title.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                           e.Description.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // Apply category filter
            if (CategoryFilter.SelectedItem is string selectedCategory && !string.IsNullOrEmpty(selectedCategory))
            {
                events = events.Where(e => e.Category == selectedCategory);
            }

            // Apply date filter
            if (DateFilter.SelectedDate.HasValue)
            {
                var selectedDate = DateFilter.SelectedDate.Value.Date;
                events = events.Where(e => e.Date.Date == selectedDate);
            }

            return events.OrderBy(e => e.Date);
        }

        private string TruncateDescription(string description, int maxLength = 200)
        {
            if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
                return description;

            return description.Substring(0, maxLength) + "...";
        }

        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplayedEvents();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            CategoryFilter.SelectedIndex = -1;
            DateFilter.SelectedDate = null;
            UpdateDisplayedEvents();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentPage = 1;
            _userProfileService.AddSearchQuery(_currentUser.Id, SearchBox.Text);
            UpdateDisplayedEvents();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPage = 1;
            if (CategoryFilter.SelectedItem is string selectedCategory)
            {
                _userProfileService.AddCategoryInteraction(_currentUser.Id, selectedCategory);
            }
            UpdateDisplayedEvents();
        }

        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPage = 1;
            UpdateDisplayedEvents();
        }

        private void OpenEventUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var selectedEvent = _allEvents.FirstOrDefault(e => e.Url == url);
                if (selectedEvent != null)
                {
                    _userProfileService.AddViewedEvent(_currentUser.Id, selectedEvent.Id);
                }
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                EventsScrollViewer.ScrollToTop();
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                EventsScrollViewer.ScrollToTop();
            }
        }



        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // This will close the LocalEventsWindow and return to the MainWindow
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute((T)parameter);

        public void Execute(object parameter) => _execute((T)parameter);
    }
}