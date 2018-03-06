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
        [OperationContract]
        string GetDoodleUser(int userID);
        [OperationContract]
        DTO_Users AddUser(DTO_Users u);
        [OperationContract]
        List<DTO_Users> GetAllUsers();
        [OperationContract]
        DTO_Users Login(DTO_Login login);
        [OperationContract]
        List<DTO_OpenDraws> GatherOpenDraws();
        [OperationContract]
        DTO_Guess CheckGuess(DTO_Guess guess);
        [OperationContract]
        DTO_JoinDraw JoinDraw(DTO_JoinDraw join);
        [OperationContract]
        DTO_Winner SetWinner(DTO_Winner winner);
        [OperationContract]
        DTO_OpenDraws CreateDraw(DTO_NewDraw draw);
        [OperationContract]
        DTO_OpenDraws StartDraw(DTO_OpenDraws draw);
        [OperationContract]
        List<DTO_GameCategory> GetDrawCategories();
        [OperationContract]
        DTO_DrawID EndDraw(DTO_DrawID drawid);
    }
}
