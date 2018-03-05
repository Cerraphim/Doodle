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
        public string GetDoodleUser(int userID)
        {
            DB_42039_doodleEntities db = new DB_42039_doodleEntities();

            var record = db.users.Where(c => c.UserID == userID).FirstOrDefault();
            if (record != null)
                return record.DisplayName;
            else
                return "Not Found";
        }

        public DTO_Users AddUser(DTO_Users user)
        {
            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                var record = db.users.Where(c => c.Email == user.EmailAddress).FirstOrDefault();
                if (record == null)
                {
                    user sqlobj = new user();
                    sqlobj.DisplayName = user.DisplayName;
                    sqlobj.Email = user.EmailAddress;
                    sqlobj.Picture = user.Picture;
                    sqlobj.Pword = user.Password;
                    sqlobj.Active = user.Active;

                    db.users.Add(sqlobj);
                    db.SaveChanges();

                    user.ID = sqlobj.UserID;
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<DTO_Users> GetAllUsers()
        {
            List<DTO_Users> users = new List<DTO_Users>();

            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                var sqllist = db.users.OrderBy(c=>c.DisplayName).ToList();
                foreach( var s in sqllist)
                {
                    DTO_Users u = new DTO_Users();
                    u.DisplayName = s.DisplayName;
                    u.Active = s.Active.Value;
                    u.EmailAddress = s.Email;
                    u.ID = s.UserID;
                    u.Password = s.Pword;
                    u.Picture = s.Picture;

                    users.Add(u);
                }
            }

            return users;
        }

        public DTO_Users Login(DTO_Login login)
        {
            DTO_Users user = new DTO_Users();

            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                var sqlrecord = db.users.Where(c => c.Email.ToLower() == login.Email.ToLower()
                && c.Pword == login.Password).FirstOrDefault();

                if (sqlrecord != null)
                {
                    user.DisplayName = sqlrecord.DisplayName;
                    user.Active = sqlrecord.Active.Value;
                    user.EmailAddress = sqlrecord.Email;
                    user.ID = sqlrecord.UserID;
                    user.Password = sqlrecord.Pword;
                    user.Picture = sqlrecord.Picture;

                    userLogin sqlobj = new userLogin();
                    sqlobj.UserID = sqlrecord.UserID;
                    sqlobj.Latitude = 300;
                    sqlobj.Longitude = 400;
                    sqlobj.LoginDateTime = System.DateTime.Now;

                    db.userLogins.Add(sqlobj);
                    db.users.Find(user.ID).Active = true;
                    db.SaveChanges();
                }
                else
                    user = null;
            }
            return user;
        }

        public List<DTO_OpenDraws> GatherOpenDraws()
        {
            List<DTO_OpenDraws> list = new List<DTO_OpenDraws>();

            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                var records = db.sp_GatherOpenDraws();
                foreach(var s in records)
                {
                    DTO_OpenDraws o = new DTO_OpenDraws();
                    o.Doodler = s.Doodler;
                    o.DrawCategoryName = s.DrawCategoryName;
                    o.DrawID = s.DrawID;
                    o.DrawStatusDesc = s.DrawStatusDesc;
                    o.StartTime = s.StartTime.Value;

                    list.Add(o);
                }
            }

            return list;
        }

        public DTO_JoinDraw JoinDraw(DTO_JoinDraw join)
        {        
            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                noodler sqlobj = new noodler();
                sqlobj.DrawID = join.DrawID;
                sqlobj.UserID = join.UserID;
                sqlobj.Winner = false;
                db.noodlers.Add(sqlobj);
                db.SaveChanges();

                var sqlrecord = db.noodlers.Where(c => c.UserID == join.UserID).FirstOrDefault();
                join.NoodlerID = sqlrecord.DrawNoodlerID;
            }
            return join;
        }

        public DTO_Winner SetWinner(DTO_Winner winner)
        {
            DTO_Winner win = new DTO_Winner();
            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                //db.users.Find(u.ID).Active = true;
                
                var obj = db.noodlers.Where(c => c.DrawNoodlerID == winner.NoodlerID).FirstOrDefault();
                if(obj != null)
                {
                    obj.Winner = true;
                    db.SaveChanges();
                }
            }
            return win;
        }

        public DTO_Guess CheckGuess(DTO_Guess guess)
        {
            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                var record = db.draws.Where(c => c.DrawID == guess.DrawID).FirstOrDefault();
                if (guess.Guess == record.Answer)
                {
                    guess.IsCorrect = true;
                }
                else guess.IsCorrect = false;
            }
            return guess;
        }

        public List<DTO_DrawPoints> GetDrawPoints(DTO_DrawID drawID)
        {
            List<DTO_DrawPoints> drawpointlist = new List<DTO_DrawPoints>();

            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                var points = db.drawPoints.Where(c => c.DrawID == drawID.DrawID).ToList();
                foreach (var s in points)
                {
                    DTO_DrawPoints u = new DTO_DrawPoints();
                    u.DrawID = s.DrawID.Value;
                    u.DrawPointID = s.DrawPointID;
                    u.DrawPointX = s.DrawPointX.Value;
                    u.DrawPointY = s.DrawPointY.Value;
                    drawpointlist.Add(u);
                }
            }
            return drawpointlist;
        }

        public DTO_OpenDraws CreateDraw(DTO_NewDraw draw)
        {
            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
            {
                DTO_OpenDraws opendraw = new DTO_OpenDraws();

                draw sqlobj = new draw();
                sqlobj.CategoryID = draw.DrawCategoryID;
                sqlobj.DoodlerUserID = draw.DoodlerID;
                sqlobj.Answer = "1";
                sqlobj.DrawStatusId = 1;
                sqlobj.StartTime = new DateTime();

                db.draws.Add(sqlobj);
                db.SaveChanges();

                return opendraw;
            }
        }

        public DTO_OpenDraws StartDraw(DTO_OpenDraws draw)
        {

            return draw;
        }

        public List<DTO_GameCategory> GetDrawCategories()
        {
            List<DTO_GameCategory> list= new List<DTO_GameCategory>();
            using (DB_42039_doodleEntities db = new DB_42039_doodleEntities())
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
    }
}
