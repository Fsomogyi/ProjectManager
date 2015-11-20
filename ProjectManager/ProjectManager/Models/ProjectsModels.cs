using BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class OverviewModel
    {
        public Project Project { get; private set; }
        public IEnumerable<String> DeveloperNames { get; private set; }
        public int TasksDone { get; private set; }
        public int TasksActive { get; private set; }
        public int TasksUnasigned { get; private set; }
        public int WorkHours { get; private set; }

        public OverviewModel(Project Project, IEnumerable<String> DeveloperNames, int TasksDone, int TasksActive, int TasksUnasigned, int WorkHours)
        {
            this.Project = Project;
            this.DeveloperNames = DeveloperNames;
            this.TasksDone = TasksDone;
            this.TasksActive = TasksActive;
            this.TasksUnasigned = TasksUnasigned;
            this.WorkHours = WorkHours;
        }
    }

    public class TaskListElement
    {
        public Task Task { get; private set; }
        public string State { get; private set; }
        public IEnumerable<String> DeveloperNames { get; private set; }
        public int WorkHours { get; private set; }
        public bool HasComments { get; private set; }

        public TaskListElement(Task Task, string State, IEnumerable<String> DeveloperNames, int WorkHours, bool HasComments)
        {
            this.Task = Task;
            this.State = State;
            this.DeveloperNames = DeveloperNames;
            this.WorkHours = WorkHours;
            this.HasComments = HasComments;
        }
    }

    public class DeveloperListElement
    {
        public ProjectUser ProjectUser { get; private set; }
        public int WorkHours { get; private set; }
        public int TasksDone { get; private set; }
        public int AssignedTasks { get; private set; }

        public DeveloperListElement(ProjectUser ProjectUser, int WorkHours, int TasksDone, int AssignedTasks)
        {
            this.ProjectUser = ProjectUser;
            this.WorkHours = WorkHours;
            this.TasksDone = TasksDone;
            this.AssignedTasks = AssignedTasks;
        }
    }

    public class StatisticsModel
    {

    }
}