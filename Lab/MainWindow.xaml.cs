using DoodleModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
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
using System.Net.Http.Headers;
using System.Windows.Threading;

namespace Lab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string URL = @"http://www.michaelbleggett.com/doodle/Service1.svc/";
        HttpClient client;
        Point previousPoint;

        public MainWindow()
        {
            InitializeComponent();
            StartFetchingData();          
        }

        //SINGLETON GLOBAL VARIABLES///////////////////////////////////////////////////////////////

        public sealed class S_User
        {
            private static readonly S_User instance = new S_User();

            private S_User() { }

            public static S_User Instance
            {
                get
                {
                    return instance;
                }
            }
            public int userID;
            public string displayName;
            public string emailAddress;
            public string picture;
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
            public int drawID;
            public int doodleUserID;
            public string doodlerName;
            public int categoryID;
            public string drawCategoryName;
            public DateTime? startTime;
            public int drawStatusID;
            public string drawStatusDesc;
        }

        public sealed class S_DrawList
        {
            private static readonly S_DrawList instance = new S_DrawList();

            private S_DrawList() { }

            public static S_DrawList Instance
            {
                get
                {
                    return instance;
                }
            }
            public List<DTO_OpenDraws> drawList;
        }

        public sealed class S_Noodler
        {
            private static readonly S_Noodler instance = new S_Noodler();

            private S_Noodler() { }

            public static S_Noodler Instance
            {
                get
                {
                    return instance;
                }
            }
            public int drawNoodlerID;
        }

        //CLASS EXAMPLE FUNCTIONS///////////////////////////////////////////////////////////////

        private void StartFetchingData()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0, 5, 0);
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            (sender as DispatcherTimer).Stop();

            await PullLines();

            lastFetch.Text = string.Format("{0}", DateTime.Now);

            (sender as DispatcherTimer).Start();
        }

        public async Task GetLineCount()
        {
            try
            {
                DTO_DrawID temp = new DTO_DrawID();
                temp.DrawID = S_Draw.Instance.drawID;
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "GetLineCount"), temp);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_LineCount>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_LineCount>));
                var countList = des.Data.ToList();

                if (countList.Count > 0)
                {
                    lineCount.Text = countList.First().lineCount.ToString();
                }
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
        }

        private async Task PostLine(Point p1, Point p2)
        {
            DTO_DrawPoints line = new DTO_DrawPoints();
            line.DrawID = S_Draw.Instance.drawID;
            line.DrawPointX = p1.X;
            line.DrawPointY = p1.Y;
            line.DrawPointX2 = p2.X;
            line.DrawPointY2 = p2.Y;

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "PushLine"), line);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            await PullLines();
            await GetLineCount();
        }

        private async void Push_Click(object sender, RoutedEventArgs e)
        {
            var p1 = new Point(10, 20);
            var p2 = new Point(20, 30);

            await PostLine(p1, p2);
        }

        private async void Pull_Click(object sender, RoutedEventArgs e)
        {
            await PullLines();
        }

        private async Task PullLines()
        {
            DTO_DrawID temp = new DTO_DrawID();
            temp.DrawID = S_Draw.Instance.drawID;
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "GetDrawPoints"), temp);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_DrawPoints>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_DrawPoints>));
                var pointsList = des.Data.ToList();
                listviewLines.ItemsSource = pointsList;

                await GetLineCount();

                Noodlerplot.Children.Clear();

                int x = 0;
                foreach( var p in pointsList)
                {
                    Line line = new Line();
                    line.X1 = p.DrawPointX;
                    line.Y1 = p.DrawPointY;
                    line.X2 = p.DrawPointX2;
                    line.Y2 = p.DrawPointY2;
                    line.Stroke = Brushes.LightSteelBlue;
                    line.StrokeThickness = 2;

                    x++;
                    if (x % 2 == 0)
                    {
                        line.Stroke = Brushes.Crimson;
                    }
                    Noodlerplot.Children.Add(line);
                }

                if( pointsList.Count > 0)
                {
                    Point pt = new Point();
                    var last = pointsList.Last();

                    pt.X = last.DrawPointX2;
                    pt.Y = last.DrawPointY2;

                    previousPoint = pt;
                }
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
        }

        private async void plot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = Mouse.GetPosition(plot);
            if (previousPoint.X == 0)
            {
                previousPoint = new Point();
                previousPoint.X = pt.X;
                previousPoint.Y = pt.Y;
            }
            Line line = new Line();
            line.Stroke = Brushes.LightSteelBlue;

            line.X1 = previousPoint.X;
            line.Y1 = previousPoint.Y;
            line.X2 = pt.X;
            line.Y2 = pt.Y;

            line.StrokeThickness = 2;
            plot.Children.Add(line);

            await PostLine(previousPoint, pt );

            previousPoint.X = pt.X;
            previousPoint.Y = pt.Y;
        }

        private async void Erase_Click(object sender, RoutedEventArgs e)
        {
            await EraseLines();
        }

        private async Task EraseLines()
        {
            DTO_DrawID temp = new DTO_DrawID();
            temp.DrawID = 2;

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "ClearDrawPoints"), temp);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_DrawPoints>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_DrawPoints>));
                var pointsList = des.Data.ToList();

                listviewLines.ItemsSource = pointsList;
                await GetLineCount();
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
        }

        //WEB SERVICE FUNCTIONS///////////////////////////////////////////////////////////////////////

        public async Task<DTO_Users> WS_Login(DTO_Login login)
        {
            DTO_Users temp = new DTO_Users(); 
            try
            {           
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "Login"), login);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_Users>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_Users>));
                var userList = des.Data.ToList();

                if (userList.Count == 1)
                {
                    temp = userList.FirstOrDefault();
                }
                else
                    temp = null;
            }
            catch (Newtonsoft.Json.JsonSerializationException hre)
            {
                Debug.WriteLine(hre.Message);
            }        
            return temp;
        }

        public async Task<DTO_Users> WS_RegisterUser(DTO_Users user)
        {
            DTO_Users temp = new DTO_Users();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "AddUser"), user);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_Users>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_Users>));
                var userList = des.Data.ToList();

                if (userList.Count == 1)
                {
                    temp = userList.FirstOrDefault();
                }
                else
                    temp = null;
            }
            catch (Newtonsoft.Json.JsonSerializationException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return temp;
        }

        public async Task GetAllUsers()
        {
            try
            {
                DTO_Users temp = new DTO_Users();
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "GetAllUsers"), temp);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_Users>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_Users>));
                var userList = des.Data.ToList();
                listviewUsers.ItemsSource = userList;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
        }

        public async Task WS_GatherOpenDraws()
        {
            DTO_OpenDraws draw = new DTO_OpenDraws();
            List<DTO_OpenDraws> drawList;
            try
            {      
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "GetOpenDraws"), draw);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_OpenDraws>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_OpenDraws>));
                drawList = des.Data.ToList();
                S_DrawList.Instance.drawList = drawList;         
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
        }

        public async Task<DTO_JoinDraw> WS_JoinDraw(DTO_JoinDraw join)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "JoinDraw"), join);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_JoinDraw>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_JoinDraw>));
                var drawList = des.Data.ToList();

                if (drawList.Count == 1)
                {
                    join = drawList.FirstOrDefault();
                    S_Noodler.Instance.drawNoodlerID = join.NoodlerID;
                }
                else
                    join = null;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }       
            return join;
        }

        public async Task<DTO_Guess> WS_CheckGuess(DTO_Guess guess)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "CheckGuess"), guess);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_Guess>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_Guess>));
                var userList = des.Data.ToList();

                if (userList.Count == 1)
                {
                    guess = userList.FirstOrDefault();
                }
                else
                    guess = null;
            }
            catch (Newtonsoft.Json.JsonSerializationException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return guess;
        }

        public async Task<DTO_Winner> WS_SetWinner(DTO_Winner winner)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "SetWinner"), winner);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_Winner>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_Winner>));
                var drawList = des.Data.ToList();

                if (drawList.Count == 1)
                {
                    winner = drawList.FirstOrDefault();
                }
                else
                    winner = null;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return winner;
        }

        public async Task<DTO_OpenDraws> WS_StartDraw(DTO_OpenDraws draw)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "StartDraw"), draw);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_OpenDraws>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_OpenDraws>));
                var drawList = des.Data.ToList();

                if (drawList.Count == 1)
                {
                    draw = drawList.FirstOrDefault();                   
                }
                else
                    draw = null;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return draw;
        }

        public async Task<DTO_DrawID> WS_EndDraw(DTO_DrawID drawid)
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "EndDraw"), drawid);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_DrawID>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_DrawID>));
                var drawList = des.Data.ToList();

                if (drawList.Count == 1)
                {
                    drawid = drawList.FirstOrDefault();
                }
                else
                    drawid = null;
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return drawid;
        }

        private async Task<List<DTO_GameCategory>> WS_GetDrawCategories(DTO_GameCategory gameCategory)
        {
            List<DTO_GameCategory> gameCategoryList = new List<DTO_GameCategory>();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "GetDrawCategories"), gameCategory);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_GameCategory>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_GameCategory>));
                var tempGameCategoryList = des.Data.ToList();

                foreach (var s in tempGameCategoryList)
                {
                    DTO_GameCategory newGameCategory = new DTO_GameCategory();
                    newGameCategory.CategoryID = s.CategoryID;
                    newGameCategory.CategoryName = s.CategoryName;
                    gameCategoryList.Add(newGameCategory);
                }
            }
            catch (HttpRequestException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return gameCategoryList;
        }

        private async Task<DTO_OpenDraws> WS_CreateDraw(DTO_NewDraw draw)
        {
            DTO_OpenDraws openDraw = new DTO_OpenDraws();
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(string.Format(@"{0}{1}", URL, "CreateDraw"), draw);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var des = (Wrapper<DTO_OpenDraws>)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(Wrapper<DTO_OpenDraws>));
                var drawList = des.Data.ToList();

                if (drawList.Count == 1)
                {
                    openDraw = drawList.FirstOrDefault();
                }
                else
                    openDraw = null;
            }
            catch (Newtonsoft.Json.JsonSerializationException hre)
            {
                Debug.WriteLine(hre.Message);
            }
            return openDraw;
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
            Page_DGame.Visibility = Visibility.Collapsed;
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

        private async void BTN_LoginLogin_Click(object sender, RoutedEventArgs e)
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

                DTO_Users user = new DTO_Users();
                var temp = await WS_Login(login);
                user = temp;
                if (user != null)
                {
                    S_User.Instance.userID = user.ID;
                    S_User.Instance.displayName = user.DisplayName;
                    S_User.Instance.emailAddress = user.EmailAddress;
                    S_User.Instance.picture = user.Picture;
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

        private void BTN_LoginRegister_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            DrawPage_Register();
        }

        //HOME PAGE////////////////////////////////////////////////////////////////////////////////////

        private async void DrawPage_Home()
        {
            CollapsePages();
            Page_Homepage.Visibility = Visibility.Visible;
            BTN_HomepageJoin.Visibility = Visibility.Hidden;
            TBlock_HomepageUser.Text = S_User.Instance.displayName;
            await WS_GatherOpenDraws();
            LV_HomePageDraws.ItemsSource = S_DrawList.Instance.drawList;
            LV_HomePageDraws.SelectedIndex = -1;
        }

        private void BTN_HomepageNew_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_DLobby();
        }

        private void BTN_HomepageJoin_Click(object sender, RoutedEventArgs e)
        {
            if(LV_HomePageDraws.SelectedIndex != -1)
            {
                S_Draw.Instance.drawID = S_DrawList.Instance.drawList[LV_HomePageDraws.SelectedIndex].DrawID;
                S_Draw.Instance.doodlerName = S_DrawList.Instance.drawList[LV_HomePageDraws.SelectedIndex].Doodler;
                S_Draw.Instance.drawCategoryName = S_DrawList.Instance.drawList[LV_HomePageDraws.SelectedIndex].DrawCategoryName;
                DrawPage_NLobby();
            }
        }

        private void LV_HomePageDraws_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BTN_HomepageJoin.Visibility = Visibility.Visible;
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

        private void BTN_RegisterBack_Click(object sender, RoutedEventArgs e)
        {
            output.Text = "";
            DrawPage_Login();
        }

        public async void BTN_RegisterSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (TBox_RegisterEmail.Text != "" && TBox_RegisterPassword.Text != ""
                && TBox_RegistrationPicture.Text != "" && TBox_RegisterUserName.Text != "")
            {
                DTO_Users user = new DTO_Users();
                user.EmailAddress = TBox_RegisterEmail.Text;
                user.Password = TBox_RegisterPassword.Text;
                user.Picture = TBox_RegistrationPicture.Text;
                user.DisplayName = TBox_RegisterUserName.Text;

                user = await WS_RegisterUser(user);
                if (user != null)
                {
                    DTO_Login login = new DTO_Login();
                    login.Email = user.EmailAddress;
                    login.Password = user.Password;
                    login.Latitude = 400;
                    login.Longitude = 200;

                    user = await WS_Login(login);
                    if (user != null)
                    {
                        S_User.Instance.userID = user.ID;
                        S_User.Instance.displayName = user.DisplayName;
                        S_User.Instance.emailAddress = user.EmailAddress;
                        S_User.Instance.picture = user.Picture;
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
                if (TBox_RegisterEmail.Text == "")
                {
                    TBox_RegisterEmail.Text = "Required";
                }
                if (TBox_RegisterPassword.Text == "")
                {
                    TBox_RegisterPassword.Text = "Required";
                }
                if (TBox_RegistrationPicture.Text == "")
                {
                    TBox_RegistrationPicture.Text = "Required";
                }
                if (TBox_RegisterUserName.Text == "")
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
            TBlock_NLobbyDoodler.Text = S_Draw.Instance.doodlerName;
            TBlock_NLobbyCategory.Text = S_Draw.Instance.drawCategoryName;
        }

        private void BTN_NLobbyBack_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        private async void BTN_NLobbyJoin_Click(object sender, RoutedEventArgs e)
        {
            DTO_JoinDraw join = new DTO_JoinDraw();
            join.DrawID = S_Draw.Instance.drawID;
            join.UserID = S_User.Instance.userID;

            join = await WS_JoinDraw(join);
            if (join != null)
            {
                DrawPage_NGame();
            }
        }

        //NOODLER GAME PAGE///////////////////////////////////////////////////////////////////////////////

        public async void DrawPage_NGame()
        {
            CollapsePages();
            Page_NGame.Visibility = Visibility.Visible;
            BTN_NGameGuess.Visibility = Visibility.Visible;
            TBox_NGameGuess.Text = "";
            TBlock_NGameGuessTicker.Text = "";
            TBlock_NGameCategory.Text = S_Draw.Instance.drawCategoryName;
            BTN_NGameHome.Visibility = Visibility.Hidden;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += Timer_Tick;

            await GetLineCount();
            await PullLines();

            timer.Start();
        }

        private async void BTN_NGameGuess_Click(object sender, RoutedEventArgs e)
        {
            DTO_Guess guess = new DTO_Guess();
            guess.DrawID = S_Draw.Instance.drawID;
            guess.Guess = TBox_NGameGuess.Text;

            guess = await WS_CheckGuess(guess);
            if (guess.IsCorrect == true)
            {
                TBlock_NGameGuessTicker.Text = "The Winner is: " + S_User.Instance.displayName;
                TBox_NGameGuess.Visibility = Visibility.Hidden;
                BTN_NGameGuess.Visibility = Visibility.Hidden;
                BTN_NGameHome.Visibility = Visibility.Visible;
                DTO_Winner winner = new DTO_Winner();
                winner.NoodlerID = S_Noodler.Instance.drawNoodlerID;
                await WS_SetWinner(winner);
                DTO_DrawID drawid = new DTO_DrawID();
                drawid.DrawID = S_Draw.Instance.drawID;
                await WS_EndDraw(drawid);
            }
            else
            {
                TBlock_NGameGuessTicker.Text = "Incorrect";
            }
        }

        private void BTN_NGameHome_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        //DOODLER LOBBY PAGE//////////////////////////////////////////////////////////////////////////////

        private async void DrawPage_DLobby()
        {
            CollapsePages();
            DTO_GameCategory gameCategory = new DTO_GameCategory();
            CBox_GGameCategory.ItemsSource = await WS_GetDrawCategories(gameCategory);
            CBox_GGameCategory.SelectedIndex = -1;
            Page_DLobby.Visibility = Visibility.Visible;
        }

        private void BTN_DLobbyBack_Click(object sender, RoutedEventArgs e)
        {
            DrawPage_Home();
        }

        private async void BTN_DLobbySubmit_Click(object sender, RoutedEventArgs e)
        {
            DTO_NewDraw draw = new DTO_NewDraw();
            draw.DoodlerID = S_User.Instance.userID;
            draw.Answer = TBox_GGameAnswer.Text;
            draw.StartTime = null;
            draw.DrawCategoryID = CBox_GGameCategory.SelectedIndex + 1;

            DTO_OpenDraws newdraw = await WS_CreateDraw(draw);
            S_Draw.Instance.drawCategoryName = newdraw.DrawCategoryName;
            S_Draw.Instance.drawID = newdraw.DrawID;
            S_Draw.Instance.doodleUserID = S_User.Instance.userID;
            S_Draw.Instance.drawStatusDesc = newdraw.DrawStatusDesc;
            S_Draw.Instance.drawStatusID = 1;
            S_Draw.Instance.categoryID = draw.DrawCategoryID.Value;
            S_Draw.Instance.startTime = newdraw.StartTime;
            //S_Draw.Instance.drawStatusID = 

            /*
            while(GV_Draw.StartTime > System.DateTime.Now)
            {
                TBlock_GGameStartTimer.Text = (GV_Draw.StartTime - System.DateTime.Now).ToString();
            }
            */
            DrawPage_DGame();
            await WS_StartDraw(newdraw);
        }

        //DOODLER GAME PAGE////////////////////////////////////////////////////////////////////////////////////

        public async void DrawPage_DGame()
        {
            CollapsePages();
            Page_DGame.Visibility = Visibility.Visible;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += Timer_Tick;

            await GetLineCount();
            await PullLines();

            timer.Start();
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
