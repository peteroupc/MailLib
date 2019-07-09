using System;
using NUnit.Framework;
using PeterO;
using PeterO.Mail;
using Test;
//using PeterO.Cbor;

namespace MailLibTest {
  [TestFixture]
  public class MarkdownTest {
    public Message MarkdownMessage(string str) {
      var msg = new Message();
      msg.SetTextBody(str);
      msg.ContentType = MediaType.Parse("text/markdown\u003bcharset=utf-8");
      return msg;
    }

    System.Collections.Generic.List<string> mdone=new System.Collections.Generic.List<string>();

    [OneTimeTearDown]
    public void TearDown(){
       // Console.WriteLine(CBORObject.FromObject(mdone).ToJSONString());
    }

    [Test]
    public void TestMarkdown() {
      var resources = new AppResources("Resources");
      string[] strings = DictUtility.ParseJSONStringArray(
         resources.GetString("markdown"));
      for(var i = 0; i < strings.Length; i += 2){
        Message msg = this.MarkdownMessage(strings[i + 1]);
        Assert.AreEqual(strings[i], msg.GetFormattedBodyString());
      }
    }
  }
}
