using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Forms;

namespace AiAssistant
{
    public partial class ProcessingWindow : Window
    {
        private const int WINDOW_MARGIN_PX = 20;

        public ProcessingWindow()
        {
            InitializeComponent();
            PositionWindowBottomRight();
        }

        public void UpdateStatus(string status)
        {
            StatusTextBlock.Text = status;
        }

        private void PositionWindowBottomRight()
        {
            var cursorPosition = System.Windows.Forms.Cursor.Position;
            var currentScreen = Screen.FromPoint(cursorPosition);
            var workingArea = currentScreen.WorkingArea;
            
            Left = workingArea.Right - Width - WINDOW_MARGIN_PX;
            Top = workingArea.Bottom;
            Opacity = 0;

            var finalTop = workingArea.Bottom - Height - WINDOW_MARGIN_PX;
            Loaded += (s, e) => AnimatePopUp(finalTop);
        }

        private void AnimatePopUp(double finalTop)
        {
            var slideAnimation = new DoubleAnimation
            {
                From = Top,
                To = finalTop,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 },
            };

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(300),
            };

            BeginAnimation(TopProperty, slideAnimation);
            BeginAnimation(OpacityProperty, fadeAnimation);
        }
    }
}
