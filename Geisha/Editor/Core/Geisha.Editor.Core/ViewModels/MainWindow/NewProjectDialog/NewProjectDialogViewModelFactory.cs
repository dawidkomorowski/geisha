﻿using System.ComponentModel.Composition;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;

namespace Geisha.Editor.Core.ViewModels.MainWindow.NewProjectDialog
{
    public interface INewProjectDialogViewModelFactory
    {
        NewProjectDialogViewModel Create();
    }

    [Export(typeof(INewProjectDialogViewModelFactory))]
    public class NewProjectDialogViewModelFactory : INewProjectDialogViewModelFactory
    {
        private readonly IRequestFilePathService _requestFilePathService;
        private readonly IProjectService _projectService;

        [ImportingConstructor]
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