using System;
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
        #region Implementation of IDebugRenderer

        public void DrawCircle(Circle circle, Color color)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IDebugRendererForRenderingSystem

        public void DrawDebugInformation(IRenderer2D renderer2D, Matrix3x3 cameraTransformationMatrix)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}