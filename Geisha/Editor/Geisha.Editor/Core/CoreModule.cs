using Autofac;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.Models.Persistence.ProjectHandling;
using Geisha.Editor.Core.ViewModels.MainWindow;
using Geisha.Editor.Core.ViewModels.MainWindow.NewProjectDialog;
using Geisha.Editor.Core.Views.Infrastructure;

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
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<NewProjectDialogViewModelFactory>().As<INewProjectDialogViewModelFactory>().SingleInstance();

            // Views
            builder.RegisterType<OpenFileDialogRequestFilePathService>().As<IRequestFilePathService>().SingleInstance();
        }
    }
}