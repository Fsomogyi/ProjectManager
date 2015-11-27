using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManagerWebAPI.Models
{
    public class OverviewModel
    {
        public int ProjectId { get; private set; }
        public String Name { get; private set; }
        public String Description { get; private set; }
        public DateTime Deadline { get; private set; }
        public IEnumerable<String> DeveloperNames { get; private set; }
        public int TasksDone { get; private set; }
        public int TasksActive { get; private set; }
        public int TasksUnassigned { get; private set; }
        public int WorkHours { get; private set; }
        public string ProjectLeaderName { get; set; }

        public OverviewModel(int Project, String Name, String Description, DateTime Deadline, IEnumerable<String> DeveloperNames, int TasksDone, int TasksActive, int TasksUnassigned, int WorkHours, string ProjectLeaderName)
        {
            this.ProjectId = Project;
            this.Name = Name;
            this.Description = Description;
            this.Deadline = Deadline;
            this.DeveloperNames = DeveloperNames;
            this.TasksDone = TasksDone;
            this.TasksActive = TasksActive;
            this.TasksUnassigned = TasksUnassigned;
            this.WorkHours = WorkHours;
            this.ProjectLeaderName = ProjectLeaderName;
        }
    }

    public class TaskListElement
    {
        public int TaskId { get; private set; }
        public String TaskName { get; private set; }
        public string State { get; private set; }
        public IEnumerable<String> DeveloperNames { get; private set; }
        public int WorkHours { get; private set; }
        public bool HasComments { get; private set; }

        public TaskListElement(int TaskId, string TaskName, string State, IEnumerable<String> DeveloperNames, int WorkHours, bool HasComments)
        {
            this.TaskId = TaskId;
            this.TaskName = TaskName;
            this.State = State;
            this.DeveloperNames = DeveloperNames;
            this.WorkHours = WorkHours;
            this.HasComments = HasComments;
        }
    }
}