﻿using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Panuon.UI
{
    /// <summary>
    /// PUImageCuter.xaml 的交互逻辑
    /// </summary>
    public partial class PUImageCuter : UserControl
    {
        private double _width;
        private double _height;

        private double _left;
        private double _top;

        public PUImageCuter()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
     
        #region Porperty

        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(PUImageCuter), new PropertyMetadata(OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cuter = d as PUImageCuter;
            cuter.grdImage.Width = Double.NaN;
            cuter.grdImage.Height = Double.NaN;

            if (cuter.ImageSource == null)
                cuter.resizeGrid.Visibility = Visibility.Collapsed;
            else
                cuter.resizeGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 剪切后的图片。
        /// </summary>
        public Bitmap CutImageSource
        {
            get
            {
                if (ImageSource == null)
                    return null;
                using (MemoryStream outStream = new MemoryStream())
                {
                    var widthScale = ImageSource.PixelWidth / _width;
                    var heightScale = ImageSource.PixelHeight / _height;

                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(ImageSource));
                    enc.Save(outStream);
                    Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                    Rectangle section = new Rectangle(new System.Drawing.Point((int)(_left * widthScale), (int)(_top * heightScale)), new System.Drawing.Size((int)(resizeGrid.ActualWidth * widthScale), (int)(resizeGrid.ActualHeight * heightScale)));
                    Bitmap bmp = new Bitmap(section.Width, section.Height);

                    Graphics g = Graphics.FromImage(bmp);

                    g.DrawImage(bitmap, 0, 0, section, GraphicsUnit.Pixel);
                    return bmp;
                }
            }
        }


        /// <summary>
        /// 裁剪区域形状。默认为矩形。
        /// </summary>
        public AreaStyles AreaStyle
        {
            get { return (AreaStyles)GetValue(AreaStyleProperty); }
            set { SetValue(AreaStyleProperty, value); }
        }

        public static readonly DependencyProperty AreaStyleProperty =
            DependencyProperty.Register("AreaStyle", typeof(AreaStyles), typeof(PUImageCuter), new PropertyMetadata(AreaStyles.Rectangle));


        #endregion



        public enum AreaStyles
        {
            /// <summary>
            /// 矩形区域。
            /// </summary>
            Rectangle,
            /// <summary>
            /// 正方形区域。
            /// </summary>
            Square,
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            _left = Canvas.GetLeft(resizeGrid) + e.HorizontalChange;
            _top = Canvas.GetTop(resizeGrid) + e.VerticalChange;
            if (_left < 0)
                _left = 0;
            else if (_left > canvas.Width - resizeGrid.ActualWidth)
                _left = canvas.Width - resizeGrid.ActualWidth;
            if (_top < 0)
                _top = 0;
            else if (_top > canvas.Height - resizeGrid.ActualHeight)
                _top = canvas.Height - resizeGrid.ActualHeight;

            Canvas.SetLeft(resizeGrid, _left);
            Canvas.SetTop(resizeGrid, _top);
        }

        private void img_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _width = img.ActualWidth;
            _height =img.ActualHeight;
            grdImage.Width = _width;
            grdImage.Height = _height;
            canvas.Height = _height;
            canvas.Width = _width;
            resizeGrid.MaxWidth = _width;
            resizeGrid.MaxHeight = _height;
        }

        private void resizeGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                var top = _top + resizeGrid.ActualHeight;
                if (top > _height)
                    resizeGrid.Height = _height - _top;
                if(AreaStyle == AreaStyles.Square)
                {
                    if (resizeGrid.Height > resizeGrid.MaxWidth)
                        resizeGrid.Height = resizeGrid.MaxWidth;
                    resizeGrid.Width = resizeGrid.Height;
                    return;
                }
            }
            if(e.WidthChanged)
            {
                var left = _left + resizeGrid.ActualWidth;
                if (left > _width)
                    resizeGrid.Width = _width - _left;

                if (AreaStyle == AreaStyles.Square)
                {
                    if (resizeGrid.Width > resizeGrid.MaxHeight)
                        resizeGrid.Width = resizeGrid.MaxHeight;
                    resizeGrid.Height = resizeGrid.Width;
                }
            }

        }
    }
}
