using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExamMultiplyMatrixes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Size { get; set; }
        private List<Thread> activeThreads = new List<Thread>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void T1TextThread()
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            
            for (int i = 0; i < Size; i++)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    PBT1.Value = ((double)i + 1) / Size * 100;
                }));
                for (int j = 0; j < Size; j++)
                {

                    Matrix.MultiplreOneElement(new MatrixParams(Size, i, j, a, b, c, null));
                }
            }

        }
        private void TRTextThread()
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            Thread[] threadList1 = new Thread[Size];
            int s;
            for (int i = 0; i < Size; i++)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                s = i;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    PBThPR.Value = ((double)s + 1) / Size * 100;
                }));
                threadList1[i] = new Thread(() =>
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                            return;
                        Matrix.MultiplreOneElement(new MatrixParams(Size, s, j, a, b, c, null));
                    }
                });
                activeThreads.Add(threadList1[i]);
                threadList1[i].Start();
            }

            for (int i = 0; i < Size; i++)
            {
                threadList1[i].Join();
            }

        }
        private void TRTextTask()
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            Task[] TaskList = new Task[Size];
            int row;
            for (int i = 0; i < Size; i++)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                row = i;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    PBTaPR.Value = ((double)row + 1) / Size * 100;
                }));
                TaskList[i] = Task.Run(() =>
                {
                    for (int j = 0; j < Size; j++)
                    {
                        Matrix.MultiplreOneElement(new MatrixParams(Size, row, j, a, b, c, null));
                    }
                });
            }

            for (int i = 0; i < Size; i++)
            {
                TaskList[i].Wait();
            }
        }
        private void TETextThread()
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            Thread[,] threadList = new Thread[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    PBThPE.Value = ((double)i + 1) / Size * 100;
                }));
                for (int j = 0; j < Size; j++)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        return;
                    threadList[i, j] = new Thread(Matrix.MultiplreOneElement);
                    threadList[i, j].Start(new MatrixParams(Size, i, j, a, b, c, null));
                    activeThreads.Add(threadList[i, j]);
                }
            }
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    threadList[i, j].Join();
                }
            }

        }
        private void TETextTask()
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            Task[,] TaskList = new Task[Size, Size];
            int row, col;
            for (int i = 0; i < Size; i++)
            {
                row = i;
                if (cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    PBTaPE.Value = ((double)row + 1) / Size * 100;
                }));

                for (int j = 0; j < Size; j++)
                {
                    col = j;
                    TaskList[i, j] = Task.Run(() => Matrix.MultiplreOneElement(new MatrixParams(Size, row, col, a, b, c, null)));
                }
            }
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    TaskList[i, j].Wait();
                }
            }
        }

        private void T2TextThread(object ProggresBarr)
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            int n = 0;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                n = int.Parse(((ProgressBar)ProggresBarr).Tag.ToString());
            }));
            Thread[] threads = new Thread[n];
            int chunkSize = Size / n;
            for (int i = 0; i < n; i++)
            {
                int start1 = i * chunkSize;
                int end = (i == n - 1) ? Size : start1 + chunkSize;
                threads[i] = new Thread(() =>
                {
                    for (int i = start1; i < end; i++)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                            return;
                        else
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                if (i < Size / n && !cancellationTokenSource.Token.IsCancellationRequested)
                                {
                                    ((ProgressBar)ProggresBarr).Value = ((double)i * n + 1) / Size * 100;
                                }
                            }));
                        }
                        for (int j = 0; j < Size; j++)
                        {
                            Matrix.MultiplreOneElement(new MatrixParams(Size, i, j, a, b, c, null));
                        }
                    }
                });
                activeThreads.Add(threads[i]);
                threads[i].Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (!cancellationTokenSource.Token.IsCancellationRequested)
                    ((ProgressBar)ProggresBarr).Value = 100;
            }));

        }
        private void T2TextTask(int n, ProgressBar progressBar)
        {
            double[,] a = Matrix.CreateMatrix(Size);
            double[,] b = Matrix.CreateMatrix(Size);
            double[,] c = Matrix.CreateMatrix(Size);
            Task[] TaskList = new Task[n];
            int chunkSize = Size / n;
            for (int i = 0; i < n; i++)
            {
                int start1 = i * chunkSize;
                int end = (i == n - 1) ? Size : start1 + chunkSize;
                TaskList[i] = Task.Run(() =>
                {
                    for (int i = start1; i < end; i++)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                            return;
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            if (i < Size / n)
                            {
                                progressBar.Value = ((double)i * n + 1) / Size * 100;
                            }
                        }));
                        for (int j = 0; j < Size; j++)
                        {
                            Matrix.MultiplreOneElement(new MatrixParams(Size, i, j, a, b, c, null));
                        }
                    }
                });
            }
            foreach (Task thread in TaskList)
            {
                thread.Wait();
            }
            if (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                progressBar.Value = 100;
            }));
            }

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SizeText.Text, out int a) && a > 0)
            {
                Size = a;
                foreach (var item in GridWithBars.Children.OfType<StackPanel>().SelectMany(innerPanel => innerPanel.Children.OfType<ProgressBar>()))
                {
                    item.Value = 0;
                }
                foreach (CheckBox cb in GridWithBars
                    .Children
                    .OfType<StackPanel>()
                    .SelectMany(innerPanel => innerPanel.Children.OfType<CheckBox>()))
                {


                    if (cb.IsChecked == true)
                    {
                        

                        switch (cb.Name)
                        {
                            case "T1":
                                Task.Run(T1TextThread);
                                break;
                            case "T2":
                                Thread thread2 = new Thread(T2TextThread);
                                thread2.Start(PBTh2);
                                Task.Run(() => T2TextTask(2, PBTa2));
                                break;
                            case "T4":
                                Thread thread4 = new Thread(T2TextThread);
                                thread4.Start(PBTh4);
                                Task.Run(() => T2TextTask(4, PBTa4));
                                break;
                            case "T8":
                                Thread thread8 = new Thread(T2TextThread);
                                thread8.Start(PBTh8);
                                Task.Run(() => T2TextTask(8, PBTa8));
                                break;
                            case "T16":
                                Thread thread16 = new Thread(T2TextThread);
                                thread16.Start(PBTh16);
                                Task.Run(() => T2TextTask(16, PBTa16));
                                break;
                            case "TPerElement":
                                Thread threadE = new Thread(TETextThread);
                                threadE.Start();
                                Task.Run(TETextTask);
                                break;
                            case "TPerRow":
                                Thread threadR = new Thread(TRTextThread);
                                threadR.Start();
                                Task.Run(TRTextTask);
                                break;
                        }
                    }
                    
                }

            }

        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
            Thread.Sleep(100);
            foreach (var thread in activeThreads)
            {
                if (thread.IsAlive)
                {
                    thread.Interrupt();
                }
            }

            Application.Current.Shutdown();
        }


    }
}