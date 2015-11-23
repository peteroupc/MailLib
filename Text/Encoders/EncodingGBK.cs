using System;
using System.IO;
using PeterO;

using PeterO.Text;

namespace PeterO.Text.Encoders {
  internal class EncodingGBK : ICharacterEncoding {
    private readonly ICharacterEncoder enc = EncodingGB18030.GetEncoder2(true);

    public ICharacterDecoder GetDecoder() {
      return EncodingGB18030.GetDecoder2();
    }

    public ICharacterEncoder GetEncoder() {
      return this.enc;
    }
  }
}
