using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using DoodleModel;

namespace DoodleMobile
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
            InitializeComponent();

            

    }
        private void BTN_Test_Click(object sender, EventArgs e)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            L_Test.Text = ws.GetDoodleUserAsync(1).ToString();
        }
    }
}
