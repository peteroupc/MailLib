using System;

namespace PeterO.Mail {
    /// <summary>Stores an arbitrary string and a "quality value" for that
    /// string. For instance, the string can be a language tag, and the
    /// "quality value" can be the degree of preference for that
    /// language.</summary>
    public sealed class StringAndQuality {
    /// <summary>Initializes a new instance of the
    /// <see cref='PeterO.Mail.StringAndQuality'/> class.</summary>
    /// <param name='value'>An arbitrary text string.</param>
    /// <param name='quality'>A 32-bit signed integer serving as the
    /// "quality" value.</param>
      public StringAndQuality(string value, int quality) {
        this.Value = value;
        this.Quality = quality;
      }

    /// <summary>Gets the arbitrary string stored by this object.</summary>
    /// <value>The arbitrary string stored by this object.</value>
      public String Value { get; private set; }

    /// <summary>Gets the quality value stored by this object.</summary>
    /// <value>The quality value stored by this object.</value>
      public int Quality { get; private set; }
    }
}
