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
      <version>0.10.0</version>
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

Version 0.10.0:

- Character encoding conversion library moved out of this library (see [PeterO.Encoding](https://github.com/peteroupc/Encoding); currently uses version 2.0 of that library.
- Updated normalization and IDNA data to Unicode 9.0
- Normalization implementation renamed to NormalizerInput from NormalizerCharacterInput; the latter class is now deprecated
- Add GetHeaderArray, SetDate, and GetDate methods of Message class
- Add support for parsing several new header fields
- Date header field is included in generated messages
- Improve MakeFilename method of ContentDisposition class
- Fix bugs in Unicode normalization implementation
- Fixed several bugs and addressed corner cases in message generation, such as Quoted-Printable encoding
- In the .NET version of the source code, documentation is moved out of the source code and placed into a consolidated XML file
- SetBody method of Message class now returns the Message object that was edited
- Other bug fixes

Version 0.9.2:

- Removed dependencies (.NET version)

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
