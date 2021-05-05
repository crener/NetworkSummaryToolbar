using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using System.Xml.Serialization;
using NetworkToolbar.Utility;
using NetworkToolbar.VM;
using NetworkToolbar.VM.Container;

namespace NetworkToolbar.Views
{
    public class NetworkSummary : FrameworkElement
    {
        public const double AbsoluteMinWidth = 60; 
        public const double AbsoluteMinHeight = 28; 
        
        public IReadOnlyList<NetworkFrame> NetworkFrames
        {
            get { return (IReadOnlyList<NetworkFrame>) GetValue(NetworkFramesProperty); }
            set { SetValue(NetworkFramesProperty, value); }
        }
        public static readonly DependencyProperty NetworkFramesProperty = DependencyProperty.Register(
            nameof(NetworkFrames), typeof(IReadOnlyList<NetworkFrame>), typeof(NetworkSummary),
            new FrameworkPropertyMetadata(default(IReadOnlyList<NetworkFrame>), FrameworkPropertyMetadataOptions.AffectsRender, OnFramesChanged));

        private static void OnFramesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as NetworkSummary)?.InvalidateVisual();
        }

        public string DownloadText
        {
            get { return (string) GetValue(Property); }
            set { SetValue(Property, value); }
        }
        public static readonly DependencyProperty Property = DependencyProperty.Register(
            nameof(DownloadText), typeof(string), typeof(NetworkSummary),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

        public string UploadText
        {
            get { return (string) GetValue(UploadTextProperty); }
            set { SetValue(UploadTextProperty, value); }
        }
        public static readonly DependencyProperty UploadTextProperty = DependencyProperty.Register(
            nameof(UploadText), typeof(string), typeof(NetworkSummary),
            new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsRender));

        public RenderingMode RenderMode
        {
            get { return (RenderingMode) GetValue(RenderModeProperty); }
            set { SetValue(RenderModeProperty, value); }
        }
        public static readonly DependencyProperty RenderModeProperty = DependencyProperty.Register(
            nameof(RenderMode), typeof(RenderingMode), typeof(NetworkSummary), 
            new PropertyMetadata(RenderingMode.Average));

        public int DesiredNetworkBufferSize
        {
            get { return (int) GetValue(DesiredNetworkBufferSizeProperty); }
            set { SetValue(DesiredNetworkBufferSizeProperty, value); }
        }
        public static readonly DependencyProperty DesiredNetworkBufferSizeProperty = DependencyProperty.Register(
            "DesiredNetworkBufferSize", typeof(int), typeof(NetworkSummary), new PropertyMetadata(default(int)));

        
        private const double EmFontSize = 10.5;
        
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
            m_backgroundBrush = new SolidColorBrush(Color.FromRgb(237, 237, 237));
            m_backgroundPen = new Pen(m_backgroundBrush, 1);

            m_downloadBrush = Brushes.Green;
            m_downloadPen = new Pen(m_downloadBrush, 1);
            m_uploadBrush = Brushes.Firebrick;
            m_uploadPen = new Pen(m_uploadBrush, 1);
            m_mixedBrush = Brushes.Orange;
            m_mixedPen = new Pen(m_mixedBrush, 1);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = base.MeasureOverride(constraint);
            size.Height = Math.Max(size.Height, (m_textHeight * 2));
            return size;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if(ActualWidth < AbsoluteMinWidth || ActualHeight < AbsoluteMinHeight) return;
            
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
            if(NetworkFrames.Count <= 1)
                return;

            RenderOptions.SetEdgeMode(this, EdgeMode.Unspecified);
            int x = (int) ActualWidth - 1;
            double yMin = ActualHeight-1;
            double yRange = yMin - 3;
            DesiredNetworkBufferSize = x;

            NetworkFrame[] frames = NetworkFrames.Take(x).ToArray();
            double[] up = new double[Math.Min(x, frames.Length)];
            double[] down = new double[Math.Min(x, frames.Length)];
            double range = 0;
 
            const int maxedAveragingRange = 3; 
            const int rollingAveragingRange = 2;
            const int movingAveragingRange = 5;
            if(RenderMode == RenderingMode.Average && up.Length > rollingAveragingRange)
            {
                int i = 0;
                DropOutStack<double> upStack = new DropOutStack<double>(rollingAveragingRange);
                DropOutStack<double> downStack = new DropOutStack<double>(rollingAveragingRange);
                foreach (NetworkFrame frame in frames)
                {
                    if(i > up.Length) break;

                    upStack.Push(frame.Upload);
                    downStack.Push(frame.Download);
                    up[i] = Math.Max(frame.Upload, upStack.Average());
                    down[i] = Math.Max(frame.Download, downStack.Average());

                    range = Math.Max(up[i], Math.Max(down[i], range));
                    i++;
                }
            }
            else if(RenderMode == RenderingMode.AverageMoving && up.Length > movingAveragingRange)
            {
                int i = 0;
                DropOutStack<double> upStack = new DropOutStack<double>(movingAveragingRange);
                DropOutStack<double> downStack = new DropOutStack<double>(movingAveragingRange);
                foreach (NetworkFrame frame in frames)
                {
                    if(i > up.Length) break;

                    upStack.Push(frame.Upload);
                    downStack.Push(frame.Download);
                    up[i] = Math.Max(frame.Upload, CalculateMovingAverage(upStack));
                    down[i] = Math.Max(frame.Download, CalculateMovingAverage(downStack));

                    range = Math.Max(up[i], Math.Max(down[i], range));
                    i++;
                }
            }
            else if(RenderMode == RenderingMode.Thick && up.Length > maxedAveragingRange)
            {
                int i = 0;
                DropOutStack<double> upStack = new DropOutStack<double>(maxedAveragingRange);
                DropOutStack<double> downStack = new DropOutStack<double>(maxedAveragingRange);
                foreach (NetworkFrame frame in frames)
                {
                    if(i >= up.Length) break;

                    upStack.Push(frame.Upload);
                    downStack.Push(frame.Download);
                    up[i] = upStack.Max();
                    down[i] = downStack.Max();

                    range = Math.Max(up[i], Math.Max(down[i], range));
                    i++;
                }
            }
            else // RenderMode == RenderingMode.Direct
            {
                int i = 0;
                foreach (NetworkFrame frame in frames)
                {
                    if(i > up.Length) break;

                    up[i] = frame.Upload;
                    down[i] = frame.Download;

                    range = Math.Max(up[i], Math.Max(down[i], range));
                    i++;
                }
            }

            for (int i = 0; i < up.Length-1; i++)
            {
                double barRange = down[i] / range;
                drawingContext.DrawLine(m_downloadPen, new Point(x, yMin), new Point(x, yMin - (yRange * barRange)));

                barRange = up[i] / range;
                drawingContext.DrawLine(m_uploadPen, new Point(x, yMin), new Point(x, yMin - (yRange * barRange)));

                x -= 1;
            }

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        }

        private double CalculateMovingAverage(DropOutStack<double> stack)
        {
            int weight = stack.Capacity;
            double total = 0;
            int size = 0;

            foreach (double item in stack)
            {
                total += item * weight;
                size += weight;
                weight--;
            }

            return total / size;
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