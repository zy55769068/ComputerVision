using System.ComponentModel;
using ComputerVision.ViewModels;
using Xamarin.Forms;

namespace ComputerVision
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private MainViewModel mainviewModel = new MainViewModel();
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = mainviewModel;
        }
    }
}
