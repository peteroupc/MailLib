using System;

namespace PeterO.Mail {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="T:PeterO.Mail.LanguageTags.StringAndQuality"]/*'/>
    public sealed class StringAndQuality {
    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="M:PeterO.Mail.LanguageTags.StringAndQuality.#ctor(System.String,System.Int32)"]/*'/>
      public StringAndQuality(string value, int quality) {
        this.Value = value;
        this.Quality = quality;
      }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.LanguageTags.StringAndQuality.Value"]/*'/>
      public String Value { get; private set; }

    /// <include file='../../docs.xml'
    /// path='docs/doc[@name="P:PeterO.Mail.LanguageTags.StringAndQuality.Quality"]/*'/>
      public int Quality { get; private set; }
    }
}
