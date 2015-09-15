using System.Collections.Generic;

namespace Twitcher.Model
{
    public class M3uPlaylist
    {
        #region Declaration

        #region Fields

        private IDictionary<StreamQuality, string> links;
        private string rawContent;
        #endregion Fields

        #region Properties
        public IDictionary<StreamQuality, string> Links
        {
            get
            {
                if (links == null || links.Values.Count < 4)
                {
                    links = new Dictionary<StreamQuality, string>();
                    string[] lines = rawContent.Split( '\n' );
                    StreamQuality nextQuality = null;
                    foreach (string line in lines)
                    {
                        if (line.IndexOf( StreamQuality.Source.ToString() ) > 0)
                            nextQuality = StreamQuality.Source;
                        if (line.IndexOf( StreamQuality.High.ToString() ) > 0)
                            nextQuality = StreamQuality.High;
                        if (line.IndexOf( StreamQuality.Medium.ToString() ) > 0)
                            nextQuality = StreamQuality.Medium;
                        if (line.IndexOf( StreamQuality.Low.ToString() ) > 0)
                            nextQuality = StreamQuality.Low;

                        if (line.StartsWith( "http" ) && !links.ContainsKey(nextQuality))
                            links.Add( nextQuality, line );
                    }
                }
                return links;
            }
        }
        #endregion Properties

        public M3uPlaylist(string data )
        {
            this.rawContent = data;
        }

        #endregion Declaration
    }
}