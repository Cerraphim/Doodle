//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoodleDAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class userLogin
    {
        public int LoginID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<System.DateTime> LoginDateTime { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
    
        public virtual user user { get; set; }
    }
}
