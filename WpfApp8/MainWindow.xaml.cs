using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SmokersProblem
{
    public partial class MainWindow : Window
    {
        private static readonly object _lock = new object();
        private static Random _random = new Random();
        private static string[] _resources = { "Табак", "Бумага", "Спички" };
        private static string _brokerResource1, _brokerResource2;

        public MainWindow()
        {
            InitializeComponent();
            StartSimulation();
        }

        private void StartSimulation()
        {
            Thread brokerThread = new Thread(Broker);
            brokerThread.Start();

            Thread smoker1Thread = new Thread(() => Smoker("Табак", Smoker1Status));
            smoker1Thread.Start();

            Thread smoker2Thread = new Thread(() => Smoker("Бумага", Smoker2Status));
            smoker2Thread.Start();

            Thread smoker3Thread = new Thread(() => Smoker("Спички", Smoker3Status));
            smoker3Thread.Start();
        }

        private void Broker()
        {
            while (true)
            {
                lock (_lock)
                {
                    _brokerResource1 = _resources[_random.Next(3)];
                    do
                    {
                        _brokerResource2 = _resources[_random.Next(3)];
                    } while (_brokerResource1 == _brokerResource2);

                    Dispatcher.Invoke(() => BrokerStatus.Text = $"Посредник: {_brokerResource1} и {_brokerResource2}");
                }
                Thread.Sleep(1000);
            }
        }

        private void Smoker(string resource, TextBlock status)
        {
            while (true)
            {
                lock (_lock)
                {
                    if ((_brokerResource1 != resource && _brokerResource2 != resource) &&
                        (_brokerResource1 != null && _brokerResource2 != null))
                    {
                        Dispatcher.Invoke(() => status.Text = $"Курильщик с {resource}: Курит");
                        Thread.Sleep(2000); // Время на курение
                        Dispatcher.Invoke(() => status.Text = $"Курильщик с {resource}: Ожидает");
                        _brokerResource1 = null;
                        _brokerResource2 = null;
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
