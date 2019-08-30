using Autofac;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.Core.Views.Infrastructure;
using Geisha.Editor.ProjectHandling.Domain;
using Geisha.Editor.ProjectHandling.Infrastructure;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.Core
{
    internal sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Infrastructure
            builder.RegisterType<VersionProvider>().As<IVersionProvider>().SingleInstance();

            // Models
            builder.RegisterType<ProjectService>().As<IProjectService>().SingleInstance();
            builder.RegisterType<ProjectRepository>().As<IProjectRepository>().SingleInstance();

            // ViewModels
            builder.RegisterType<NewProjectDialogViewModelFactory>().As<INewProjectDialogViewModelFactory>().SingleInstance();
            builder.RegisterType<ProjectExplorerDockableViewViewModelFactory>().As<IDockableViewViewModelFactory>().SingleInstance();
            builder.RegisterType<ProjectExplorerViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ProjectItemViewModelFactory>().As<IProjectItemViewModelFactory>().SingleInstance();
            builder.RegisterType<AddContextMenuItemFactory>().As<IAddContextMenuItemFactory>().SingleInstance();
            builder.RegisterType<AddNewFolderDialogViewModelFactory>().As<IAddNewFolderDialogViewModelFactory>().SingleInstance();

            // Views
            builder.RegisterType<OpenFileDialogRequestFilePathService>().As<IRequestFilePathService>().SingleInstance();
        }
    }
}