using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;

namespace DemoMap.Cache
{

    public class MapCacheForm : Form
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();

        private List<GPoint> _list;

        private int _zoom;

        private GMapProvider _provider;

        private int _sleep;

        private int _all;

        public bool ShowCompleteMessage;

        private RectLatLng _area;

        private GSize _maxOfTiles;

        public GMapOverlay Overlay;

        private int _retry;

        private readonly AutoResetEvent _done = new AutoResetEvent(initialState: true);

        public readonly Queue<GPoint> CachedTiles = new Queue<GPoint>();

        private IContainer components;

        private Label label1;

        private TableLayoutPanel tableLayoutPanel1;

        private ProgressBar progressBarDownload;

        private Label label2;
        public Action OnFinish;

        public MapCacheForm()
        {
            InitializeComponent();
            GMaps instance = GMaps.Instance;
            instance.OnTileCacheComplete = (TileCacheComplete)Delegate.Combine(instance.OnTileCacheComplete, new TileCacheComplete(OnTileCacheComplete));
            GMaps instance2 = GMaps.Instance;
            instance2.OnTileCacheStart = (TileCacheStart)Delegate.Combine(instance2.OnTileCacheStart, new TileCacheStart(OnTileCacheStart));
            GMaps instance3 = GMaps.Instance;
            instance3.OnTileCacheProgress = (TileCacheProgress)Delegate.Combine(instance3.OnTileCacheProgress, new TileCacheProgress(OnTileCacheProgress));
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        private void OnTileCacheComplete()
        {
            if (!base.IsDisposed)
            {
                MethodInvoker method = delegate
                {
                    label2.Text = "all tiles saved";
                    Thread.Sleep(123);
                    Close();
                };
                Invoke(method);
            }
        }

        private void OnTileCacheStart()
        {
            if (!base.IsDisposed)
            {
                _done.Reset();
                MethodInvoker method = delegate
                {
                    label2.Text = "saving tiles...";
                };
                Invoke(method);
            }
        }

        private void OnTileCacheProgress(int left)
        {
            if (!base.IsDisposed)
            {
                MethodInvoker method = delegate
                {
                    label2.Text = left + " tile to save...";
                };
                Invoke(method);
            }
        }

        public void Start(RectLatLng area, int zoom, GMapProvider provider, int sleep, int retry)
        {
            if (!worker.IsBusy)
            {
                label1.Text = "...";
                progressBarDownload.Value = 0;
                _area = area;
                _zoom = zoom;
                _provider = provider;
                _sleep = sleep;
                _retry = retry;
                GMaps.Instance.UseMemoryCache = false;
                GMaps.Instance.CacheOnIdleRead = false;
                GMaps.Instance.BoostCacheEngine = true;
                Overlay?.Markers.Clear();

                worker.RunWorkerAsync();
                Show();
            }
        }

        public void Stop()
        {
            GMaps instance = GMaps.Instance;
            instance.OnTileCacheComplete = (TileCacheComplete)Delegate.Remove(instance.OnTileCacheComplete, new TileCacheComplete(OnTileCacheComplete));
            GMaps instance2 = GMaps.Instance;
            instance2.OnTileCacheStart = (TileCacheStart)Delegate.Remove(instance2.OnTileCacheStart, new TileCacheStart(OnTileCacheStart));
            GMaps instance3 = GMaps.Instance;
            instance3.OnTileCacheProgress = (TileCacheProgress)Delegate.Remove(instance3.OnTileCacheProgress, new TileCacheProgress(OnTileCacheProgress));
            _done.Set();
            if (worker.IsBusy)
            {
                worker.CancelAsync();
            }

            GMaps.Instance.CancelTileCaching();
            _done.Close();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ShowCompleteMessage)
            {
                if (!e.Cancelled)
                {
                    MessageBox.Show(this, "Prefetch Complete! => " + (int)e.Result + " of " + _all);
                }
                else
                {
                    MessageBox.Show(this, "Prefetch Canceled! => " + (int)e.Result + " of " + _all);
                }
            }
            OnFinish?.Invoke();
            _list.Clear();
            GMaps.Instance.UseMemoryCache = true;
            GMaps.Instance.CacheOnIdleRead = true;
            GMaps.Instance.BoostCacheEngine = false;
            worker.Dispose();
            Close();
        }

        private bool CacheTiles(int zoom, GPoint p)
        {
            GMapProvider[] overlays = _provider.Overlays;
            foreach (GMapProvider gMapProvider in overlays)
            {
                PureImage pureImage = ((!gMapProvider.InvertedAxisY) ? 
                    GMaps.Instance.GetImageFrom(gMapProvider, p, zoom, out _) : 
                    GMaps.Instance.GetImageFrom(gMapProvider, new GPoint(p.X, _maxOfTiles.Height - p.Y), zoom, out _));
                if (pureImage != null)
                {
                    pureImage.Dispose();
                    continue;
                }

                return false;
            }

            return true;
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_list != null)
            {
                _list.Clear();
                _list = null;
            }

            _list = _provider.Projection.GetAreaTileList(_area, _zoom, 0);
            _maxOfTiles = _provider.Projection.GetTileMatrixMaxXY(_zoom);
            _all = _list.Count;
            int num = 0;
            int num2 = 0;

            lock (this)
            {
                CachedTiles.Clear();
            }

            for (int i = 0; i < _all && !worker.CancellationPending; i++)
            {
                GPoint gPoint = _list[i];
                if (CacheTiles(_zoom, gPoint))
                {
                    if (Overlay != null)
                    {
                        lock (this)
                        {
                            CachedTiles.Enqueue(gPoint);
                        }
                    }

                    num++;
                    num2 = 0;
                }
                else
                {
                    if (++num2 <= _retry)
                    {
                        i--;
                        Thread.Sleep(1111);
                        continue;
                    }

                    num2 = 0;
                }

                worker.ReportProgress((i + 1) * 100 / _all, i + 1);
                if (_sleep > 0)
                {
                    Thread.Sleep(_sleep);
                }
            }

            e.Result = num;
            if (!base.IsDisposed)
            {
                _done.WaitOne();
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = "Fetching tile at zoom (" + _zoom + "): " + (int)e.UserState + " of " + _all + ", complete: " + e.ProgressPercentage + "%";
            progressBarDownload.Value = e.ProgressPercentage;
            if (Overlay == null)
            {
                return;
            }

            GPoint? gPoint = null;
            lock (this)
            {
                if (CachedTiles.Count > 0)
                {
                    gPoint = CachedTiles.Dequeue();
                }
            }

            if (gPoint.HasValue)
            {
                GPoint p = Overlay.Control.MapProvider.Projection.FromTileXYToPixel(gPoint.Value);
                PointLatLng p2 = Overlay.Control.MapProvider.Projection.FromPixelToLatLng(p, _zoom);
                double groundResolution = Overlay.Control.MapProvider.Projection.GetGroundResolution(_zoom, p2.Lat);
                double num = Overlay.Control.MapProvider.Projection.GetGroundResolution((int)Overlay.Control.Zoom, p2.Lat) / groundResolution;
                GMapMarkerTileMy item = new GMapMarkerTileMy(p2, (int)((double)Overlay.Control.MapProvider.Projection.TileSize.Width / num));
                Overlay.Markers.Add(item);
            }
        }

        private void Prefetch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void Prefetch_FormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        internal class GMapMarkerTileMy : GMapMarker
        {
            private static Brush Fill = new SolidBrush(Color.FromArgb(155, Color.Blue));

            public GMapMarkerTileMy(PointLatLng p, int size)
                : base(p)
            {
                base.Size = new Size(size, size);
            }

            public override void OnRender(Graphics g)
            {
                g.FillRectangle(Fill, new Rectangle(base.LocalPosition.X, base.LocalPosition.Y, base.Size.Width, base.Size.Height));
            }
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBarDownload = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(406, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel1.Controls.Add(this.progressBarDownload, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(539, 75);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // progressBarDownload
            // 
            this.progressBarDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBarDownload.Location = new System.Drawing.Point(4, 20);
            this.progressBarDownload.Margin = new System.Windows.Forms.Padding(4);
            this.progressBarDownload.Name = "progressBarDownload";
            this.progressBarDownload.Size = new System.Drawing.Size(406, 51);
            this.progressBarDownload.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarDownload.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(418, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 59);
            this.label2.TabIndex = 2;
            this.label2.Text = "please wait...";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TilePrefetcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AliceBlue;
            this.ClientSize = new System.Drawing.Size(549, 85);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TilePrefetcher";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GMap.NET - esc to cancel fetching";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Prefetch_FormClosed);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Prefetch_PreviewKeyDown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}