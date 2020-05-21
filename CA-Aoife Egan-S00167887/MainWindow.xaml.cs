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
using System.IO;

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

        // synchronisation obj
        public static Object synObj = new Object();

        // n  is added to each point
        [ThreadStaticAttribute()]
        double n = 0; //Double.Parse(mainWindow.txtbxN.Text);

        public static Thread t2;
        public static Thread t3;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;

            //default light theme
            rbtnLight.IsChecked = true;

            //main thread:
                // -reads user inputs & creates points obj
                // -saves points to data text file in documents     
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
            double X, Y;            
            // data is read in using the main thread        
            
            //double X = Double.Parse(txtbxX.Text);
            //double Y = Double.Parse(txtbxY.Text);
            Monitor.Enter(synObj);
            try
            {
                //read in X & Y values & validate user input
                if (Double.TryParse(txtbxX.Text, out X) && Double.TryParse(txtbxY.Text, out Y))
                {
                    //create data points
                    points.Add(new Point(X, Y));

                }
                else
                {
                    MessageBox.Show("Please input a number in double format");
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                Monitor.Exit(synObj);
            }

            //clear textboxes for new points
            txtbxX.Clear();
            txtbxY.Clear();
            
        }

        private void btnAddN_Click(object sender, RoutedEventArgs e)
        {
            // call constructor method to create threadingCode obj
           // threadingObj = new ThreadingCode();

            // t2 add n+1 to x
            t2 = new Thread(new ThreadStart(addNToX));
            //set name
            t2.Name = "Thread 2";
            // t3 add n+2 to y
            t3 = new Thread(new ThreadStart(addNToY));
            //set name
            t3.Name = "Thread 3";

            //start threads
            t2.Start();
            
            t3.Start();
            

        }

        public void addNToX()
        {
            n = n + 10;
            Monitor.Enter(synObj);
            try
            {
                MessageBox.Show(Thread.CurrentThread.Name + " is adding " + n + " to X values");
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(points.ToString(), "X t2");
                    PointCollection newPoints = new PointCollection();

                    for (int i = 0; i < points.Count(); i++)
                    {
                        Point newPoint = points[i];
                        newPoint.X = newPoint.X + n;
                        newPoints.Add(newPoint);
                    }
                    points = newPoints;
                    MessageBox.Show(points.ToString());                    

                });
                Monitor.Pulse(synObj);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

                Monitor.Exit(synObj);
            }
        }

        public void addNToY()
        {
            t2.Join(); //wait until t2 is finished change x values then change y values
            n = n + 10;
            Monitor.Enter(synObj);
            try
            {
                MessageBox.Show(Thread.CurrentThread.Name + " is subtracting " + n + " from Y values");

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(points.ToString(), "Y t3");
                    PointCollection newPoints = new PointCollection();

                    for (int i = 0; i < points.Count(); i++)
                    {
                        Point newPoint = points[i];
                        newPoint.Y = newPoint.Y - n;
                        newPoints.Add(newPoint);
                    }
                    points = newPoints;
                    MessageBox.Show(points.ToString());
                                       
                    //plot new points
                    plotGraph();
                });
                Monitor.Pulse(synObj);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {

                Monitor.Exit(synObj);
            }


        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
          // main method thread working here
            saveData();

        }

        //method to save data to dataFile.txt
        public void saveData()
        {
            //synchronise access to data text file
            Monitor.Enter(synObj);
            try
            {                
                // Set a variable to the Documents path.
                string docPath =
                  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                Dispatcher.Invoke(() =>
                {
                    using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(docPath, "DataFile.txt")))
                    {
                        MessageBox.Show("Saving to DataFile.txt");

                        //make thread sleep to wait incase quit button is clicked
                        Thread.Sleep(1000);
                        writer.Write(points);                     
                        
                    }
                });
                Monitor.Pulse(synObj);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }
            finally
            {
                Monitor.Exit(synObj);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //Quits application & deletes 
            //create thread
            Thread t4 = new Thread(new ParameterizedThreadStart(deleteDataFile));
            //name thread
            t4.Name = "Thread 4";
            string fileName = "DataFile.txt";
            //start thread
            t4.Start(fileName);
            //deleteIsolatedStorage("ColourFile.txt");
        }

        public void deleteDataFile(Object fileName)
        {
            string file = fileName.ToString();
            //folder path
            string rootFolder = @"C:\Users\hp\Documents";
            try
            {
                //check file exists
                if (File.Exists(System.IO.Path.Combine(rootFolder, file)))
                {
                    MessageBox.Show("File corrupted! \n" + Thread.CurrentThread.Name + " is deleting " + file);
                    Thread.Sleep(1000); // take one second to give use chance to click ok
                    //delete
                    File.Delete(System.IO.Path.Combine(rootFolder, file));
                }
                else
                {
                    MessageBox.Show(file + " does not exist");
                }
            }
            catch(IOException ioExp)
            {
                MessageBox.Show(ioExp.Message);             
            }
            
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
                    double xmin = 0;// margin;
                    double xmax = graph.Width - margin;
                    double ymin = 0;// margin;
                    double ymax = graph.Height - margin;
                    const double step = 10;

                    //make X axis
                    GeometryGroup xaxis_geom = new GeometryGroup();
                    //xaxis_geom.Children.Add(new LineGeometry(new Point(0, ymax), new Point(graph.Width, ymax)));
                    xaxis_geom.Children.Add(new LineGeometry(new Point(0, ymax), new Point(graph.Width, ymax)));

                    for (double x = xmin + step; x <= graph.Width - step; x += step)
                    {
                        xaxis_geom.Children.Add(new LineGeometry(
                            new Point(x, ymax - margin / 2),
                            new Point(x, ymax + margin / 2)));
                    }

                   System.Windows.Shapes.Path xaxis_path = new System.Windows.Shapes.Path();
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

                    System.Windows.Shapes.Path yaxis_path = new System.Windows.Shapes.Path();
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
                if (graph.Children.Contains(polyline))
                {
                    graph.Children.Remove(polyline);
                }
                polyline.StrokeThickness = 1;
                polyline.Stroke = brush;
                polyline.Points = points;

                graph.Children.Add(polyline);

                //place dots at data points
                const float width = 4;
                const float radius = width / 2;            
                foreach (Point point in points)
                {
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
                //setting as background thread 
                t6.IsBackground = true;
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
                // t5 sets light theme & writes to isolated storage
                Thread t5 = new Thread(new ParameterizedThreadStart(threadingObj.writeToStorage));
                //set thread name
                t5.Name = "Thread 5";
                //setting as background thread
                t5.IsBackground = true;
                // start t5  thread & pass theme colour into it 
                string colour = "White";
                t5.Start(colour);
            }
        }
    }
}
