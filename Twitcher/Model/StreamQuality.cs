using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Twitcher.Model
{
    public class StreamQuality
    {
        private readonly string _name;
        private readonly int _value;

        public static readonly StreamQuality Low = new StreamQuality(1, "Low");
        public static readonly StreamQuality Medium = new StreamQuality(2, "Medium");
        public static readonly StreamQuality High = new StreamQuality(3, "High");
        public static readonly StreamQuality Source = new StreamQuality(4, "Source");

        private StreamQuality(int value, string name)
        {
            _name = name;
            _value = value;
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
