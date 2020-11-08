using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering
{
    internal interface IDebugRendererForRenderingSystem
    {
        void DrawDebugInformation(IRenderer2D renderer2D, Matrix3x3 cameraTransformationMatrix);
    }

    internal sealed class DebugRenderer : IDebugRenderer, IDebugRendererForRenderingSystem
    {
        private readonly List<CircleToDraw> _circlesToDraw = new List<CircleToDraw>();

        #region Implementation of IDebugRenderer

        public void DrawCircle(Circle circle, Color color)
        {
            _circlesToDraw.Add(new CircleToDraw(circle, color));
        }

        #endregion

        #region Implementation of IDebugRendererForRenderingSystem

        public void DrawDebugInformation(IRenderer2D renderer2D, Matrix3x3 cameraTransformationMatrix)
        {
            foreach (var circleToDraw in _circlesToDraw)
            {
                renderer2D.RenderEllipse(circleToDraw.Circle.ToEllipse(), circleToDraw.Color, false, cameraTransformationMatrix);
            }

            _circlesToDraw.Clear();
        }

        #endregion

        private readonly struct CircleToDraw
        {
            public CircleToDraw(Circle circle, Color color)
            {
                Circle = circle;
                Color = color;
            }

            public Circle Circle { get; }
            public Color Color { get; }
        }
    }
}