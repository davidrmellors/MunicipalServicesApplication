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

        public int TotalPages => Math.Max((_allEvents?.Count ?? 0 + EventsPerPage - 1) / EventsPerPage, 1);

        public LocalEventsWindow()
        {
            InitializeComponent();
            DataContext = this;
            _eventService = ((App)Application.Current).EventService;
            _allEvents = new List<LocalEvent>();
            eventsByDate = new SortedDictionary<DateTime, List<LocalEvent>>();
            eventsByCategory = new Dictionary<string, HashSet<LocalEvent>>();
            upcomingEvents = new Queue<LocalEvent>();
            pastEvents = new Stack<LocalEvent>();
            OpenEventUrlCommand = new RelayCommand<string>(OpenEventUrl);
            Loaded += LocalEventsWindow_Loaded;
        }

        private async void LocalEventsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsLoading = true;
            LoadingStatus = "Loading Events...";
            await _eventService.WaitForInitialLoadAsync();
            await LoadEvents();
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
        }

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
            var filteredEvents = ApplyFilters(_allEvents);
            DisplayedEvents = filteredEvents
                .Skip((CurrentPage - 1) * EventsPerPage)
                .Take(EventsPerPage)
                .ToList();

            if (DisplayedEvents.Count > 0)
            {
                EventsItemsControl.ItemsSource = null;
                EventsItemsControl.ItemsSource = DisplayedEvents;
            }
            else
            {
                EventsItemsControl.ItemsSource = null;
            }

            OnPropertyChanged(nameof(TotalPages));

            Dispatcher.InvokeAsync(() => EventsScrollViewer.ScrollToTop(), System.Windows.Threading.DispatcherPriority.Render);
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

        private void OpenEventUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CurrentPage = 1;
            UpdateDisplayedEvents();
        }

        private void CategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPage = 1;
            UpdateDisplayedEvents();
        }

        private void DateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPage = 1;
            UpdateDisplayedEvents();
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