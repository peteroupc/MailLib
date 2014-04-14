/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 4/13/2014
 * Time: 11:47 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace PeterO.Mail
{
  internal interface IStringEncoder
  {
    void WriteToString(StringBuilder str, byte[] data, int offset, int count);

    void FinalizeEncoding(StringBuilder str);
  }
}
