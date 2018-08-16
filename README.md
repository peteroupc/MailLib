MailLib
=======

[![NuGet Status](http://img.shields.io/nuget/v/PeterO.MailLib.svg?style=flat)](https://www.nuget.org/packages/PeterO.MailLib)
[![Maven Central](https://img.shields.io/maven-central/v/com.upokecenter/maillib.svg?style=plastic)](https://search.maven.org/#search|ga|1|g%3A%22com.upokecenter%22%20AND%20a%3A%22maillib%22)

**Download source code: [ZIP file](https://github.com/peteroupc/MailLib/archive/master.zip)**

If you like this software, consider donating to me at this link: [http://peteroupc.github.io/](http://peteroupc.github.io/)

----

A portable library in C# and Java for parsing and generating Internet mail messages.

Note that sending and receiving email messages is outside the scope of this library; however, an application that sends and/or receives such messages can use this library to help interpret those messages.

Documentation
------------

**See the [Java API documentation](https://peteroupc.github.io/MailLib/api/).**

**See the [C# (.NET) API documentation](https://peteroupc.github.io/MailLib/docs/).**

How to Install
---------
Starting with version 0.6.1, the C# implementation is available in the
NuGet Package Gallery under the name
[PeterO.MailLib](https://www.nuget.org/packages/PeterO.MailLib). To install
this library as a NuGet package, enter `Install-Package PeterO.MailLib` in the
NuGet Package Manager Console.

Starting with version 0.6.1, the Java implementation is available
as an [artifact](https://search.maven.org/#search|ga|1|g%3A%22com.upokecenter%22%20AND%20a%3A%22maillib%22) in the Central Repository. To add this library to a Maven
project, add the following to the `dependencies` section in your `pom.xml` file:

    <dependency>
      <groupId>com.upokecenter</groupId>
      <artifactId>maillib</artifactId>
      <version>0.12.0</version>
    </dependency>

In other Java-based environments, the library can be referred to by its
group ID (`com.upokecenter`), artifact ID (`maillib`), and version, as given above.

Source Code
---------
Source code is available in the [project page](https://github.com/peteroupc/MailLib).

Example
---------

An example of reading an email message from a file:

    // Create a file stream from the email message file
    using(var file = new System.IO.FileStream("email.eml",System.IO.FileMode.Read)){
      // Read the email message
      var message = new Message(file);
      // Output each address in the From header
      foreach(var addr in message.FromAddresses){
         Console.WriteLine("From: "+addr);
      }
      // Output each address in the To header
      foreach(var addr in message.ToAddresses){
         Console.WriteLine("To: "+addr);
      }
      // Output the message's text
      Console.WriteLine(message.BodyString);
    }

Release Notes
---------

Version 0.12.0:

- The internal code for parsing and generating message headers was extensively refactored.  Much of the refactoring improves conformance with RFCs that define the message format and MIME.
- As a result of the refactoring, the message generation can better ensure, when possible, that each line of a generated message is no more than 78 characters long, as recommended by those RFCs.
- Many bugs were fixed, including those relating to downgrading header fields containing non-ASCII characters to ASCII in the Generate method.  Another bug involves an infinite decoding loop involving certain Content-Type and Content-Disposition strings.
- One additional deviation to RFC 2047 (encoded words) is noted in the documentation for the Message class.  I think the deviation shows a weakness in that RFC.
- At least three methods added to ContentDisposition class, including ToSingleLineString.
- ToSingleLineString method added to MediaType class.
- GenerateBytes method added to Message class.
- Added MailDateTime class.
- MakeFilename class in ContentDisposition is now idempotent and shows better. performance in some common cases of file names.

Version 0.11.0:

- Corrected the documentation for MediaType's and ContentDisposition's Parameters properties
- Added DisplayName property in NamedAddress class
- More robust ContentDisposition.MakeFilename method
- Update Unicode data
- Converted to .NET Standard
- Updated Encoding library used
- Supported parsing several new header fields
- Bug fixes

See [History.md](https://github.com/peteroupc/MailLib/tree/master/History.md)
for release notes for older versions.

About
-----------

Written in 2013-2014 by Peter O.

Any copyright is dedicated to the Public Domain.
[http://creativecommons.org/publicdomain/zero/1.0/](http://creativecommons.org/publicdomain/zero/1.0/)
(For exceptions, see  [LICENSE.md](https://github.com/peteroupc/MailLib/blob/master/LICENSE.md).)

If you like this, you should donate to Peter O.
at: [http://peteroupc.github.io/](http://peteroupc.github.io/)
