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
        public int id { get; set; }
        public int user_id { get; set; }
        public int task_id { get; set; }
        public System.DateTime timestamp { get; set; }
        public int state { get; set; }
        public string reason { get; set; }
        public bool accepted { get; set; }
    
        public virtual ProjectUser ProjectUser { get; set; }
        public virtual Task Task { get; set; }
        public virtual TaskState TaskState { get; set; }
    }
}