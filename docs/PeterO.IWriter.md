## PeterO.IWriter

    public interface IWriter :
        PeterO.IByteWriter

A generic interface for writing bytes of data.

### Write

    void Write(
        byte[] bytes,
        int offset,
        int length);

Writes a portion of a byte array to the data source.

<b>Parameters:</b>

 * <i>bytes</i>: A byte array containing the data to write.

 * <i>offset</i>: A zero-based index showing where the desired portion of  <i>bytes</i>
 begins.

 * <i>length</i>: The number of elements in the desired portion of  <i>bytes</i>
 (but not more than  <i>bytes</i>
 's length).

<b>Exceptions:</b>

 * System.ArgumentNullException:
Should be thrown if the parameter "bytes" is null.

 * System.ArgumentException:
Should be thrown if either "offset" or "length" is less than 0 or greater than "bytes" 's length, or "bytes" 's length minus "offset" is less than "length".
