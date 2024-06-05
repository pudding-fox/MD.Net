namespace MD.Net
{
    public interface IActionBuilder
    {
        IActions GetActions(IDevice device, IDisc currentDisc, IDisc updatedDisc);
    }
}
