//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BusinessLogicLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskStateChange
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public System.DateTime Timestamp { get; set; }
        public int TaskState { get; set; }
        public string Reason { get; set; }
        public bool Accepted { get; set; }
    
        public virtual ProjectUser ProjectUser { get; set; }
        public virtual Task Task { get; set; }
        public virtual TaskState TaskState1 { get; set; }
    }
}
