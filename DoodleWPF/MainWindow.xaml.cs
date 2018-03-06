using DoodleModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace DoodleWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DTO_Users GV_User;
        public List<DTO_OpenDraws> GV_DrawList;
        public DTO_OpenDraws GV_Draw;
        public DTO_JoinDraw GV_Noodler;

        public MainWindow()
        {
            InitializeComponent();
            //TestWebService();
        }

        public sealed class S_Draw
        {
            private static readonly S_Draw instance = new S_Draw();

            private S_Draw() { }

            public static S_Draw Instance
            {
                get
                {
                    return instance;
                }
            }
            public int test;
        }

        private void TestWebService()
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();

            DTO_Login login = new DTO_Login();
            login.Email = "ROB@class.com";
            login.Password = "0007";
            login.Latitude = 399;
            login.Longitude = 100;

            var user = ws.Login(login);
            if (user != null)
                Debug.WriteLine(user.DisplayName);
            else
                Debug.WriteLine("Invalid login");

            var openDraws = ws.GatherOpenDraws();

            foreach( var od in openDraws)
            {
                Debug.WriteLine("{0} {1} {2}", od.Doodler, od.DrawCategoryName, od.StartTime);

            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            DTO_Users u = new DTO_Users();
            u.DisplayName = dn.Text;
            u.EmailAddress = ea.Text;
            u.Active = true;
            u.Password = "12";

            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            var retobject = ws.AddUser(u);
            u.ID = retobject.ID;

            output.Text = u.print();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void fetch_Click(object sender, RoutedEventArgs e)
        {
            int userid = Convert.ToInt32(id.Text);

            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            var name = ws.GetDoodleUser(userid);
            output.Text = name;

            var list = ws.GetAllUsers();

            lv.ItemsSource = list;
        }

        private void Load_list(object sender, RoutedEventArgs e)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();

            var list = ws.GetAllUsers();

            lv.ItemsSource = list;
        }

        //WEB SERVICE FUNCTIONS///////////////////////////////////////////////////////////////////////

        private DTO_Users WS_Login(DTO_Login login)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            DTO_Users user = ws.Login(login);
            if (user != null)
            {
                GV_User = user;
            }
            return user;
        }

        public void WS_GatherOpenDraws()
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            GV_DrawList = ws.GatherOpenDraws().ToList();
        }

        public DTO_JoinDraw WS_JoinDraw(DTO_JoinDraw join)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            join = ws.JoinDraw(join);
            if (join != null)
            {
                GV_Noodler = join;
            }
            return join;
        }

        public DTO_Users WS_RegisterUser(DTO_Users user)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            user = ws.AddUser(user);
            return user;
        }

        public DTO_OpenDraws WS_StartDraw(DTO_OpenDraws draw)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            draw = ws.StartDraw(draw);
            return draw;
        }

        public DTO_Guess WS_CheckGuess(DTO_Guess guess)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            guess = ws.CheckGuess(guess);
            return guess;
        }

        public void WS_SetWinner(DTO_Winner winner)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            ws.SetWinner(winner);
        }

        public void WS_EndDraw(DTO_DrawID drawid)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            drawid = ws.EndDraw(drawid);
        }

        private List<DTO_GameCategory> WS_GetDrawCategories()
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            List<DTO_GameCategory> list = ws.GetDrawCategories().ToList();
            return list;
        }

        private DTO_OpenDraws WS_CreateDraw(DTO_NewDraw draw)
        {
            ServiceReference1.Service1Client ws = new ServiceReference1.Service1Client();
            DTO_OpenDraws newdraw = new DTO_OpenDraws();
            newdraw = ws.CreateDraw(draw);
            return newdraw;
        }

        //UTILITY////////////////////////////////////////////////////////////////////////////////////

        private void CollapsePages()
        {
            output.Visibility = Visibility.Collapsed;
            Page_Login.Visibility = Visibility.Collapsed;
            Page_Homepage.Visibility = Visibility.Collapsed;
            Page_NLobby.Visibility = Visibility.Collapsed;
            Page_DLobby.Visibility = Visibility.Collapsed;
            Page_NGame.Visibility = Visibility.Collapsed;
            Page_Register.Visibility = Visibility.Collapsed;
        }

        //LOGIN PAGE///////////////////////////////////////////////////////////////////////////////////

        private void DrawPage_Login()
        {
            CollapsePages();
            TBox_LoginEmail.Text = "";
            TBox_LoginPassword.Text = "";
            Page_Login.Visibility = Visibility.Visible;
            output.Visibility = Visibility.Visible;
        }

        public void BTN_LoginRegister_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            DrawPage_Register();
        }

        private void BTN_LoginLogin_Click(object sender, RoutedEventArgs e)
        {
            if (TBox_LoginEmail.Text != "" && TBox_LoginPassword.Text != "")
            {
                //Validate that it's actually an email address

                DTO_Login login = new DTO_Login();
                login.Email = TBox_LoginEmail.Text;
                login.Password = TBox_LoginPassword.Text;

                //Figure out how to pull real location
                login.Latitude = 200;
                login.Longitude = 200;

                DTO_Users user = WS_Login(login);
                if (user != null)
                {
                    DrawPage_Home();
                }
                else
                {
                    DrawPage_Login();
                    output.Text = "Invalid Login";
                }
            }
            else
            {
                DrawPage_Login();
                if (TBox_LoginEmail.Text == "")
                {
                    TBox_LoginEmail.Text = "Required";
                }
                if (TBox_LoginPassword.Text == "")
                {
                    TBox_LoginPassword.Text = "Required";
                }
                output.Text = "Invalid Login";
            }   
        }
        
        //HOME PAGE////////////////////////////////////////////////////////////////////////////////////

        private void DrawPage_Home()
        {
            CollapsePages();
            Page_Homepage.Visibility = Visibility.Visible;
            TBlock_HomepageUser.Text = GV_User.DisplayName;
            WS_GatherOpenDraws();
            LV_HomePageDraws.ItemsSource = GV_DrawList;
        }

        public void BTN_HomepageNew_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_DLobby();
        }

        private void LV_HomePageDraws_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LV_HomePageDraws.SelectedIndex > -1)
            {
                GV_Draw = GV_DrawList[LV_HomePageDraws.SelectedIndex];
                DrawPage_NLobby();
            }
        }           

        //REGISTER PAGE/////////////////////////////////////////////////////////////////////////////////

        public void DrawPage_Register()
        {
            CollapsePages();
            TBox_RegisterEmail.Text = "";
            TBox_RegisterPassword.Text = "";
            TBox_RegisterUserName.Text = "";
            TBox_RegistrationPicture.Text = "";
            Page_Register.Visibility = Visibility.Visible;
        }

        public void BTN_RegisterBack_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            DrawPage_Login();
        }

        public void BTN_RegisterSubmit_Click(object sender, RoutedEventArgs e)
        {
            if(TBox_RegisterEmail.Text != "" && TBox_RegisterPassword.Text != "" 
                && TBox_RegistrationPicture.Text != "" && TBox_RegisterUserName.Text != "")
            {
                DTO_Users user = new DTO_Users();
                user.EmailAddress = TBox_RegisterEmail.Text;
                user.Password = TBox_RegisterPassword.Text;
                user.Picture = TBox_RegistrationPicture.Text;
                user.DisplayName = TBox_RegisterUserName.Text;

                user = WS_RegisterUser(user);
                if(user != null)
                {
                    DTO_Login login = new DTO_Login();
                    login.Email = user.EmailAddress;
                    login.Password = user.Password;
                    login.Latitude = 400;
                    login.Longitude = 200;

                    user = WS_Login(login);
                    if(user != null)
                    {
                        DrawPage_Home();
                    }    
                    else
                    {
                        DrawPage_Login();
                        output.Text = "Unable to Login";
                    }
                }
                else
                {
                    output.Visibility = Visibility.Visible;
                    output.Text = "Email Already In Use";
                }
            }
            else
            {
                if(TBox_RegisterEmail.Text == "")
                {
                    TBox_RegisterEmail.Text = "Required";
                }
                if(TBox_RegisterPassword.Text == "")
                {
                    TBox_RegisterPassword.Text = "Required";
                }
                if(TBox_RegistrationPicture.Text == "")
                {
                    TBox_RegistrationPicture.Text = "Required";
                }
                if(TBox_RegisterUserName.Text == "")
                {
                    TBox_RegisterUserName.Text = "Required";
                }
            }
        }

        //NOODLER LOBBY PAGE/////////////////////////////////////////////////////////////////////////////

        public void DrawPage_NLobby()
        {
            CollapsePages();
            Page_NLobby.Visibility = Visibility.Visible;
            TBlock_NLobbyDoodler.Text = GV_Draw.Doodler;
            TBlock_NLobbyCategory.Text = GV_Draw.DrawCategoryName;
        }

        private void BTN_NLobbyBack_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        private void BTN_NLobbyJoin_Click(object sender, RoutedEventArgs e)
        {
            DTO_JoinDraw join = new DTO_JoinDraw();
            join.DrawID = GV_Draw.DrawID;
            join.UserID = GV_User.ID;

            join = WS_JoinDraw(join);
            if (join != null)
            {
                DrawPage_NGame();
            }
        }

        //NOODLER GAME PAGE///////////////////////////////////////////////////////////////////////////////

        public void DrawPage_NGame()
        {
            CollapsePages();
            Page_NGame.Visibility = Visibility.Visible;
            TBox_NGameGuess.Text = "";
            TBlock_NGameCategory.Text = GV_Draw.DrawCategoryName;
        }

        private void BTN_NGameGuess_Click(object sender, RoutedEventArgs e)
        {
            DTO_Guess guess = new DTO_Guess();
            guess.DrawID = GV_Draw.DrawID;
            guess.Guess = TBox_NGameGuess.Text;

            guess = WS_CheckGuess(guess);
            if (guess.IsCorrect == true)
            {
                TBlock_NGameGuessTicker.Text = "The Winner is: " + GV_User.DisplayName;
                BTN_NGameHome.Visibility = Visibility.Visible;
                DTO_Winner winner = new DTO_Winner();
                winner.NoodlerID = GV_Noodler.NoodlerID;
                WS_SetWinner(winner);
                DTO_DrawID drawid = new DTO_DrawID();
                drawid.DrawID = GV_Draw.DrawID;
                WS_EndDraw(drawid);
                
            }
            else
            {
                TBlock_NGameGuessTicker.Text = TBox_NGameGuess.Text;
            }        
        }    

        private void BTN_NGameHome_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        //DOODLER LOBBY PAGE//////////////////////////////////////////////////////////////////////////////

        private void DrawPage_DLobby()
        {
            CollapsePages();
            CBox_GGameCategory.ItemsSource = WS_GetDrawCategories();
            CBox_GGameCategory.SelectedIndex = 0;
            Page_DLobby.Visibility = Visibility.Visible;
        }      

        private void BTN_DLobbyBack_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        private void BTN_DLobbySubmit_Click(object sender, RoutedEventArgs e)
        {
            //create draw
            DTO_NewDraw draw = new DTO_NewDraw();
            draw.DoodlerID = GV_User.ID;
            draw.Answer = "1";
            draw.DrawCategoryID = 1;

            DTO_OpenDraws newdraw = WS_CreateDraw(draw);
            GV_Draw = newdraw;

            /*
            while(GV_Draw.StartTime > System.DateTime.Now)
            {
                TBlock_GGameStartTimer.Text = (GV_Draw.StartTime - System.DateTime.Now).ToString();
            }
            */

            newdraw = WS_StartDraw(newdraw);
            DrawPage_DGame();
        }

        //DOODLER GAME PAGE////////////////////////////////////////////////////////////////////////////////////

        public void DrawPage_DGame()
        {
            CollapsePages();
            Page_DGame.Visibility = Visibility.Visible;
        }

        private void BTN_DGameQuit_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        private void BTN_DGameReturn_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }      

        
    }
}
