## PeterO.Mail.StringAndQuality

    public sealed class StringAndQuality

 Stores an arbitrary string and a "quality value" for that string. For instance, the string can be a language tag, and the "quality value" can be the degree of preference for that language.  ### Member Summary
* <code>[Quality](#Quality)</code> - Gets the quality value stored by this object.
* <code>[Value](#Value)</code> - Gets the arbitrary string stored by this object.

<a id="Void_ctor_String_Int32"></a>
### StringAndQuality Constructor

    public StringAndQuality(
        string value,
        int quality);

 Initializes a new instance of the [PeterO.Mail.StringAndQuality](PeterO.Mail.StringAndQuality.md) class.    <b>Parameters:</b>

 * <i>value</i>:  The parameter  <i>value</i>
 is a text string.

 * <i>quality</i>:  The parameter  <i>quality</i>
 is a 32-bit signed integer.

<a id="Quality"></a>
### Quality

    public int Quality { get; }

 Gets the quality value stored by this object.  <b>Returns:</b>

The quality value stored by this object.

<a id="Value"></a>
### Value

    public string Value { get; }

 Gets the arbitrary string stored by this object.  <b>Returns:</b>

The arbitrary string stored by this object.
