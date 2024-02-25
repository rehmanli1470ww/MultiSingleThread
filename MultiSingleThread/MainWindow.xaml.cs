using EntityFremworkImtahan.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace MultiSingleThread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, NotificationService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        private DateTime startTime;
        private ObservableCollection<Car> cars;

        public DispatcherTimer dispatcherTimer { get; set; }
        public ObservableCollection<Car> Cars { get => cars; set { cars = value; OnPropertyChanged(); } }
        public ObservableCollection<Car> Cars1 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            dispatcherTimer = new DispatcherTimer();
            Cars = new ObservableCollection<Car>();

        }



        private void test()
        {
            startTime = DateTime.Now;
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            LabelTimer.Content = elapsedTime.ToString(@"hh\:mm\:ss\:fff");
            CommandManager.InvalidateRequerySuggested();
        }

        private void StartMethod(object sender, RoutedEventArgs e)
        {

            if (SingleRB.IsChecked == true)
            {
                Cars1 = new ObservableCollection<Car>();
                test();
                Thread thread = new Thread(() =>
                {
                    var start = 1;
                    for (int i = 1; i <= start; i++)
                    {
                        if (File.Exists("cars" + i + ".json"))
                        {
                            var temp = File.ReadAllText("cars" + i + ".json");

                            foreach (var item in JsonSerializer.Deserialize<List<Car>>(temp))
                            {
                                Cars1.Add(item);
                            }
                            start += 1;
                        }
                    }
                    Thread.Sleep(1000);
                    Cars = Cars1;
                    dispatcherTimer.Stop();
                });
                thread.Start();
            }
            else if (MultiRB.IsChecked == true)
            {
                Cars1 = new ObservableCollection<Car>();
                test();
                var start = 1;
                for (int i = 1; i <= start; i++)
                {
                    if (File.Exists("cars" + i + ".json"))
                    {
                        Thread thread = new Thread(() =>
                        {
                            lock (this)
                            {
                                var temp = File.ReadAllText("cars" + i + ".json");

                                foreach (var item in JsonSerializer.Deserialize<List<Car>>(temp))
                                {
                                    Cars1.Add(item);
                                }
                                start += 1;
                                
                            }
                            Thread.Sleep(1000);
                            Cars = Cars1;
                            dispatcherTimer.Stop();
                        });
                        thread.Start();
                        
                    }
                }    

            }

        }
    }
}