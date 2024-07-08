using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenCVDemo.Components
{
    public partial class Labeler : UserControl
    {
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
        "Left", typeof(double),
        typeof(Labeler));

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
        "Top", typeof(double),
        typeof(Labeler));

        public static readonly DependencyProperty CanvasWidthProperty = DependencyProperty.Register(
        "CanvasWidth", typeof(double),
        typeof(Labeler), new PropertyMetadata { PropertyChangedCallback = OnUpdateCanvasWidth });

        public static readonly DependencyProperty CanvasHeightProperty = DependencyProperty.Register(
        "CanvasHeight", typeof(double),
        typeof(Labeler), new PropertyMetadata { PropertyChangedCallback = OnUpdateCanvasHeight });

        public static readonly DependencyProperty ImageRectProperty = DependencyProperty.Register(
        "ImageRect", typeof(Rect),
        typeof(Labeler), new PropertyMetadata() { PropertyChangedCallback = OnUpdateImageRect });

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        "Command", typeof(ICommand),
        typeof(Labeler));

        private bool isDragging = false;

        private static void OnUpdateCanvasHeight(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double canvasHeight = (double)e.NewValue;
            Labeler labeler = (Labeler)d;
            labeler.Left = labeler.ImageRect.Left * labeler.CanvasWidth;
            labeler.Top = labeler.ImageRect.Top * canvasHeight;
            labeler.Width = labeler.ImageRect.Width * labeler.CanvasWidth;
            labeler.Height = labeler.ImageRect.Height * canvasHeight;
        }

        private static void OnUpdateCanvasWidth(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            double canvasWidth = (double)e.NewValue;
            Labeler labeler = (Labeler)d;
            labeler.Left = labeler.ImageRect.Left * canvasWidth;
            labeler.Top = labeler.ImageRect.Top * labeler.CanvasHeight;
            labeler.Width = labeler.ImageRect.Width * canvasWidth;
            labeler.Height = labeler.ImageRect.Height * labeler.CanvasHeight;
        }


        private static void OnUpdateImageRect(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Rect rect = (Rect)e.NewValue;
            Labeler labeler = (Labeler)d;
            labeler.Left = rect.Left * labeler.CanvasWidth;
            labeler.Top = rect.Top * labeler.CanvasHeight;
            labeler.Width = rect.Width * labeler.CanvasWidth;
            labeler.Height = rect.Height * labeler.CanvasHeight;
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public double Top
        {
            get => (double)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public double CanvasWidth
        {
            get => (double)GetValue(CanvasWidthProperty);
            set => SetValue(CanvasWidthProperty, value);
        }

        public double CanvasHeight
        {
            get => (double)GetValue(CanvasHeightProperty);
            set => SetValue(CanvasHeightProperty, value);
        }

        public Rect ImageRect
        {
            get => (Rect)GetValue(ImageRectProperty);
            set => SetValue(ImageRectProperty, value);
        }

        System.Windows.Point dragStartPosition = new(0, 0);
        Rectangle dragStartRectangle;

        bool moveWidth = false;
        bool moveHeight = false;

        public Labeler()
        {
            InitializeComponent();
            Application.Current.MainWindow.PreviewMouseUp += StopDragging;
        }

        private void NW_StartDragging(object sender, MouseButtonEventArgs e)
        {
            moveHeight = false;
            moveWidth = false;
            StartDragging(sender, e);
        }

        private void NE_StartDragging(object sender, MouseButtonEventArgs e)
        {
            moveHeight = false;
            moveWidth = true;
            StartDragging(sender, e);
        }

        private void SW_StartDragging(object sender, MouseButtonEventArgs e)
        {
            moveHeight = true;
            moveWidth = false;
            StartDragging(sender, e);
        }

        private void SE_StartDragging(object sender, MouseButtonEventArgs e)
        {
            moveHeight = true;
            moveWidth = true;
            StartDragging(sender, e);
        }

        private void StartDragging(object sender, MouseButtonEventArgs e)
        {
            dragStartPosition = Mouse.GetPosition(Application.Current.MainWindow);
            dragStartRectangle = new Rectangle(Convert.ToInt32(Left), Convert.ToInt32(Top), Convert.ToInt32(Width), Convert.ToInt32(Height));
            Application.Current.MainWindow.MouseMove += Drag;
            isDragging = true;
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            var currentPosition = Mouse.GetPosition(Application.Current.MainWindow);
            if (moveWidth)
            {
                Width = Math.Max(40, dragStartRectangle.Width + currentPosition.X - dragStartPosition.X);
            }
            else
            {
                var left = dragStartRectangle.Left + currentPosition.X - dragStartPosition.X;
                var width = dragStartRectangle.Width + dragStartRectangle.Left - Left;
                if (width < 40)
                {
                    left -= 40 - width;
                    width = 40;
                }

                Left = left;
                Width = width;
            }

            if (moveHeight)
            {
                Height = Math.Max(40, dragStartRectangle.Height + currentPosition.Y - dragStartPosition.Y);
            }
            else
            {
                var top = dragStartRectangle.Top + currentPosition.Y - dragStartPosition.Y;
                var height = dragStartRectangle.Height + dragStartRectangle.Top - Top;
                if (height < 40)
                {
                    top -= 40 - height;
                    height = 40;
                }

                Top = top;
                Height = height;
            }
        }

        private void StopDragging(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                Rect rect = new Rect(Left / CanvasWidth, Top / CanvasHeight, ActualWidth / CanvasWidth, ActualHeight / CanvasHeight);
                if (Command?.CanExecute(rect) == true)
                {
                    Command.Execute(rect);
                }
            }

            Application.Current.MainWindow.MouseMove -= Drag;
        }
    }
}
