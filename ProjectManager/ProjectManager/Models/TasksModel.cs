using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class CreateTaskModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Estimated work hours")]
        public int WorkHours { get; set; }

        [Required]
        [Range(1, 4)]
        [Display(Name = "Priority")]
        public int Priority { get; set; }

        [Display(Name = "Maximum developers")]
        public int MaxDevelopers { get; set; }
    }

    public class TaskDetailsModel
    {
        public Task Task { get; private set; }
        public string StateName { get; private set; }
        public IEnumerable<String> DeveloperNames { get; private set; }
        public Dictionary<string, int> WorkHours { get; private set; }
        public IEnumerable<CommentViewModel> Comments { get; private set; }
        public bool CanComment { get; set; }
        public List<ProjectUser> AddableDevelopers { get; private set; }
        public List<ProjectUser> RemovableDevelopers { get; private set; }
        public List<ProjectUser> UnacceptedDevelopers { get; private set; }

        public TaskDetailsModel(Task Task, string State, IEnumerable<String> DeveloperNames,
            Dictionary<string, int> WorkHours, IEnumerable<CommentViewModel> Comments, bool CanComment,
            List<ProjectUser> AddableDevelopers, List<ProjectUser> RemovableDevelopers, List<ProjectUser> UnacceptedDevelopers)
        {
            this.Task = Task;
            this.StateName = State;
            this.DeveloperNames = DeveloperNames;
            this.WorkHours = WorkHours;
            this.Comments = Comments;
            this.CanComment = CanComment;
            this.AddableDevelopers = AddableDevelopers;
            this.RemovableDevelopers = RemovableDevelopers;
            this.UnacceptedDevelopers = UnacceptedDevelopers;
        }
    }

    public class CommentViewModel
    {
        public string CommentContent { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public String UserName { get; private set; }

        public CommentViewModel(string CommentContent, DateTime TimeStamp, String UserName)
        {
            this.CommentContent = CommentContent;
            this.TimeStamp = TimeStamp;
            this.UserName = UserName;
        }
    }
}