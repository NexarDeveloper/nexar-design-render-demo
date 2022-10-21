using Nexar.Renderer.Api;
using Nexar.Renderer.DesignEntities;
using StrawberryShake;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexar.Renderer.Forms
{
    public partial class ProjectsForm : Form
    {
        private NexarHelper NexarHelper { get; }

        public Project? SelectedDesignProject { get; set; }

        public ProjectsForm()
        {
            InitializeComponent();
            NexarHelper = new NexarHelper();
        }

        public async Task LoadProjectsAsync(Workspace workspace)
        {
            flowLayoutPanel.Controls.Clear();

            await NexarHelper.LoginAsync();
            var nexarClient = NexarHelper.GetNexarClient();

            var projectInfo = await nexarClient.GetProjectInfo.ExecuteAsync(workspace.Url);

            projectInfo.EnsureNoErrors();

            bool? hasPage = projectInfo.Data?.DesProjects?.TotalCount != 0;

            if (hasPage == true)
            {
                int? pageFraction = projectInfo.Data?.DesProjects?.TotalCount % 10;
                int? pageTotal = ((projectInfo.Data?.DesProjects?.TotalCount - pageFraction) / 10);

                string cursor = "LTE="; // Start cursor at -1 to select after  variantInfo.Pcb.DesignItems.PageInfo.StartCursor;

                while (hasPage == true)
                {
                    var projects = await nexarClient.GetProjects.ExecuteAsync(workspace.Url, cursor);

                    if (projects?.Data?.DesProjects?.Nodes != null)
                    {
                        foreach (var project in projects.Data.DesProjects.Nodes)
                        {
                            var designProject = new Project()
                            {
                                Id = project.Id,
                                Description = project?.Description ?? string.Empty,
                                Name = project?.Name ?? string.Empty,
                                PreviewUrl = project?.PreviewUrl ?? string.Empty,
                                UpdatedAt = project?.UpdatedAt?.UtcDateTime ?? DateTime.MinValue,
                                Workspace = workspace
                            };

                            var projectCard = new ProjectCard(workspace, designProject);
                            flowLayoutPanel.Controls.Add(projectCard);

                            projectCard.OpenButtonClicked = OpenButtonClicked;
                        }

                        hasPage = projects.Data?.DesProjects?.PageInfo.HasNextPage;

                        if (hasPage == true)
                        {
                            cursor = projects.Data?.DesProjects?.PageInfo.EndCursor ?? string.Empty;
                        }
                    }
                }
            }
        }

        private void OpenButtonClicked(Project designProject)
        {
            SelectedDesignProject = designProject;
            DialogResult = DialogResult.OK;
        }
    }
}
