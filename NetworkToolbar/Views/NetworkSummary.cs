using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NetworkToolbar.VM;

namespace NetworkToolbar.Views
{
    public partial class NetworkSummary : FrameworkElement
    {
        public static readonly DependencyProperty NetworkFramesProperty = DependencyProperty.Register(
            nameof(NetworkFrames), typeof(IEnumerable<NetworkFrame>), typeof(NetworkSummary),
            new FrameworkPropertyMetadata(default(IEnumerable<NetworkFrame>), FrameworkPropertyMetadataOptions.AffectsRender, OnFramesChanged));

        private static void OnFramesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NetworkSummary)?.InvalidateVisual();
        }

        public IEnumerable<NetworkFrame> NetworkFrames
        {
            get { return (IEnumerable<NetworkFrame>) GetValue(NetworkFramesProperty); }
            set { SetValue(NetworkFramesProperty, value); }
        }

        public static readonly DependencyProperty Property = DependencyProperty.Register(
            nameof(DownloadText), typeof(string), typeof(NetworkSummary),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

        public string DownloadText
        {
            get { return (string) GetValue(Property); }
            set { SetValue(Property, value); }
        }

        public static readonly DependencyProperty UploadTextProperty = DependencyProperty.Register(
            "UploadText", typeof(string), typeof(NetworkSummary),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

        public string UploadText
        {
            get { return (string) GetValue(UploadTextProperty); }
            set { SetValue(UploadTextProperty, value); }
        }

        private Brush m_boarderBrush;
        private Pen m_boarderPen;
        private Brush m_textBackgroundBrush;
        private Brush m_backgroundBrush;
        private Pen m_backgroundPen;

        private Brush m_downloadBrush;
        private Pen m_downloadPen;
        private Brush m_uploadBrush;
        private Pen m_uploadPen;
        private Brush m_mixedBrush;
        private Pen m_mixedPen;
        private Typeface m_font;
        private readonly double m_textHeight;

        public NetworkSummary()
        {
            //m_font = SystemFonts.MenuFontFamily.GetTypefaces().First();
            m_font = new Typeface("Segoe UI");
            m_textHeight = new FormattedText("P", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, m_font, EmFontSize, Brushes.Black).Height;

            m_boarderBrush = new SolidColorBrush(Color.FromRgb(166, 166, 166));
            m_boarderPen = new Pen(m_boarderBrush, 1);

            m_textBackgroundBrush = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            m_backgroundBrush = new SolidColorBrush(Colors.White);
            m_backgroundPen = new Pen(m_backgroundBrush, 1);

            m_downloadBrush = Brushes.Green;
            m_downloadPen = new Pen(m_downloadBrush, 1);
            m_uploadBrush = Brushes.Firebrick;
            m_uploadPen = new Pen(m_uploadBrush, 1);
            m_mixedBrush = Brushes.Orange;
            m_mixedPen = new Pen(m_mixedBrush, 1);
        }

        private double EmFontSize = 10.5;

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            size.Height = Math.Max(size.Height, (m_textHeight * 2));
            return size;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // background
            RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
            Rect rect = new Rect(1, 1, ActualWidth - 1, ActualHeight - 1);
            drawingContext.DrawRectangle(m_backgroundBrush, m_boarderPen, rect);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            DrawGraph(drawingContext);
            DrawTextSummary(drawingContext);
        }

        /// <summary>
        /// draws the main graph section
        /// </summary>
        private void DrawGraph(DrawingContext drawingContext)
        {
            if(!NetworkFrames.Any())
                return;

            RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
            int x = (int) ActualWidth - 1;
            double yMin = ActualHeight;
            double yRange = yMin - 3;
            NetworkFrame[] frames = NetworkFrames.Take(x).ToArray();
            double range = frames.SelectMany(f => new[] {f.Download, f.Upload}).Max();

            for (int i = frames.Length - 1; i >= 0; i--)
            {
                double barRange = frames[i].Download / range;
                drawingContext.DrawLine(m_downloadPen, new Point(x, yMin), new Point(x, yMin - (yRange * barRange)));
                barRange = frames[i].Upload / range;
                drawingContext.DrawLine(m_uploadPen, new Point(x, yMin), new Point(x, yMin - (yRange * barRange)));

                x -= 1;
            }

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        }

        /// <summary>
        /// Draws text summary in the top left corner of the graph
        /// </summary>
        private void DrawTextSummary(DrawingContext drawingContext)
        {
            FormattedText dText = new FormattedText(
                $"D {DownloadText}", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, m_font, EmFontSize, m_downloadBrush);
            FormattedText uText = new FormattedText(
                $"U {UploadText}", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, m_font, EmFontSize, m_uploadBrush);

            Rect textSize = new Rect(1, 1, Math.Max(dText.Width, uText.Width) + 3, dText.Height + uText.Height - 4);

            RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
            drawingContext.DrawRectangle(m_textBackgroundBrush, m_boarderPen, textSize);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            drawingContext.DrawText(dText, new Point(1, 0));
            drawingContext.DrawText(uText, new Point(1, dText.Height - 3));
        }
    }
}