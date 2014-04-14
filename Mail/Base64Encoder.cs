/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/14/2014
 * Time: 12:06 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using PeterO;

namespace PeterO.Mail
{
    /// <summary>Encodes binary data in Base64.</summary>
  internal sealed class Base64Encoder : IStringEncoder
  {
    private const string Base64Classic = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

    private int lineCount;
    private int quantumCount;
    private int byte1;
    private int byte2;
    private bool padding;
    private bool lenientLineBreaks;
    private bool haveCR;

    public Base64Encoder(bool padding, bool lenientLineBreaks) {
      this.padding = padding;
      this.lenientLineBreaks = lenientLineBreaks;
      this.byte1 = -1;
      this.byte2 = -1;
    }

    private void LineAwareAppend(StringBuilder sb, char c) {
      ++this.lineCount;
      if (this.lineCount > 76) {
        sb.Append("\r\n");
        this.lineCount = 0;
      }
      sb.Append(c);
    }

    private void AddByteInternal(StringBuilder str, byte b) {
      int ib = ((int)b) & 0xff;
      if (this.quantumCount == 2) {
        this.LineAwareAppend(str, Base64Classic[(this.byte1 >> 2) & 63]);
        this.LineAwareAppend(str, Base64Classic[((this.byte1 & 3) << 4) + ((this.byte2 >> 4) & 15)]);
        this.LineAwareAppend(str, Base64Classic[((this.byte2 & 15) << 2) + ((ib >> 6) & 3)]);
        this.LineAwareAppend(str, Base64Classic[ib & 63]);
        this.byte1 = -1;
        this.byte2 = -1;
        this.quantumCount = 0;
      } else if (this.quantumCount == 1) {
        this.byte2 = ib;
        this.quantumCount = 2;
      } else {
        this.byte1 = ib;
        this.quantumCount = 1;
      }
    }

    private void AddByte(StringBuilder str, byte b) {
      if (b == 0x0d && this.lenientLineBreaks) {
        // CR
        this.haveCR = true;
        this.AddByteInternal(str, 0x0d);
        this.AddByteInternal(str, 0x0a);
      } else if (b == 0x0a && this.lenientLineBreaks && !this.haveCR) {
        // bare LF
        this.AddByteInternal(str, 0x0d);
        this.AddByteInternal(str, 0x0a);
        this.haveCR = false;
      } else if (b == 0x0a && this.lenientLineBreaks) {
        // Do nothing, this is an LF that follows CR
        this.haveCR = false;
      } else {
        this.AddByteInternal(str, b);
        this.haveCR = false;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A StringBuilder object.</param>
    public void FinalizeEncoding(StringBuilder str) {
      if (this.byte1 < 0 || this.byte2 < 0) {  // if one or two bytes are left over
        string alphabet = Base64Classic;
        this.LineAwareAppend(str, alphabet[(this.byte1 >> 2) & 63]);
        if (this.byte2 >= 0) {  // if two bytes are left over
          this.LineAwareAppend(str, alphabet[((this.byte1 & 3) << 4) + ((this.byte2 >> 4) & 15)]);
          this.LineAwareAppend(str, alphabet[(this.byte2 & 15) << 2]);
          if (this.padding) {
            this.LineAwareAppend(str, '=');
          }
        } else {
          this.LineAwareAppend(str, alphabet[(this.byte1 & 3) << 4]);
          if (this.padding) {
            this.LineAwareAppend(str, '=');
            this.LineAwareAppend(str, '=');
          }
        }
        this.byte1 = 0;
        this.byte2 = 0;
      }
    }

    /// <summary>Not documented yet.</summary>
    /// <param name='str'>A StringBuilder object.</param>
    /// <param name='data'>A byte array.</param>
    /// <param name='offset'>A 32-bit signed integer.</param>
    /// <param name='count'>A 32-bit signed integer. (2).</param>
    public void WriteToString(StringBuilder str, byte[] data, int offset, int count) {
      if (str == null) {
        throw new ArgumentNullException("str");
      }
      if (data == null) {
        throw new ArgumentNullException("data");
      }
      if (offset < 0) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (offset > data.Length) {
        throw new ArgumentException("offset (" + Convert.ToString((long)offset, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (count < 0) {
        throw new ArgumentException("count (" + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + "0");
      }
      if (count > data.Length) {
        throw new ArgumentException("count (" + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture) + ") is more than " + Convert.ToString((long)data.Length, System.Globalization.CultureInfo.InvariantCulture));
      }
      if (data.Length - offset < count) {
        throw new ArgumentException("data's length minus " + offset + " (" + Convert.ToString((long)(data.Length - offset), System.Globalization.CultureInfo.InvariantCulture) + ") is less than " + Convert.ToString((long)count, System.Globalization.CultureInfo.InvariantCulture));
      }
      for (int i = 0; i < count; ++i) {
        this.AddByte(str, data[offset + i]);
      }
    }
  }
}
