MailLib
=======

If you like this software, consider donating to me at this link: http://upokecenter.com/d/

----

A portable library in C# and Java for parsing Internet mail messages.

Documentation
------------

See the [Wiki](https://github.com/peteroupc/MailLib/wiki) for Java API documentation.

See [docs/APIDocs.md](https://github.com/peteroupc/MailLib/blob/master/docs/APIDocs.md) for C# (.NET) API documentation.

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
      <version>0.7.0</version>
    </dependency>

In other Java-based environments, the library can be referred to by its
group ID (`com.upokecenter`), artifact ID (`maillib`), and version, as given above.

Source Code
---------
Source code is available in the [project page](https://github.com/MailLib/CBOR).

Release Notes
---------
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
http://creativecommons.org/publicdomain/zero/1.0/

If you like this, you should donate to Peter O.
at: http://peteroupc.github.io/MailLib/
