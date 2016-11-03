MailLib
=======

[![NuGet Status](http://img.shields.io/nuget/v/PeterO.MailLib.svg?style=flat)](https://www.nuget.org/packages/PeterO.MailLib)
[![Maven Central](https://img.shields.io/maven-central/v/com.upokecenter/maillib.svg?style=plastic)](https://search.maven.org/#search|ga|1|g%3A%22com.upokecenter%22%20AND%20a%3A%22maillib%22)

**Download source code: [ZIP file](https://github.com/peteroupc/MailLib/archive/master.zip)**

If you like this software, consider donating to me at this link: [http://peteroupc.github.io/](http://peteroupc.github.io/)

----

A portable library in C# and Java for parsing and generating Internet mail messages.

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
      <version>0.9.0</version>
    </dependency>

(The .NET version is currently version 0.9.1.)

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

Version 0.9.1:

- Removed a reference to OpenCover that was apparently left in version 0.9 (.NET version)

Version 0.9:

- The C# version of the library now also targets "dotnet", which should make it compatible with platform .NET runtime
environments such as the upcoming cross-platform "coreclr" runtime.
- Bug fixes to ensure number-format independence in some cases
- ContentDisposition.MakeFilename implementation was improved
- Another overload for GetEncoding was added
- Additional fixes in character encodings
- Known issue: The library implements character encodings based on the Encoding Standard
candidate recommendation.  But several issues have emerged with that spec in the meantime.
For example, GB18030 currently uses a problematic range table, so that for certain code points designed
for that table, round-tripping is not possible.  As a result, tests on GB18030 are disabled
for now.

Version 0.8.1:

- Fixes bugs in character encoding algorithms

Version 0.8.0:

- New helper classes added:
- - ArrayWriter
- -  Data abstraction interfaces IByteWriter, IReader, IWriter
- -  Encoding namespace, to access character encodings
- Normalizer class deprecated; use NormalizingCharacterInput
  instead
- Updated normalization and IDNA data to Unicode 8.0
- Fixed wrongly generated data that can make normalization form NFKC
  behave incorrectly
- Parses three new structured header fields
- ContentDisposition.MakeFilename decodes RFC2047 words
  in more cases and avoids returning unsuitable filenames in more cases
- Adds byte[] constructor to Message class
- Be more lenient for some "message/rfc822" bodies
- Fix bug in header parsing if a header field ends in folding whitespace
- Some parts of the code were rewritten, such as the Quoted-Printable
  encoder

Version 0.7.0:

- More header fields that can be merged are merged when generating messages from Message objects.
- The Unicode data for normalization and IDNA was updated to version 7.0.

Version 0.6.1:

- First release as an artifact and a NuGet package.

The [commit history](https://github.com/peteroupc/MailLIb/commits/master)
contains details on code changes in previous versions.

About
-----------

Written in 2013-2014 by Peter O.

Any copyright is dedicated to the Public Domain.
[http://creativecommons.org/publicdomain/zero/1.0/](http://creativecommons.org/publicdomain/zero/1.0/)
(For exceptions, see  [LICENSE.md](https://github.com/peteroupc/MailLib/blob/master/LICENSE.md).)

If you like this, you should donate to Peter O.
at: [http://peteroupc.github.io/](http://peteroupc.github.io/)
