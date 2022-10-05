using Nexar.Renderer.DesignEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nexar.Renderer
{
    public partial class ProjectCard : UserControl
    {
        private Workspace Workspace { get; set; }

        private Project DesignProject { get; set; }

        public Action<Project> OpenButtonClicked { get; set; } = default!;

        public ProjectCard(
            Workspace workspace,
            Project designProject)
        {
            InitializeComponent();

            Workspace = workspace;
            DesignProject = designProject;

            thumbnail.ImageLocation = DesignProject.PreviewUrl;
            projectName.Text = DesignProject.Name;
            projectDescription.Text = DesignProject.Description;

            openProjectButton.Click += OpenButton_Click;
        }

        private void OpenButton_Click(object? sender, EventArgs e)
        {
            OpenButtonClicked?.Invoke(DesignProject);
        }
    }
}
