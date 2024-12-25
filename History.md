Older versions release notes
---------------------

Version 0.10.0:

- Character encoding conversion library moved out of this library (see [PeterO.Encoding](https://github.com/peteroupc/Encoding); currently uses version 0.2.0 of that library.
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
- Other issue fixes

Version 0.9.2:

- Removed dependencies (.NET version)

Version 0.9.1:

- Removed a reference to OpenCover that was apparently left in version 0.9 (.NET version)

Version 0.9:

- The C# version of the library now also targets "dotnet", which should make it compatible with platform .NET runtime
environments such as the upcoming cross-platform "coreclr" runtime.
- Issue fixes to ensure number-format independence in some cases
- ContentDisposition.MakeFilename implementation was improved
- Another overload for GetEncoding was added
- Additional fixes in character encodings
- Known issue: The library implements character encodings based on the Encoding Standard
candidate recommendation.  But several issues have emerged with that specification in the meantime.
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

The [commit history](https://github.com/peteroupc/MailLib/commits/master)
contains details on code changes in previous versions.
