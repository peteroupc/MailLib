## PeterO.Mail.StringAndQuality

    public sealed class StringAndQuality

Stores an arbitrary string and a "quality value" for that string. For instance, the string can be a language tag, and the "quality value" can be the degree of preference for that language.

### StringAndQuality Constructor

    public StringAndQuality(
        string value,
        int quality);

Initializes a new instance of the [PeterO.Mail.StringAndQuality](PeterO.Mail.StringAndQuality.md) class.

<b>Parameters:</b>

 * <i>value</i>: An arbitrary text string.

 * <i>quality</i>: A 32-bit signed integer serving as the "quality" value.

### Quality

    public int Quality { get; }

Gets the quality value stored by this object.

<b>Returns:</b>

The quality value stored by this object.

### Value

    public string Value { get; }

Gets the arbitrary string stored by this object.

<b>Returns:</b>

The arbitrary string stored by this object.