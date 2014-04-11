/*
 * Created by SharpDevelop.
 * User: Peter
 * Date: 3/31/2014
 * Time: 3:18 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace PeterO.Mail
{
  internal interface IHeaderFieldParser {
    bool IsStructured();

    string DowngradeFieldValue(string str);

    string ReplaceEncodedWords(string str);
  }
}
