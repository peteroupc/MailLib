### RemoveHeader

    public PeterO.Mail.Message RemoveHeader(
        int index);

Not documented yet.

<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.

<b>Returns:</b>

A Message object.

### AddHeader

    public PeterO.Mail.Message AddHeader(
        System.Collections.Generic.KeyValuePair header);

Not documented yet.

<b>Parameters:</b>

 * <i>header</i>: A KeyValuePair object.

<b>Returns:</b>

A Message object.

### AddHeader

    public PeterO.Mail.Message AddHeader(
        string name,
        string value);

Not documented yet.

<b>Parameters:</b>

 * <i>name</i>: A string object.

 * <i>value</i>: A string object. (2).

<b>Returns:</b>

A Message object.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        System.Collections.Generic.KeyValuePair header);

Not documented yet.

<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.

 * <i>header</i>: A KeyValuePair object.

<b>Returns:</b>

A Message object.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        string name,
        string value);

Not documented yet.

<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.

 * <i>name</i>: A string object.

 * <i>value</i>: A string object. (2).

<b>Returns:</b>

A Message object.

### SetHeader

    public PeterO.Mail.Message SetHeader(
        int index,
        string value);

Not documented yet.

<b>Parameters:</b>

 * <i>index</i>: A 32-bit signed integer.

 * <i>value</i>: A string object.

<b>Returns:</b>

A Message object.

### Subject

    public string Subject { get; set;}

Gets or sets this message's subject.

<b>Returns:</b>

This message's subject.


