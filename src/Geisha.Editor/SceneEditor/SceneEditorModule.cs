﻿using Autofac;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.SceneEditor.Model;
using Geisha.Editor.SceneEditor.UserInterface.SceneEditor;
using Geisha.Editor.SceneEditor.UserInterface.SceneOutline;

namespace Geisha.Editor.SceneEditor
{
    internal sealed class SceneEditorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CreateEmptySceneService>().As<ICreateEmptySceneService>().SingleInstance();

            builder.RegisterType<SceneEditorDocumentFactory>().As<IDocumentFactory>().SingleInstance();

            builder.RegisterType<SceneOutlineTool>().As<Tool>().SingleInstance();
        }
    }
}