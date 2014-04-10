/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/9/2014
 * Time: 11:54 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PeterO.Mail
{
    /// <summary/>
  public class MessageDataException : Exception
  {
    public MessageDataException() {
    }

     public MessageDataException(string message) : base(message) {
    }

    public MessageDataException(string message, Exception innerException) : base(message, innerException) {
    }
  }
}
