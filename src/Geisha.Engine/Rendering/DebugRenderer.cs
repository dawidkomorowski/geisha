using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering
{
    internal interface IDebugRendererForRenderingSystem
    {
        void DrawDebugInformation(IRenderer2D renderer2D, in Matrix3x3 cameraTransformationMatrix);
    }

    internal sealed class DebugRenderer : IDebugRenderer, IDebugRendererForRenderingSystem
    {
        private readonly List<CircleToDraw> _circlesToDraw = new List<CircleToDraw>();
        private readonly List<RectangleToDraw> _rectanglesToDraw = new List<RectangleToDraw>();

        #region Implementation of IDebugRenderer

        public void DrawCircle(in Circle circle, Color color)
        {
            _circlesToDraw.Add(new CircleToDraw(circle, color));
        }

        public void DrawRectangle(in AxisAlignedRectangle rectangle, Color color, in Matrix3x3 transform)
        {
            _rectanglesToDraw.Add(new RectangleToDraw(rectangle, color, transform));
        }

        #endregion

        #region Implementation of IDebugRendererForRenderingSystem

        public void DrawDebugInformation(IRenderer2D renderer2D, in Matrix3x3 cameraTransformationMatrix)
        {
            foreach (var circleToDraw in _circlesToDraw)
            {
                renderer2D.RenderEllipse(circleToDraw.Circle.ToEllipse(), circleToDraw.Color, false, cameraTransformationMatrix);
            }

            _circlesToDraw.Clear();

            foreach (var rectangleToDraw in _rectanglesToDraw)
            {
                var finalTransform = cameraTransformationMatrix * rectangleToDraw.Transform;
                renderer2D.RenderRectangle(rectangleToDraw.Rectangle, rectangleToDraw.Color, false, finalTransform);
            }

            _rectanglesToDraw.Clear();
        }

        #endregion

        private readonly struct CircleToDraw
        {
            public CircleToDraw(in Circle circle, Color color)
            {
                Circle = circle;
                Color = color;
            }

            public Circle Circle { get; }
            public Color Color { get; }
        }

        private readonly struct RectangleToDraw
        {
            public RectangleToDraw(in AxisAlignedRectangle rectangle, Color color, in Matrix3x3 transform)
            {
                Rectangle = rectangle;
                Color = color;
                Transform = transform;
            }

            public AxisAlignedRectangle Rectangle { get; }
            public Color Color { get; }
            public Matrix3x3 Transform { get; }
        }
    }
}