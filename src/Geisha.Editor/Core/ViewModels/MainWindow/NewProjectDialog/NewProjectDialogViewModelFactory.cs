﻿using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;

namespace Geisha.Editor.Core.ViewModels.MainWindow.NewProjectDialog
{
    public interface INewProjectDialogViewModelFactory
    {
        NewProjectDialogViewModel Create();
    }

    public class NewProjectDialogViewModelFactory : INewProjectDialogViewModelFactory
    {
        private readonly IRequestFilePathService _requestFilePathService;
        private readonly IProjectService _projectService;

        public NewProjectDialogViewModelFactory(IRequestFilePathService requestFilePathService, IProjectService projectService)
        {
            _requestFilePathService = requestFilePathService;
            _projectService = projectService;
        }

        public NewProjectDialogViewModel Create()
        {
            return new NewProjectDialogViewModel(_requestFilePathService, _projectService);
        }
    }
}