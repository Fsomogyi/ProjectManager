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
        public int TasksUnassigned { get; private set; }
        public int WorkHours { get; private set; }
        public string ProjectLeaderName { get; set; }

        public OverviewModel(Project Project, IEnumerable<String> DeveloperNames, int TasksDone, int TasksActive, int TasksUnassigned, int WorkHours, string ProjectLeaderName)
        {
            this.Project = Project;
            this.DeveloperNames = DeveloperNames;
            this.TasksDone = TasksDone;
            this.TasksActive = TasksActive;
            this.TasksUnassigned = TasksUnassigned;
            this.WorkHours = WorkHours;
            this.ProjectLeaderName = ProjectLeaderName;
        }
    }

    public class ProjectListElement
    {
        public Project Project { get; private set; }
        public int Developers { get; private set; }
        public int Tasks { get; private set; }

        public ProjectListElement(Project Project, int Developers, int Tasks)
        {
            this.Project = Project;
            this.Developers = Developers;
            this.Tasks = Tasks;
        }
    }

    public class ProjectsViewModel
    {
        public IEnumerable<ProjectListElement> Projects { get; private set; }

        public ProjectsViewModel(IEnumerable<ProjectListElement> Projects)
        {
            this.Projects = Projects;
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

    public class AddDeveloperDialogModel
    {
        public int ProjectId { get; private set; }
        public List<ProjectUser> AddableDevelopers { get; private set; }
        public List<ProjectUser> RemovableDevelopers { get; private set; }

        public AddDeveloperDialogModel(int ProjectId, List<ProjectUser> AddableDevelopers,
            List<ProjectUser> RemovableDevelopers)
        {
            this.ProjectId = ProjectId;
            this.AddableDevelopers = AddableDevelopers;
            this.RemovableDevelopers = RemovableDevelopers;
        }
    }

    public class ProjectDetailsViewModel
    {
        public Project Project { get; private set; }
        public int DetailsPage { get; private set; }

        public ProjectDetailsViewModel(Project Project, int DetailsPage)
        {
            this.Project = Project;
            this.DetailsPage = DetailsPage;
        }

    }

    public class StatisticsModel
    {

    }
}