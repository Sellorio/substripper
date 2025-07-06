namespace Sellorio.Substripper.Models.Subtitles
{
    /// <remarks>
    /// Example command
    /// ffmpeg -i input.mkv -map 0 -c copy -disposition:s:0 0 -disposition:s:1 default output.mkv
    /// Clear disposition: -disposition:s:0 0
    /// Set disposition: -disposition:s:1 default
    /// </remarks>
    public class SubtitleDispositionEdit(int sourceIndex, int sourceSubtitleIndex, Disposition newDisposition)
    {
        public int SourceIndex => sourceIndex;
        public int SourceSubtitleIndex => sourceSubtitleIndex;
        public Disposition NewDisposition => newDisposition;
    }
}
