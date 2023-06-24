namespace Geisha.Engine.Rendering.Systems
{
    internal interface IRenderNodeVisitor
    {
        void Visit(RenderNode node);
        void Visit(EllipseNode node);
        void Visit(RectangleNode node);
        void Visit(SpriteNode node);
        void Visit(TextNode node);
    }
}