## PeterO.Mail.StringAndQuality

    public sealed class StringAndQuality

Stores an arbitrary string and a "quality value" for that string. For instance, the string can be a language tag, and the "quality value" can be the degree of preference for that language.


### Member Summary
* <code>[Quality](#Quality)</code> - Gets the quality value stored by this object.
* <code>[Value](#Value)</code> - Gets the arbitrary string stored by this object.

<a id="Void_ctor_String_Int32"></a>

### Quality

    public int Quality { get; }

Gets the quality value stored by this object.

<b>Returns:</b>

The quality value stored by this object.


<a id="Value"></a>

### Value

    public string Value { get; }

Gets the arbitrary string stored by this object.

<b>Returns:</b>

The arbitrary string stored by this object.
