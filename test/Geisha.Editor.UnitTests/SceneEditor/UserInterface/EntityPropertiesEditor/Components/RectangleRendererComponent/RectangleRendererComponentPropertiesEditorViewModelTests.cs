﻿using System.Threading;
using Geisha.Common.Math;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent;
using Geisha.Engine.Rendering;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent
{
    [TestFixture]
    public class RectangleRendererComponentPropertiesEditorViewModelTests
    {
        private RectangleRendererComponentModel _rectangleRendererComponentModel;
        private RectangleRendererComponentPropertiesEditorViewModel _rectangleRendererComponentPropertiesEditorViewModel;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            var rectangleRendererComponent = new Engine.Rendering.Components.RectangleRendererComponent
            {
                Dimension = new Vector2(1, 2),
                Color = Color.FromArgb(1, 2, 3, 4),
                FillInterior = true,
                Visible = true,
                SortingLayerName = "Test Layer",
                OrderInLayer = 1
            };
            _rectangleRendererComponentModel = new RectangleRendererComponentModel(rectangleRendererComponent);
            _rectangleRendererComponentPropertiesEditorViewModel = new RectangleRendererComponentPropertiesEditorViewModel(_rectangleRendererComponentModel);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dimension_ShouldUpdateRectangleRendererComponentModelDimension()
        {
            // Assume
            Assume.That(_rectangleRendererComponentPropertiesEditorViewModel.Dimension, Is.EqualTo(new Vector2(1, 2)));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.Dimension = new Vector2(123, 456);

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Dimension, Is.EqualTo(new Vector2(123, 456)));
            Assert.That(_rectangleRendererComponentModel.Dimension, Is.EqualTo(new Vector2(123, 456)));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Color_ShouldUpdateRectangleRendererComponentModelColor()
        {
            // Assume
            Assume.That(_rectangleRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(1, 2, 3, 4)));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.Color = Color.FromArgb(11, 22, 33, 44);

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
            Assert.That(_rectangleRendererComponentModel.Color, Is.EqualTo(Color.FromArgb(11, 22, 33, 44)));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void FillInterior_ShouldUpdateRectangleRendererComponentModelFillInterior()
        {
            // Assume
            Assume.That(_rectangleRendererComponentPropertiesEditorViewModel.FillInterior, Is.True);

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.FillInterior = false;

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.FillInterior, Is.False);
            Assert.That(_rectangleRendererComponentModel.FillInterior, Is.False);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Visible_ShouldUpdateRectangleRendererComponentModelVisible()
        {
            // Assume
            Assume.That(_rectangleRendererComponentPropertiesEditorViewModel.Visible, Is.True);

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.Visible = false;

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.Visible, Is.False);
            Assert.That(_rectangleRendererComponentModel.Visible, Is.False);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void SortingLayerName_ShouldUpdateRectangleRendererComponentSortingLayerName()
        {
            // Assume
            Assume.That(_rectangleRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Test Layer"));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.SortingLayerName = "Other Layer";

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.SortingLayerName, Is.EqualTo("Other Layer"));
            Assert.That(_rectangleRendererComponentModel.SortingLayerName, Is.EqualTo("Other Layer"));
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void OrderInLayer_ShouldUpdateRectangleRendererComponentOrderInLayer()
        {
            // Assume
            Assume.That(_rectangleRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(1));

            // Act
            _rectangleRendererComponentPropertiesEditorViewModel.OrderInLayer = 123;

            // Assert
            Assert.That(_rectangleRendererComponentPropertiesEditorViewModel.OrderInLayer, Is.EqualTo(123));
            Assert.That(_rectangleRendererComponentModel.OrderInLayer, Is.EqualTo(123));
        }
    }
}