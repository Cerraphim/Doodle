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
    
    public partial class draw
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public draw()
        {
            this.drawPoints = new HashSet<drawPoint>();
            this.noodlers = new HashSet<noodler>();
        }
    
        public int DrawID { get; set; }
        public Nullable<int> DoodlerUserID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public Nullable<int> DrawStatusId { get; set; }
        public string Answer { get; set; }
    
        public virtual drawCategory drawCategory { get; set; }
        public virtual drawStatu drawStatu { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<drawPoint> drawPoints { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<noodler> noodlers { get; set; }
    }
}