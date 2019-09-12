﻿using Autofac;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling
{
    internal sealed class ProjectHandlingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectService>().As<IProjectService>().SingleInstance();

            builder.RegisterType<ProjectExplorerTool>().As<Tool>().SingleInstance();

            builder.RegisterType<NewProjectDialogViewModelFactory>().As<INewProjectDialogViewModelFactory>().SingleInstance();
            builder.RegisterType<ProjectExplorerViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ProjectExplorerItemViewModelFactory>().As<IProjectExplorerItemViewModelFactory>().SingleInstance();
            builder.RegisterType<AddContextMenuItemFactory>().As<IAddContextMenuItemFactory>().SingleInstance();
            builder.RegisterType<AddNewFolderDialogViewModelFactory>().As<IAddNewFolderDialogViewModelFactory>().SingleInstance();
        }
    }
}