using Task6Itransition.Services.Drawers.Interfaces;

namespace Task6Itransition.Services.Drawers
{
    public static class DrawerFactory
    {
        public static IDrawer? GetDrawer(string type)
        {
            Type? parsedType = DrawerTypeParser.Parse(type);
            if(parsedType is null) return null; 
            var item = Activator.CreateInstance(parsedType);
            if (item is not IDrawer) throw new ArgumentException($"type {type} is not implements \"IDrawer\"");
            return (IDrawer)item;
        }
    }
}
