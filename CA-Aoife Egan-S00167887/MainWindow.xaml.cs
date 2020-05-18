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

namespace CA_Aoife_Egan_S00167887
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //static int[] graphData = new int[];

        public MainWindow()
        {
            InitializeComponent();



            
        }

        private void btnEnterData_Click(object sender, RoutedEventArgs e)
        {
            //dataArray
            //txtbxX 
        }

        private void btnAddN_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPlot_Click(object sender, RoutedEventArgs e)
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

        }      

        public void make_data(double ymin, double ymax, double xmin, double xmax, double step)
        {
            // make some data sets
            Brush[] brushes = { Brushes.Red, Brushes.Green, Brushes.Blue };
            Random rand = new Random();
            for (int data_set = 0; data_set < 3; data_set++)
            {
                int last_y = rand.Next((int)ymin, (int)ymax);

                PointCollection points = new PointCollection();
                for (double x = xmin; x <= xmax; x += step)
                {
                    last_y = rand.Next(last_y - 10, last_y + 10);
                    if (last_y < ymin)
                    {
                        last_y = (int)ymin;
                    }
                    if (last_y > ymax)
                    {
                        last_y = (int)ymax;
                    }
                    points.Add(new Point(x, last_y));
                }

                Polyline polyline = new Polyline();
                polyline.StrokeThickness = 1;
                polyline.Stroke = brushes[data_set];
                polyline.Points = points;

                graph.Children.Add(polyline);
            }
        }
    }
}
