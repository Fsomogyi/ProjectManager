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
    
    public partial class Comment
    {
        public int Id { get; set; }
        public int ProjectUserId { get; set; }
        public int TaskId { get; set; }
        public string Content { get; set; }
        public System.DateTime Timestamp { get; set; }
    
        public virtual Task Task { get; set; }
        public virtual ProjectUser ProjectUser { get; set; }
    }
}
