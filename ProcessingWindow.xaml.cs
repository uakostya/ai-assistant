using System.Windows;

namespace AiAssistant
{
    public partial class ProcessingWindow : Window
    {
        public ProcessingWindow()
      {
  InitializeComponent();
        }

  public void UpdateStatus(string status)
        {
     StatusTextBlock.Text = status;
  }
    }
}
