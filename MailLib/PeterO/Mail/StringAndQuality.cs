using System;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.StringAndQuality"]/*'/>
    public sealed class StringAndQuality {
    /// <summary>Initializes a new instance of the <see cref='StringAndQuality'/> class.</summary>
    /// <param name='value'>
    /// The parameter
    /// <paramref name='value'/>
    /// is a text string.
    /// </param>
    /// <param name='quality'>
    /// The parameter
    /// <paramref name='quality'/>
    /// is a 32-bit signed integer.
    /// </param>
      public StringAndQuality(string value, int quality) {
        this.Value = value;
        this.Quality = quality;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.StringAndQuality.Value"]/*'/>
      public String Value { get; private set; }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.StringAndQuality.Quality"]/*'/>
      public int Quality { get; private set; }
    }
}
