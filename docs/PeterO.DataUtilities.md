## PeterO.DataUtilities

    public static class DataUtilities

### CodePointAt

    public static int CodePointAt(
        string str,
        int index);

### CodePointAt

    public static int CodePointAt(
        string str,
        int index,
        int surrogateBehavior);

### CodePointBefore

    public static int CodePointBefore(
        string str,
        int index);

### CodePointBefore

    public static int CodePointBefore(
        string str,
        int index,
        int surrogateBehavior);

### CodePointCompare

    public static int CodePointCompare(
        string strA,
        string strB);

### GetUtf8Bytes

    public static byte[] GetUtf8Bytes(
        string str,
        bool replace);

### GetUtf8Bytes

    public static byte[] GetUtf8Bytes(
        string str,
        bool replace,
        bool lenientLineBreaks);

### GetUtf8Length

    public static long GetUtf8Length(
        string str,
        bool replace);

### GetUtf8String

    public static string GetUtf8String(
        byte[] bytes,
        bool replace);

### GetUtf8String

    public static string GetUtf8String(
        byte[] bytes,
        int offset,
        int bytesCount,
        bool replace);

### ReadUtf8

    public static int ReadUtf8(
        System.IO.Stream stream,
        int bytesCount,
        System.Text.StringBuilder builder,
        bool replace);

### ReadUtf8FromBytes

    public static int ReadUtf8FromBytes(
        byte[] data,
        int offset,
        int bytesCount,
        System.Text.StringBuilder builder,
        bool replace);

### ReadUtf8ToString

    public static string ReadUtf8ToString(
        System.IO.Stream stream);

### ReadUtf8ToString

    public static string ReadUtf8ToString(
        System.IO.Stream stream,
        int bytesCount,
        bool replace);

### ToLowerCaseAscii

    public static string ToLowerCaseAscii(
        string str);

### WriteUtf8

    public static int WriteUtf8(
        string str,
        int offset,
        int length,
        System.IO.Stream stream,
        bool replace);

### WriteUtf8

    public static int WriteUtf8(
        string str,
        int offset,
        int length,
        System.IO.Stream stream,
        bool replace,
        bool lenientLineBreaks);

### WriteUtf8

    public static int WriteUtf8(
        string str,
        System.IO.Stream stream,
        bool replace);
