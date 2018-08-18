using System;

namespace PeterO.Mail {
    /// <summary>Not documented yet.</summary>
    public sealed class StringAndQuality {
    /// <summary>Initializes a new instance of the StringAndQuality
    /// class.</summary>
    /// <param name='value'>A string object.</param>
    /// <param name='quality'>A 32-bit signed integer.</param>
      public StringAndQuality(string value, int quality) {
        this.Value = value;
        this.Quality = quality;
      }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public String Value { get; private set; }

    /// <summary>Gets a value not documented yet.</summary>
    /// <value>A value not documented yet.</value>
      public int Quality { get; private set; }
    }
}
