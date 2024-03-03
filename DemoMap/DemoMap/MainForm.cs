using GMap.NET.MapProviders;
using GMap.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MapDataProvider;
using GMap.NET.WindowsForms;
using System.Drawing.Imaging;
using System.Linq;
using MapDataProvider.DataSourceProvoders.Contracts;
using System.Threading.Tasks;

namespace DemoMap
{
    public partial class MainForm : Form
    {
        Size _panelSize = new Size();
        bool _panel = false;
        bool _isNightMode = false;
        bool _currentMode = false;
        string _url = string.Empty;
        DataProvider _dataProvider = new DataProvider();
        LoadingForm _loader = new LoadingForm();

        GMapOverlay _polyOverlay = new GMapOverlay("polygons");
        public MainForm()
        {
            _loader.Show();
            InitializeComponent();
            var configs = _dataProvider.Providers;
            gMapControl.MapProvider = GMapProviders.List[4];
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            gMapControl.SetPositionByKeywords("Kyiv, Ukrane");
            gMapControl.Position = new PointLatLng(49.0, 32.0);
            gMapControl.MinZoom = 0;
            gMapControl.MaxZoom = 22;
            gMapControl.Zoom = 6;
            gMapControl.DragButton = MouseButtons.Left;
            gMapControl.OnMapDrag += OnMapDrag;
            gMapControl.MouseMove += OnMouseMove;
            gMapControl.OnMapZoomChanged += OnZoomChanged;
            matrixDefault = gMapControl.ColorMatrix;
            gMapControl.EmptyMapBackground = Color.Gray;

            OnMapDrag();
            OnZoomChanged();


            SetMode(IsNight());

            WindowState = FormWindowState.Maximized;
            _panelSize.Width = panel1.Width;
            _panelSize.Height = panel1.Height;

            HidePanel();

            comboBox.DataSource = GMapProviders.List;
            comboBox.SelectedIndex = 4;

            listBox1.Items.AddRange(_dataProvider.Providers.Select(x => (object)x.Name).ToArray());
            //listBox1.SelectedIndex = 0;
            DrawSource(_dataProvider.Providers[4]);
        }

        private void DrawSource(IDataProvider source)
        {
            var result = source.GetDataAsync().Result;
            labelErr.Text = result.Errors.Count > 0 ? "Error occurred" : string.Empty;
            _polyOverlay.Clear();
            gMapControl.Overlays.Clear();
            foreach (var polygon in result.Polygons)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                foreach (var point in polygon.Points)
                {
                    points.Add(new PointLatLng(point.Lat, point.Lng));
                }
                GMapPolygon gPolygon = new GMapPolygon(points, polygon.Name);
                gPolygon.Fill = polygon.Style.Fill;
                gPolygon.Stroke = polygon.Style.Stroke;

                _polyOverlay.Polygons.Add(gPolygon);
            }
            foreach (var line in result.Lines)
            {
                List<PointLatLng> points = new List<PointLatLng>();
                foreach (var point in line.Points)
                {
                    points.Add(new PointLatLng(point.Lat, point.Lng));
                }
                GMapRoute gMapRoute = new GMapRoute(points, line.Name);
                gMapRoute.Stroke = line.Style.Stroke;

                _polyOverlay.Routes.Add(gMapRoute);
            }
            linkLabel1.Text = source.WebSite.Length < 26 ? source.WebSite : source.WebSite.Substring(0, 23);
            _url = source.WebSite;
            gMapControl.Overlays.Add(_polyOverlay);
        }

        ColorMatrix matrixGray = new ColorMatrix(new float[][]
        {
            new float[] { -.2126f, -.2126f, -.2126f, 0, 0 },
            new float[] { -.7152f, -.7152f, -.7152f, 0, 0 },
            new float[] { -.0722f, -.0722f, -.0722f, 0, 0 },
            new float[] { 0, 0, 0, 1, 0 },
            new float[] { 1, 1, 1, 0, 1 }
        });

        bool IsNight()
        {
            // convert everything to TimeSpan
            TimeSpan start = new TimeSpan(21, 0, 0);
            TimeSpan end = new TimeSpan(07, 0, 0);
            TimeSpan now = DateTime.Now.TimeOfDay;
            // see if start comes before end
            if (start < end)
            {
                return start <= now && now <= end;
            }
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }

        ColorMatrix matrixDefault;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            double lat = gMapControl.FromLocalToLatLng(e.X, e.Y).Lat;
            double lng = gMapControl.FromLocalToLatLng(e.X, e.Y).Lng;
            label2.Text = "Mouse coord Lat: " + lat + " Lng: " + lng;
        }

        private void OnMapDrag()
        {
            label1.Text = "Map centeter Lat: " + gMapControl.Position.Lat + " Lng: " + gMapControl.Position.Lng;
        }
        private void OnZoomChanged()
        {
            label3.Text = "Map Zoom: " + gMapControl.Zoom;
            OnMapDrag();
        }

        private void labelHide_Click(object sender, EventArgs e)
        {
            _panel = !_panel;
            if (_panel)
            {
                ShowPanel();
            }
            else
            {
                HidePanel();
            }
        }

        private void ShowPanel()
        {
            labelHide.Text = "Приховати джерела";
            panel1.Width = _panelSize.Width;
            panel1.Height = _panelSize.Height;
        }

        private void HidePanel()
        {
            labelHide.Text = "Показати джерела";
            panel1.Width = 130;
            panel1.Height = 18;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Enabled = false;
            gMapControl.Enabled = false;
            DrawSource(_dataProvider.Providers[listBox1.SelectedIndex]);
            gMapControl.Enabled = true;
            listBox1.Enabled = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(_url);
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox.Enabled = false;
            gMapControl.MapProvider = GMapProviders.List[comboBox.SelectedIndex];
            comboBox.Enabled = true;
        }

        private void nightButton_Click(object sender, EventArgs e)
        {
            _isNightMode = !_isNightMode;
            SetMode(_isNightMode);
        }

        void SetMode(bool mode)
        {
            _currentMode = mode;
            if (mode)
            {
                gMapControl.EmptyMapBackground = Color.Black;
                gMapControl.ColorMatrix = matrixGray;
            }
            else
            {
                gMapControl.EmptyMapBackground = Color.Gray;
                gMapControl.ColorMatrix = matrixDefault;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _loader.Hide();
        }
    }
}
