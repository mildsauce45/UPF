namespace FirstWave.Unity.Gui.Bridge
{
    /// <summary>
    /// Primary interface for loading textures/music/etc out of the resources folder without "designability"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IResourceLoader<T>
    {
        T LoadResource(string name);
    }
}
