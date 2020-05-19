using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.IO.IsolatedStorage;

namespace CA_Aoife_Egan_S00167887
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static int[] graphData = new int[];

        //create an instance of MainWindow class
        public static MainWindow mainWindow;

        //ThreadingCode class instance
        static ThreadingCode threadingObj;

        // holds data
        PointCollection points = new PointCollection();

        // n  is added to each point
        [ThreadStatic]
        double n = 0;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;

            //default light theme
            //rbtnLight.IsChecked = true;

            //main thread reads user inputs & creates points obj


            //PLOTTING METHOD IS SHARED BY T1,T2,T3

            //create threads
            

            // t2 adds n to x, reads in, changes points data & plots 

            // t3 adds n to y, reads in. changes points & plots

            // t4 writes points data to a txt file on save byn click

            // t5 can cancel the saving data to txt file & deletes file 

         


        }

        //method to apply selected theme 
        public void applyTheme(string c)
        {
            try
            {
                // convert string to colour obj
                Color colour = (Color)ColorConverter.ConvertFromString(c);
                grid1.Background = new SolidColorBrush(colour);
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not change theme. \n{0}", e.Message);
            }
        }

        private void btnEnterData_Click(object sender, RoutedEventArgs e)
        {
            // data is read in using the main thread

            //read in X & Y values
            double X = Double.Parse(txtbxX.Text);
            double Y = Double.Parse(txtbxY.Text);   
            
            //create data points
            points.Add(new Point(X, Y));

            //clear textboxes for new points
            txtbxX.Clear();
            txtbxY.Clear();
            
        }

        private void btnAddN_Click(object sender, RoutedEventArgs e)
        {
            // t2 add n+1 to x
            Thread t2 = new Thread(new ThreadStart(addNToX));
            //set name
            t2.Name = "Thread 2";
            // t3 add n+2 to y
            Thread t3 = new Thread(new ThreadStart(addNToY));

            n = Double.Parse(txtbxN.Text);

            //start threads
            t2.Start();
            t2.Start();
        }

        public void addNToX()
        {
            double newN = n + 1;

                     
            
           

        }

        public void addNToY()
        {
            double newN = n + 2;




        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPlot_Click(object sender, RoutedEventArgs e)
        {
            //create thread
            Thread t1 = new Thread(new ThreadStart(plotGraph));
            //name thread
            t1.Name = "Thread 1";
            //start thread
            t1.Start();          
        }

        public void plotGraph()
        {
            Dispatcher.Invoke(() =>
            {            
                //Draw graph 
                const double margin = 10;
                double xmin = margin;
                double xmax = graph.Width - margin;
                double ymin = margin;
                double ymax = graph.Height - margin;
                const double step = 10;

                //make X axis
                GeometryGroup xaxis_geom = new GeometryGroup();
                xaxis_geom.Children.Add(new LineGeometry(new Point(0, ymax), new Point(graph.Width, ymax)));

                for (double x = xmin + step; x <= graph.Width - step; x += step)
                {
                    xaxis_geom.Children.Add(new LineGeometry(
                        new Point(x, ymax - margin / 2),
                        new Point(x, ymax + margin / 2)));
                }

                Path xaxis_path = new Path();
                xaxis_path.StrokeThickness = 1;
                xaxis_path.Stroke = Brushes.Black;
                xaxis_path.Data = xaxis_geom;

                graph.Children.Add(xaxis_path);

                //make y axis 
                GeometryGroup yaxis_geom = new GeometryGroup();
                yaxis_geom.Children.Add(new LineGeometry(
                    new Point(xmin, 0), new Point(xmin, graph.Height)));
                for (double y = step; y <= graph.Height - step; y += step)
                {
                    yaxis_geom.Children.Add(new LineGeometry(
                        new Point(xmin - margin / 2, y),
                        new Point(xmin + margin / 2, y)));
                }

                Path yaxis_path = new Path();
                yaxis_path.StrokeThickness = 1;
                yaxis_path.Stroke = Brushes.Black;
                yaxis_path.Data = yaxis_geom;

                graph.Children.Add(yaxis_path);

                make_data(ymin, ymax, xmin, xmax, step);
            });
        }

        public void make_data(double ymin, double ymax, double xmin, double xmax, double step)
        {
            // make some data sets
            Brush brush = Brushes.PaleVioletRed;        
            Polyline polyline = new Polyline();
            polyline.StrokeThickness = 1;
            polyline.Stroke = brush;
            polyline.Points = points;

            graph.Children.Add(polyline);

            //place dots at data points
            const float width = 4;
            const float radius = width / 2;
            foreach (Point point in points)
            {
                //t1.Sleep(2000);
                Ellipse dot = new Ellipse();
                dot.SetValue(Canvas.LeftProperty, point.X - radius);
                dot.SetValue(Canvas.TopProperty, point.Y - radius);
                dot.Fill = brush;
                dot.Stroke = brush; 
                dot.StrokeThickness = 1;
                dot.Width = width;
                dot.Height = width;
                graph.Children.Add(dot);
            }         
        }

        private void rbtnDark_Checked(object sender, RoutedEventArgs e)
        {
            // call constructor method to create threadingCode obj
            threadingObj = new ThreadingCode();

            //ensure radio button is selected
            if (rbtnDark.IsChecked != false)
            {
                //create thread to access file 
                // t6 sets dark theme & writes to isolated storage
                Thread t6 = new Thread(new ParameterizedThreadStart(threadingObj.writeToStorage));
                //set thread name
                t6.Name = "Thread 6";
                // start t6  thread & pass theme colour into it 
                string colour = "Gray";
                t6.Start(colour);
            }        
        }

        private void rbtnLight_Checked(object sender, RoutedEventArgs e)
        {
            // call constructor method to create threadingCode obj
            threadingObj = new ThreadingCode();

            //ensure radio button is selected
            if (rbtnLight.IsChecked != false)
            {
                //create thread to access file 
                // t6 sets dark theme & writes to isolated storage
                Thread t7 = new Thread(new ParameterizedThreadStart(threadingObj.writeToStorage));

                //set thread name
                t7.Name = "Thread 7";

                // start t6  thread & pass theme colour into it 
                string colour = "White";
                t7.Start(colour);
            }
        }
    }
}
