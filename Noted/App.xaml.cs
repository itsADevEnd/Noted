using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Noted
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = Noted.MainPage.AppMainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
