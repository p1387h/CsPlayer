namespace CsPlayer.Shared
{
    public interface ISong
    {
        string FilePath { get; }
        string Name { get; }
        bool Valid { get; set; }
    }
}