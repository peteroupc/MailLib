Older versions release notes
---------------------

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
