using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DoodleModel
{
    [DataContract]
    public class Wrapper<T>
    {
        [DataMember]
        public List<T> Data { get; set; }
    }
    [DataContract]
    public class DTO_User
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string Picture { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public bool? Active { get; set; }
        
        public string print()
        {
            string retVal = string.Format("Name: {0} {1} Password {2}",
               DisplayName,
               EmailAddress,
               Password
               );
            return retVal;

        }
    }
    [DataContract]
    public class DTO_Login
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public float Latitude { get; set; }
        [DataMember]
        public float Longitude { get; set; }
    }
    [DataContract]
    public class DTO_OpenDraw
    {
        [DataMember]
        public string DrawCategoryName { get; set; }
        [DataMember]
        public DateTime? StartTime { get; set; }
        [DataMember]
        public string Doodler { get; set; }
        [DataMember]
        public string DrawStatusDesc { get; set; }
        [DataMember]
        public int DrawID { get; set; }
    }
    [DataContract]
    public class DTO_NewDraw
    {
        [DataMember]
        public int? DrawCategoryID { get; set; }
        [DataMember]
        public int? DoodlerID { get; set; }
        [DataMember]
        public string Answer { get; set; }
        [DataMember]
        public DateTime? StartTime { get; set; }
    }
    [DataContract]
    public class DTO_GameCategory
    {
        [DataMember]
        public int CategoryID { get; set; }
        [DataMember]
        public string CategoryName { get; set; }
    }
    [DataContract]
    public class DTO_GameStatus
    {
        [DataMember]
        public int GameStatusID { get; set; }
        [DataMember]
        public string GameStatusDesc { get; set; }
    }
    [DataContract]
    public class DTO_Guess
    {
        [DataMember]
        public int DrawID { get; set; }
        [DataMember]
        public string Guess { get; set; }
        [DataMember]
        public Boolean IsCorrect { get; set; }
    }
    [DataContract]
    public class DTO_DrawPoints
    {
        [DataMember]
        public int DrawID { get; set; }
        [DataMember]
        public double DrawPointX { get; set; }
        [DataMember]
        public double DrawPointY { get; set; }
        [DataMember]
        public double DrawPointX2 { get; set; }
        [DataMember]
        public double DrawPointY2 { get; set; }
        [DataMember]
        public int DrawPointID { get; set; }

        public string DisplayPoint
        {
            get
            {
                return string.Format("x1:{0:0.0} y1:{1:0.0} x2:{2:0.0} y2:{3:0.0}",
                    DrawPointX,
                    DrawPointY,
                    DrawPointX2,
                    DrawPointY2
                );
            }
        }
    }
    [DataContract]
    public class DTO_DrawID
    {
        [DataMember]
        public int DrawID { get; set; }
    }
    [DataContract]
    public class DTO_LineCount
    {
        [DataMember]
        public int lineCount { get; set; }
    }
    [DataContract]
    public class DTO_JoinDraw
    {
        [DataMember]
        public int UserID { get; set; }
        [DataMember]
        public int DrawID { get; set; }
        [DataMember]
        public int NoodlerID { get; set; }
    }
    [DataContract]
    public class DTO_Winner
    {
        [DataMember]
        public int NoodlerID { get; set; }
    }
}
