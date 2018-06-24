using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DoodleDAL;
using DoodleModel;

namespace DoodleService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public DTO_Users GetDoodleUser(DTO_Users uo)
        {
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {

                var record = db.users.Where(c => c.UserID == uo.ID).FirstOrDefault();
                if (record != null)
                {
                    uo.Active = record.Active.Value;
                    uo.DisplayName = record.DisplayName;
                    uo.EmailAddress = record.Email;
                    uo.ID = record.UserID;
                    uo.Password = record.Password;
                    uo.Picture = record.Picture;
                    return uo;
                }
                else
                    return uo;
            }
        }

        public List<DTO_Users> AddUser(DTO_Users user)
        {
            List<DTO_Users> users = new List<DTO_Users>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var sqllist = db.users.Where(c => c.Email == user.EmailAddress).ToList();       
                if (sqllist.Count == 0)
                {
                    user sqlobj = new user();
                    sqlobj.UserID = user.ID;
                    sqlobj.DisplayName = user.DisplayName;
                    sqlobj.Email = user.EmailAddress;
                    sqlobj.Password = user.Password;
                    sqlobj.Picture = user.Picture;
                    sqlobj.Active = true;
                    db.users.Add(sqlobj);
                    users.Add(user);
                    db.SaveChanges();
                }
                else
                {
                    users.Clear();
                }
                return users;
            }
        }

        public List<DTO_Users> GetAllUsers(DTO_Users user)
        {
            List<DTO_Users> users = new List<DTO_Users>();   
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var sqllist = db.users.OrderBy(c=>c.DisplayName).ToList();
                foreach( var s in sqllist)
                {
                    DTO_Users u = new DTO_Users();
                    u.DisplayName = s.DisplayName;
                    if (s.Active.HasValue)
                    {
                        u.Active = s.Active.Value;
                    }
                    u.EmailAddress = s.Email;
                    u.ID = s.UserID;
                    u.Password = s.Password;
                    u.Picture = s.Picture;
                    users.Add(u);
                }
            }      
            return users;
        }

        public List<DTO_Users> Login(DTO_Login login)
        {
            List<DTO_Users> users = new List<DTO_Users>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var sqllist = db.users.Where(c => c.Email.ToLower() == login.Email.ToLower()
                && c.Password == login.Password).ToList();

                foreach (var s in sqllist)
                {
                    DTO_Users u = new DTO_Users();
                    u.DisplayName = s.DisplayName;
                    if (s.Active.HasValue)
                    {
                        u.Active = s.Active.Value;
                    }
                    u.EmailAddress = s.Email;
                    u.ID = s.UserID;
                    u.Password = s.Password;
                    u.Picture = s.Picture;
                    users.Add(u);
                }

                if (sqllist.Count == 1)
                {
                    userLogin sqlobj = new userLogin();
                    sqlobj.UserID = sqllist[0].UserID;
                    sqlobj.Latitude = login.Latitude;
                    sqlobj.Longitude = login.Longitude;
                    sqlobj.LoginDateTime = System.DateTime.Now;

                    db.userLogins.Add(sqlobj);
                    db.users.Find(users[0].ID).Active = true;
                    db.SaveChanges();
                }
                else
                {
                    users.Clear();
                }
            }
            return users;
        }

        public List<DTO_OpenDraws> GetOpenDraws(List<DTO_OpenDraws> list)
        {
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                int temp = 2;
                var sqllist = db.draws.Where(c => c.DrawStatusID.Value == temp).ToList();
               
                foreach (var s in sqllist)
                {
                    DTO_OpenDraws o = new DTO_OpenDraws();
                    o.Doodler = "Test Doodler";
                    o.DrawCategoryName = "Test DrawCategoryName";
                    o.DrawID = s.DrawID;
                    if(s.StartTime.HasValue)
                    {
                        o.StartTime = s.StartTime;
                    }                   
                    o.DrawStatusDesc = "Test DrawStatusDesc";
                    list.Add(o);
                }
                return list;
            }
        }

        public List<DTO_JoinDraw> JoinDraw(DTO_JoinDraw join)
        {
            List<DTO_JoinDraw> joinlist = new List<DTO_JoinDraw>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                noodler sqlobj = new noodler();
                sqlobj.DrawID = join.DrawID;
                sqlobj.UserID = join.UserID;
                sqlobj.Winner = false;
                db.noodlers.Add(sqlobj);
                db.SaveChanges();

                var sqlrecord = db.noodlers.Where(c => c.UserID == join.UserID).FirstOrDefault();
                join.NoodlerID = sqlrecord.DrawNoodlerID;
                joinlist.Add(join);
            }
            return joinlist;
        }

        public List<DTO_Winner> SetWinner(DTO_Winner winner)
        {
            List<DTO_Winner> list = new List<DTO_Winner>();
            DTO_Winner win = new DTO_Winner();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                //db.users.Find(u.ID).Active = true;
                
                var obj = db.noodlers.Where(c => c.DrawNoodlerID == winner.NoodlerID).FirstOrDefault();
                if(obj != null)
                {
                    obj.Winner = true;
                    db.SaveChanges();
                }
            }
            list.Add(win);
            return list;
        }

        public List<DTO_Guess> CheckGuess(DTO_Guess guess)
        {
            List<DTO_Guess> list = new List<DTO_Guess>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var record = db.draws.Where(c => c.DrawID == guess.DrawID).FirstOrDefault();
                if (guess.Guess == record.Answer)
                {
                    guess.IsCorrect = true;
                }
                else
                {
                    guess.IsCorrect = false;
                }
            }
            list.Add(guess);
            return list;
        }

        public List<DTO_DrawPoints> GetDrawPoints(DTO_DrawID drawID)
        {
            List<DTO_DrawPoints> drawpointlist = new List<DTO_DrawPoints>();

            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var points = db.drawPoints.Where(c => c.DrawID == drawID.DrawID).ToList();
                foreach (var s in points)
                {
                    DTO_DrawPoints u = new DTO_DrawPoints();
                    u.DrawID = s.DrawID.Value;
                    u.DrawPointID = s.DrawPointID;
                    u.DrawPointX = s.DrawPointX.Value;
                    u.DrawPointY = s.DrawPointY.Value;
                    u.DrawPointX2 = s.DrawPointX2.Value;
                    u.DrawPointY2 = s.DrawPointY2.Value;
                    drawpointlist.Add(u);
                }
            }
            return drawpointlist;
        }

        public List<DTO_DrawPoints> ClearDrawPoints(DTO_DrawID drawID)
        {
            List<DTO_DrawPoints> drawpointlist = new List<DTO_DrawPoints>();

            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var points = db.drawPoints.Where(c => c.DrawID == drawID.DrawID).ToList();
                foreach (var s in points)
                {
                    db.drawPoints.Remove(s);
                }
                db.SaveChanges();
            }
            return drawpointlist;
        }

        public List<DTO_OpenDraws> CreateDraw(DTO_NewDraw draw)
        {
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                List<DTO_OpenDraws> openDrawList = new List<DTO_OpenDraws>();
                DTO_OpenDraws openDraw = new DTO_OpenDraws();

                int? temp = 1;
                draws sqlobj = new draws();
                sqlobj.CategoryID = draw.DrawCategoryID;
                sqlobj.DoodlerUserID = draw.DoodlerID;
                sqlobj.Answer = draw.Answer;
                sqlobj.DrawStatusID = temp;
                sqlobj.StartTime = draw.StartTime;
                sqlobj.EndTime = System.DateTime.Now;
                db.draws.Add(sqlobj);
                db.SaveChanges();

                int temp2 = 1;
                openDraw.DrawID = sqlobj.DrawID;
                //openDraw.Doodler = db.users.Where(c => c.UserID.Equals(draw.DoodlerID)).FirstOrDefault().DisplayName;
                //openDraw.DrawStatusDesc = db.drawStatus.Where(c => c.DrawStatusID.Equals(temp2)).FirstOrDefault().DrawStatusDesc;
                //openDraw.DrawCategoryName = db.drawCategories.Where(c => c.DrawCategoryID.Equals(draw.DrawCategoryID)).FirstOrDefault().DrawCategoryName;
                openDraw.Doodler = "Test Doodler";
                openDraw.DrawStatusDesc = "Test DrawStatusDesc";
                openDraw.DrawCategoryName = "Test DrawCategoryName";
                openDraw.StartTime = System.DateTime.Now;
                openDrawList.Add(openDraw);
                
                return openDrawList;
            }
        }

        public List<DTO_DrawPoints> PushLine(DTO_DrawPoints lineSegment)
        {
            List<DTO_DrawPoints> ret = new List<DTO_DrawPoints>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                drawPoint dp = new drawPoint();
                dp.DrawID = lineSegment.DrawID;
                dp.DrawPointX = lineSegment.DrawPointX;
                dp.DrawPointY = lineSegment.DrawPointY;
                dp.DrawPointX2 = lineSegment.DrawPointX2;
                dp.DrawPointY2 = lineSegment.DrawPointY2;

                db.drawPoints.Add(dp);
                db.SaveChanges();
            }
            return ret;
        }

        public List<DTO_OpenDraws> StartDraw(DTO_OpenDraws draw)
        {
            List<DTO_OpenDraws> list = new List<DTO_OpenDraws>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                int? temp = 2;
                db.draws.Where(c => c.DrawID == draw.DrawID).FirstOrDefault().DrawStatusID = temp;
                db.SaveChanges();
            }
            list.Add(draw);
            return list;
        }

        public List<DTO_GameCategory> GetDrawCategories()
        {
            List<DTO_GameCategory> list= new List<DTO_GameCategory>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var sqllist = db.drawCategories.ToList();
                foreach(var s in sqllist)
                {
                    DTO_GameCategory gamecategory = new DTO_GameCategory();
                    gamecategory.CategoryID = s.DrawCategoryID;
                    gamecategory.CategoryName = s.DrawCategoryName;

                    list.Add(gamecategory);
                }
                return list;
            }
            
        }

        public List<DTO_LineCount> GetLineCount(DTO_DrawID drawid)
        {
            List<DTO_LineCount> list = new List<DTO_LineCount>();

            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                int lc = db.drawPoints.Where(c=>c.DrawID == drawid.DrawID).Count();
                DTO_LineCount o = new DTO_LineCount();
                o.lineCount = lc;

                list.Add(o);

                return list;
            }
        }

        public List<DTO_DrawPoints> GetLinePoints(DTO_DrawID d)
        {
            List<DTO_DrawPoints> dp = new List<DTO_DrawPoints>();
                
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                var list = db.drawPoints.Where(c => c.DrawID == d.DrawID).ToList();

                foreach ( var record in list)
                {
                    DTO_DrawPoints dpo = new DTO_DrawPoints();
                    dpo.DrawID = record.DrawID.Value;
                    dpo.DrawPointID = record.DrawPointID;
                    dpo.DrawPointX = record.DrawPointX.Value;
                    dpo.DrawPointX2 = record.DrawPointX2.Value;
                    dpo.DrawPointY = record.DrawPointY.Value;
                    dpo.DrawPointY2 = record.DrawPointY2.Value;

                    dp.Add(dpo);

                    break;
                }
            }
            return dp;
        }

        public List<DTO_DrawID> EndDraw(DTO_DrawID drawid)
        {
            List<DTO_DrawID> list = new List<DTO_DrawID>();
            using (DB_122744_doodleEntities db = new DB_122744_doodleEntities())
            {
                int temp = 3;
                db.draws.Where(c => c.DrawID.Equals(drawid.DrawID)).FirstOrDefault().DrawStatusID = temp;
                db.SaveChanges();
            }
            list.Add(drawid);
            return list;
        }
    }
}
