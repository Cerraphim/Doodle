using DoodleModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DoodleService
{
    [ServiceContract]
    public interface IService1
    {
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        DTO_User GetDoodleUser(DTO_User u);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_User> AddUser(DTO_User u);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_User> GetAllUsers(DTO_User user);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_User> Login(DTO_Login login);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_OpenDraw> GetOpenDraws(List<DTO_OpenDraw> list);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_Guess> CheckGuess(DTO_Guess guess);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_JoinDraw> JoinDraw(DTO_JoinDraw join);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_Winner> SetWinner(DTO_Winner winner);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_OpenDraw> CreateDraw(DTO_NewDraw draw);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_OpenDraw> StartDraw(DTO_OpenDraw draw);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_GameCategory> GetDrawCategories();

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_GameStatus> GetGameStatuses();

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_DrawID> EndDraw(DTO_DrawID drawid);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_DrawPoints> PushLine(DTO_DrawPoints line);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_LineCount> GetLineCount(DTO_DrawID obj);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_DrawPoints> GetDrawPoints(DTO_DrawID drawIDObj);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedResponse)]
        [return: MessageParameter(Name = "Data")]
        [OperationContract]
        List<DTO_DrawPoints> ClearDrawPoints(DTO_DrawID drawIDObj);
    }
}
