
/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

var Lz4 = {};
var NormalizationData=require("./NormalizationData").NormalizationData;
var IdnaData=require("./IdnaData").IdnaData;

Lz4.Decompress = function(input){
  var index=0;
  var ms=[];
  while(index<input.length){
    var b=input[index];
    var literalLength=(b>>4)&15;
    var matchLength=b&15;
    ++index;
    // Literal
    if(literalLength==15){
          while (index < input.length) {
            b = input[index] & 0xff;
            literalLength += b;
            ++index;
            if (b != 255) {
              break;
            }
            if(index>=input.length){
           throw new Error("Invalid LZ4")
            }
          }
          }
        if (index + literalLength - 1 >= input.length) {
           throw new Error("Invalid LZ4")
        }
        if (literalLength > 0) {
          for(var k=0;k<literalLength;k++){
            ms.push(input[index+k])
          }
          index += literalLength;
        }
        if (index == input.length) {
          break;
        }
        if (index + 1 >= input.length) {
           throw new Error("Invalid LZ4")
        }
        // Match copy
        var offset = (input[index]) & 0xff;
        offset |= ((input[index + 1]) & 0xff) << 8;
        index += 2;
        if (offset == 0) {
           throw new Error("Invalid LZ4")
        }
        if (matchLength == 15) {
          while (index < input.length) {
            b = (input[index]) & 0xff;
            matchLength += b;
            ++index;
            if (b != 255) {
              break;
            }
            if (index >= input.length) {
           throw new Error("Invalid LZ4")
            }
          }
        }
        matchLength += 4;
        var pos = ms.length - offset;
        if (pos < 0) {
           throw new Error("Invalid LZ4")
        }
        if (matchLength > offset) {
           throw new Error("Invalid LZ4")
        }
        for(var k=0;k<matchLength;k++){
            ms.push(ms[pos+k])
        }
      }
//      if(typeof Uint8Array!="undefined")ms=new Uint8Array(ms)
      return ms;
}

var ByteData = function(array) {

    this.array = array;
};
(function(constructor,prototype){
    prototype.array = null;
    constructor.Decompress = function(data) {
        return new ByteData(Lz4.Decompress(data));
    };

    prototype.GetBoolean = function(cp) {
        if (cp < 0) {
            throw new Error("cp (" + ((cp|0) + "") + ") is less than " + "0");
        }
        if (cp > 1114111) {
            throw new Error("cp (" + ((cp|0) + "") + ") is more than " + ((1114111) + ""));
        }
        var b = this.array[cp >> 13] & 255;
        switch(b) {
            case 254:
                return false;
            case 255:
                return true;
            default:
                {
                    var t = cp & 8191;
                    var index = 136 + (b << 10) + (t >> 3);
                    return (this.array[index] & (1 << (t & 7))) > 0;
                }
        }
    };

    prototype.GetByte = function(cp) {
        if (cp < 0) {
            throw new Error("cp (" + ((cp|0) + "") + ") is less than " + "0");
        }
        if (cp > 1114111) {
            throw new Error("cp (" + ((cp|0) + "") + ") is more than " + ((1114111) + ""));
        }
        var index = (cp >> 9) << 1;
        var x = this.array[index + 1];
        if ((x & 128) != 0) {

            return this.array[index];
        } else {
            x = (x << 8) | ((this.array[index]) & 255);

            index = 4352 + (x << 9) + (cp & 511);
            return this.array[index];
        }
    };
})(ByteData,ByteData.prototype);


var UnicodeDatabase = function() {

};
(function(constructor,prototype){
    constructor.classes = null;
    constructor.GetCombiningClass = function(cp) {
        {
            if (UnicodeDatabase.classes == null) {
                UnicodeDatabase.classes = ByteData.Decompress(NormalizationData.CombiningClasses);
            }
        }
        return ((UnicodeDatabase.classes.GetByte(cp))|0) & 255;
    };
    constructor.idnaCat = null;
    constructor.GetIdnaCategory = function(cp) {
        {
            if (UnicodeDatabase.idnaCat == null) {
                UnicodeDatabase.idnaCat = ByteData.Decompress(IdnaData.IdnaCategories);
            }
        }
        return ((UnicodeDatabase.idnaCat.GetByte(cp))|0) & 255;
    };
    constructor.combmark = null;
    constructor.IsCombiningMark = function(cp) {
        {
            if (UnicodeDatabase.combmark == null) {
                UnicodeDatabase.combmark = ByteData.Decompress(IdnaData.CombiningMarks);
            }
            return UnicodeDatabase.combmark.GetBoolean(cp);
        }
    };
    constructor.stablenfc = null;
    constructor.stablenfd = null;
    constructor.stablenfkc = null;
    constructor.stablenfkd = null;
    constructor.IsStableCodePoint = function(cp, form) {
        {
            if (form == Normalization.NFC) {
                if (UnicodeDatabase.stablenfc == null) {
                    UnicodeDatabase.stablenfc = ByteData.Decompress(NormalizationData.StableNFC);
                }
                return UnicodeDatabase.stablenfc.GetBoolean(cp);
            }
            if (form == Normalization.NFD) {
                if (UnicodeDatabase.stablenfd == null) {
                    UnicodeDatabase.stablenfd = ByteData.Decompress(NormalizationData.StableNFD);
                }
                return UnicodeDatabase.stablenfd.GetBoolean(cp);
            }
            if (form == Normalization.NFKC) {
                if (UnicodeDatabase.stablenfkc == null) {
                    UnicodeDatabase.stablenfkc = ByteData.Decompress(NormalizationData.StableNFKC);
                }
                return UnicodeDatabase.stablenfkc.GetBoolean(cp);
            }
            if (form == Normalization.NFKD) {
                if (UnicodeDatabase.stablenfkd == null) {
                    UnicodeDatabase.stablenfkd = ByteData.Decompress(NormalizationData.StableNFKD);
                }
                return UnicodeDatabase.stablenfkd.GetBoolean(cp);
            }
            return false;
        }
    };
    constructor.decomps = null;
    constructor.GetDecomposition = function(cp, compat, buffer, offset) {
        if (cp < 128) {

            buffer[offset++] = cp;
            return offset;
        }
        UnicodeDatabase.decomps = NormalizationData.DecompMappings;
        var left = 0;
        var right = UnicodeDatabase.decomps[0] - 1;
        while (left <= right) {
            var index = (left + right) >> 1;
            var realIndex = 1 + (index << 1);
            if (UnicodeDatabase.decomps[realIndex] == cp) {
                var data = UnicodeDatabase.decomps[realIndex + 1];
                if ((data & (1 << 23)) > 0 && !compat) {
                    buffer[offset++] = cp;
                    return offset;
                }
                if ((data & (1 << 22)) > 0) {

                    buffer[offset++] = data & 2097151;
                    return offset;
                }
                var size = data >> 24;
                if (size > 0) {
                    if ((data & (1 << 23)) > 0) {
                        realIndex = data & 2097151;
                        for (var arrfillI = 0; arrfillI < size; arrfillI++) buffer[offset + arrfillI] = NormalizationData.CompatDecompMappings[realIndex + arrfillI];
                    } else {
                        realIndex = 1 + (UnicodeDatabase.decomps[0] << 1) + (data & 2097151);
                        for (var arrfillI = 0; arrfillI < size; arrfillI++) buffer[offset + arrfillI] = UnicodeDatabase.decomps[realIndex + arrfillI];
                    }
                    buffer[offset] = buffer[offset] & 2097151;
                }
                return offset + size;
            } else if (UnicodeDatabase.decomps[realIndex] < cp) {
                left = index + 1;
            } else {
                right = index - 1;
            }
        }
        buffer[offset++] = cp;
        return offset;
    };
    constructor.pairsLength = null;
    constructor.pairs = null;
    constructor.EnsurePairs = function() {
        {
            if (UnicodeDatabase.pairs == null) {
                UnicodeDatabase.pairs = NormalizationData.ComposedPairs;
                UnicodeDatabase.pairsLength = ((UnicodeDatabase.pairs.length / 3)|0);
            }
        }
    };
    constructor.GetComposedPair = function(first, second) {
        if (((first | second) >> 17) != 0) {
            return -1;
        }
        UnicodeDatabase.EnsurePairs();
        var left = 0;
        var right = UnicodeDatabase.pairsLength - 1;
        while (left <= right) {
            var index = (left + right) >> 1;
            var realIndex = index * 3;
            if (UnicodeDatabase.pairs[realIndex] == first) {
                if (UnicodeDatabase.pairs[realIndex + 1] == second) {
                    return UnicodeDatabase.pairs[realIndex + 2];
                } else if (UnicodeDatabase.pairs[realIndex + 1] < second) {
                    left = index + 1;
                } else {
                    right = index - 1;
                }
            } else if (UnicodeDatabase.pairs[realIndex] < first) {
                left = index + 1;
            } else {
                right = index - 1;
            }
        }
        return -1;
    };
})(UnicodeDatabase,UnicodeDatabase.prototype);

var Normalization={};Normalization.NFC=0;Normalization['NFC']=0;Normalization.NFD=1;Normalization['NFD']=1;Normalization.NFKC=2;Normalization['NFKC']=2;Normalization.NFKD=3;Normalization['NFKD']=3;

if(typeof exports!=="undefined")exports['Normalization']=Normalization;
if(typeof window!=="undefined")window['Normalization']=Normalization;

var Normalizer =
function(str, form) {

    if (str == null) {
        throw new Error("stream");
    }
    this.readbuffer = [0];
    this.iterEndIndex = str.length;
    this.lastStableIndex = -1;
    this.iterator = str;
    this.form = form;
    this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
};
(function(constructor,prototype){
    constructor['Normalize'] = constructor.Normalize = function(str, form) {
        if (str == null) {
            throw new Error("str");
        }
        var c = 0;
        if(str.length==1){
          c=str.charCodeAt(0);
          if(c<0xd800 && c>0xdfff && Normalizer.IsStableCodePoint(c)){
            return str;
          }
        }
        var norm = new Normalizer(str, form);
        var builder = [];
        while ((c = norm.ReadChar()) >= 0) {
  if(c<=0xffff){
   builder.push(String.fromCharCode(c))
  } else if (c <= 1114111){
   builder.push(String.fromCharCode(((((c - 65536) >> 10) & 1023) + 55296)))
   builder.push(String.fromCharCode(((c - 65536) & 1023) + 56320))
  }
        }
        return builder.join("");
    };
    constructor['DecompToBufferInternal'] = constructor.DecompToBufferInternal = function(ch, compat, buffer, index) {
        var offset = UnicodeDatabase.GetDecomposition(ch, compat, buffer, index);
        if (buffer[index] != ch) {
            var copy = [];
            for (var arrfillI = 0; arrfillI < offset - index; arrfillI++) copy[arrfillI] = 0;
            for (var arrfillI = 0; arrfillI < copy.length; arrfillI++) copy[arrfillI] = buffer[index + arrfillI];
            offset = index;
            for (var i = 0; i < copy.length; ++i) {
                offset = Normalizer.DecompToBufferInternal(copy[i], compat, buffer, offset);
            }
        }
        return offset;
    };
    prototype['DecompToBuffer'] = prototype.DecompToBuffer = function(ch, compat, buffer, index) {
        if (ch >= 44032 && ch < 44032 + 11172) {
            var valueSIndex = ch - 44032;
            var trail = 4519 + (valueSIndex % 28);
            buffer[index++] = 4352 + ((valueSIndex / 588)|0);
            buffer[index++] = 4449 + (((valueSIndex % 588) / 28)|0);
            if (trail != 4519) {
                buffer[index++] = trail;
            }
            return index;
        } else {
            return Normalizer.DecompToBufferInternal(ch, compat, buffer, index);
        }
    };
   prototype.lastStableIndex = 0;
   prototype.endIndex = 0;
   prototype.iterEndIndex = 0;
   prototype.buffer = null;
   prototype.compatMode = null;
   prototype.form = null;
   prototype.processedIndex = 0;
   prototype.flushIndex = 0;
   prototype.iterator = null;
   prototype.characterListPos = 0;
   prototype.Init = function(str, index, length, form) {
        if (str == null) {
            throw new Error("str");
        }
        if (index < 0) {
            throw new Error("index (" + index + ") is less than " + "0");
        }
        if (index > str.length) {
            throw new Error("index (" + index + ") is more than " + (((str.length)|0) + ""));
        }
        if (length < 0) {
            throw new Error("length (" + ((length|0) + "") + ") is less than " + "0");
        }
        if (length > str.length) {
            throw new Error("length (" + ((length|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (str.length - index < length) {
            throw new Error("str's length minus " + index + " (" + (((str.length - index)|0) + "") + ") is less than " + ((length|0) + ""));
        }
        this.readbuffer = [0];
        this.lastStableIndex = -1;
        this.characterListPos = index;
        this.iterator = str;
        this.iterEndIndex = index + length;
        this.form = form;
        this.compatMode = form == Normalization.NFKC || form == Normalization.NFKD;
        return this;
    };
    constructor['NormalizeAndCheckString'] = constructor.NormalizeAndCheckString = function(charList, start, length, form) {
        var i = start;
        var norm = new Normalizer(charList, form).Init(charList, start, length, form);
        var ch = 0;
        while ((ch = norm.ReadChar()) >= 0) {
            var c = charList.charCodeAt(i);
            if ((c & 64512) == 55296 && i + 1 < charList.length && charList.charCodeAt(i + 1) >= 56320 && charList.charCodeAt(i + 1) <= 57343) {

                c = 65536 + ((c - 55296) << 10) + (charList.charCodeAt(i + 1) - 56320);
                ++i;
            } else if ((c & 63488) == 55296) {

                c = 65533;
            }
            ++i;
            if (c != ch) {
                return false;
            }
        }
        return i == start + length;
    };
    constructor['IsNormalized'] = constructor.IsNormalized = function(str, form) {
        if (str == null) {
            return false;
        }
        var lastNonStable = -1;
        var mask = (form == Normalization.NFC) ? 255 : 127;
        for (var i = 0; i < str.length; ++i) {
            var c = str.charCodeAt(i);
            if ((c & 64512) == 55296 && i + 1 < str.length && str.charCodeAt(i + 1) >= 56320 && str.charCodeAt(i + 1) <= 57343) {

                c = 65536 + ((c - 55296) << 10) + (str.charCodeAt(i + 1) - 56320);
            } else if ((c & 63488) == 55296) {

                return false;
            }
            var isStable = false;
            if ((c & mask) == c && (i + 1 == str.length || (str.charCodeAt(i + 1) & mask) == str.charCodeAt(i + 1))) {

                isStable = true;
            } else {
                isStable = Normalizer.IsStableCodePoint(c, form);
            }
            if (lastNonStable < 0 && !isStable) {

                lastNonStable = i;
            } else if (lastNonStable >= 0 && isStable) {

                if (!Normalizer.NormalizeAndCheckString(str, lastNonStable, i - lastNonStable, form)) {
                    return false;
                }
                lastNonStable = -1;
            }
            if (c >= 65536) {
                ++i;
            }
        }
        if (lastNonStable >= 0) {
            if (!Normalizer.NormalizeAndCheckString(str, lastNonStable, str.length - lastNonStable, form)) {
                return false;
            }
        }
        return true;
    };
    prototype['readbuffer'] = prototype.readbuffer = null;
    prototype['ReadChar'] = prototype.ReadChar = function() {
        var r = this.Read(this.readbuffer, 0, 1);
        return r == 1 ? this.readbuffer[0] : -1;
    };
    prototype['endOfString'] = prototype.endOfString = false;
    prototype['lastChar'] = prototype.lastChar = -1;
    prototype['ungetting'] = prototype.ungetting = false;
    prototype['Unget'] = prototype.Unget = function() {
        this.ungetting = true;
    };
    prototype['GetNextChar'] = prototype.GetNextChar = function() {
        var ch;
        if (this.ungetting) {
            ch = this.lastChar;
            this.ungetting = false;
            return ch;
        } else if (this.characterListPos >= this.iterEndIndex) {
            ch = -1;
        } else {
            ch = this.iterator.charCodeAt(this.characterListPos);
            if ((ch & 64512) == 55296 && this.characterListPos + 1 < this.iterEndIndex && this.iterator.charCodeAt(this.characterListPos + 1) >= 56320 && this.iterator.charCodeAt(this.characterListPos + 1) <= 57343) {

                ch = 65536 + ((ch - 55296) << 10) + (this.iterator.charCodeAt(this.characterListPos + 1) - 56320);
                ++this.characterListPos;
            } else if ((ch & 63488) == 55296) {

                ch = 65533;
            }
            ++this.characterListPos;
        }
        if (ch < 0) {
            this.endOfString = true;
        } else if (ch > 1114111 || ((ch & 2095104) == 55296)) {
            throw new Error("Invalid character: " + ch);
        }
        this.lastChar = ch;
        return ch;
    };

    prototype['Read'] = prototype.Read = function(chars, index, length) {
        if (chars == null) {
            throw new Error("chars");
        }
        if (index < 0) {
            throw new Error("index (" + index + ") is less than " + "0");
        }
        if (index > chars.length) {
            throw new Error("index (" + index + ") is more than " + chars.length);
        }
        if (length < 0) {
            throw new Error("length (" + ((length|0) + "") + ") is less than " + "0");
        }
        if (length > chars.length) {
            throw new Error("length (" + ((length|0) + "") + ") is more than " + chars.length);
        }
        if (chars.length - index < length) {
            throw new Error("chars's length minus " + index + " (" + (((chars.length - index)|0) + "") + ") is less than " + ((length|0) + ""));
        }
        if (length == 0) {
            return 0;
        }
        var total = 0;
        var count = 0;
        if (this.processedIndex == this.flushIndex && this.flushIndex == 0) {
            while (total < length) {
                var c = this.GetNextChar();
                if (c < 0) {
                    return (total == 0) ? -1 : total;
                } else if (UnicodeDatabase.IsStableCodePoint(c, this.form)) {
                    chars[index] = c;
                    ++total;
                    ++index;
                } else {
                    this.Unget();
                    break;
                }
            }
            if (total == length) {
                return total;
            }
        }
        do {

            count = (this.processedIndex - this.flushIndex < length - total ? this.processedIndex - this.flushIndex : length - total);
            if (count < 0) {
                count = 0;
            }
            if (count != 0) {

                for (var arrfillI = 0; arrfillI < count; arrfillI++) chars[index + arrfillI] = this.buffer[this.flushIndex + arrfillI];
            }
            index = index + (count);
            total = total + (count);
            this.flushIndex += count;

            while (total < length) {
                var c = this.GetNextChar();
                if (c < 0) {
                    this.endOfString = true;
                    break;
                }
                if (Normalizer.IsStableCodePoint(c, this.form)) {
                    chars[index++] = c;
                    ++total;
                } else {
                    this.Unget();
                    break;
                }
            }
            if (total < length && this.flushIndex == this.processedIndex) {
                if (this.lastStableIndex > 0) {

                    for (var arrfillI = 0; arrfillI < this.buffer.length - this.lastStableIndex; arrfillI++) this.buffer[arrfillI] = this.buffer[this.lastStableIndex + arrfillI];

                    this.endIndex -= this.lastStableIndex;
                    this.lastStableIndex = 0;
                } else {
                    this.endIndex = 0;
                }
                if (!this.LoadMoreData()) {
                    break;
                }
            }
        } while (total < length);

        count = (0 > (this.processedIndex - this.flushIndex < length - total ? this.processedIndex - this.flushIndex : length - total) ? 0 : (this.processedIndex - this.flushIndex < length - total ? this.processedIndex - this.flushIndex : length - total));
        for (var arrfillI = 0; arrfillI < count; arrfillI++) chars[index + arrfillI] = this.buffer[this.flushIndex + arrfillI];
        index = index + (count);
        total = total + (count);
        this.flushIndex += count;
        return (total == 0) ? -1 : total;
    };
    constructor['IsStableCodePoint'] = constructor.IsStableCodePoint = function(cp, form) {
        return UnicodeDatabase.IsStableCodePoint(cp, form) && cp != 1460 && cp != 1497;
    };
    prototype['LoadMoreData'] = prototype.LoadMoreData = function() {
        var done = false;
        while (!done) {
            if (this.buffer == null) {
                this.buffer = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            }

            while (this.endIndex + 18 <= this.buffer.length) {
                var c = this.GetNextChar();
                if (c < 0) {
                    this.endOfString = true;
                    break;
                }
                this.endIndex = this.DecompToBuffer(c, this.compatMode, this.buffer, this.endIndex);
            }

            if (!this.endOfString) {
                var haveNewStable = false;

                for (var i = this.endIndex - 1; i > this.lastStableIndex; --i) {

                    if (Normalizer.IsStableCodePoint(this.buffer[i], this.form)) {
                        this.lastStableIndex = i;
                        haveNewStable = true;
                        break;
                    }
                }
                if (!haveNewStable || this.lastStableIndex <= 0) {

                    var newBuffer = [];
                    for (var arrfillI = 0; arrfillI < (this.buffer.length + 4) * 2; arrfillI++) newBuffer[arrfillI] = 0;
                    for (var arrfillI = 0; arrfillI < this.buffer.length; arrfillI++) newBuffer[arrfillI] = this.buffer[arrfillI];
                    this.buffer = newBuffer;
                    continue;
                }
            } else {

                this.lastStableIndex = this.endIndex;
            }
            done = true;
        }

        if (this.endIndex == 0) {
            return false;
        }
        this.flushIndex = 0;

        this.ReorderBuffer(this.buffer, 0, this.lastStableIndex);
        if (this.form == Normalization.NFC || this.form == Normalization.NFKC) {

            this.processedIndex = this.ComposeBuffer(this.buffer, this.lastStableIndex);
        } else {
            this.processedIndex = this.lastStableIndex;
        }
        return true;
    };
    prototype['ReorderBuffer'] = prototype.ReorderBuffer = function(buffer, index, length) {
        var i;
        if (length < 2) {
            return;
        }
        var changed;
        do {
            changed = false;

            var lead = UnicodeDatabase.GetCombiningClass(buffer[index]);
            var trail;
            for (i = 1; i < length; ++i) {
                var offset = index + i;
                trail = UnicodeDatabase.GetCombiningClass(buffer[offset]);
                if (trail != 0 && lead > trail) {
                    var c = buffer[offset - 1];
                    buffer[offset - 1] = buffer[offset];
                    buffer[offset] = c;

                    changed = true;
                } else {

                    lead = trail;
                }
            }
        } while (changed);
    };
    prototype['ComposeBuffer'] = prototype.ComposeBuffer = function(array, length) {
        if (length < 2) {
            return length;
        }
        var starterPos = 0;
        var retval = length;
        var starter = array[0];
        var last = UnicodeDatabase.GetCombiningClass(starter);
        if (last != 0) {
            last = 256;
        }
        var compPos = 0;
        var endPos = 0 + length;
        var composed = false;
        for (var decompPos = compPos; decompPos < endPos; ++decompPos) {
            var ch = array[decompPos];
            var valuecc = UnicodeDatabase.GetCombiningClass(ch);
            if (decompPos > compPos) {
                var lead = starter - 4352;
                if (0 <= lead && lead < 19) {

                    var vowel = ch - 4449;
                    if (0 <= vowel && vowel < 21 && (last < valuecc || last == 0)) {
                        starter = 44032 + (((lead * 21) + vowel) * 28);
                        array[starterPos] = starter;
                        array[decompPos] = 1114112;
                        composed = true;
                        --retval;
                        continue;
                    }
                }
                var syllable = starter - 44032;
                if (0 <= syllable && syllable < 11172 && (syllable % 28) == 0) {

                    var trail = ch - 4519;
                    if (0 < trail && trail < 28 && (last < valuecc || last == 0)) {
                        starter = starter + (trail);
                        array[starterPos] = starter;
                        array[decompPos] = 1114112;
                        composed = true;
                        --retval;
                        continue;
                    }
                }
            }
            var composite = UnicodeDatabase.GetComposedPair(starter, ch);
            var diffClass = last < valuecc;
            if (composite >= 0 && (diffClass || last == 0)) {
                array[starterPos] = composite;
                starter = composite;
                array[decompPos] = 1114112;
                composed = true;
                --retval;
                continue;
            }
            if (valuecc == 0) {
                starterPos = decompPos;
                starter = ch;
            }
            last = valuecc;
        }
        if (composed) {
            var j = compPos;
            for (var i = compPos; i < endPos; ++i) {
                if (array[i] != 1114112) {
                    array[j++] = array[i];
                }
            }
        }
        return retval;
    };
})(Normalizer,Normalizer.prototype);

if(typeof exports!=="undefined")exports['Normalizer']=Normalizer;
if(typeof window!=="undefined")window['Normalizer']=Normalizer;

var DomainUtility = function() {

};
(function(constructor,prototype){
    constructor.CodePointAt = function(str, index, endIndex) {
        if (str == null) {
            throw new Error("str");
        }
        if (index >= endIndex) {
            return -1;
        }
        if (index < 0) {
            return -1;
        }
        var c = str.charCodeAt(index);
        if ((c & 64512) == 55296 && index + 1 < endIndex && str.charCodeAt(index + 1) >= 56320 && str.charCodeAt(index + 1) <= 57343) {

            c = 65536 + ((c - 55296) << 10) + (str.charCodeAt(index + 1) - 56320);
            ++index;
        } else if ((c & 63488) == 55296) {

            return 65533;
        }
        return c;
    };

    constructor['PunycodeLength'] = function(str, index, endIndex) {
        if (str == null) {
            throw new Error("str");
        }
        if (index < 0) {
            throw new Error("index (" + ((index|0) + "") + ") is less than " + "0");
        }
        if (index > str.length) {
            throw new Error("index (" + ((index|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (endIndex < 0) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is less than " + "0");
        }
        if (endIndex > str.length) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (endIndex < index) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is less than " + ((index|0) + ""));
        }
        var n = 128;
        var delta = 0;
        var bias = 72;
        var h = 0;
        var tmpIndex;
        var firstIndex = -1;
        var codePointLength = 0;
        var basicsBeforeFirstNonbasic = 0;
        var allBasics = true;
        tmpIndex = index;
        while (tmpIndex < endIndex) {
            if (str.charCodeAt(tmpIndex) >= 128) {
                allBasics = false;
                break;
            }
            ++tmpIndex;
        }
        if (allBasics) {
            return endIndex - index;
        }
        var outputLength = 4;
        tmpIndex = index;
        while (tmpIndex < endIndex) {
            var c = DomainUtility.CodePointAt(str, tmpIndex, endIndex);
            ++codePointLength;
            if (c < 128) {

                ++outputLength;
                ++h;
            } else if (firstIndex < 0) {
                firstIndex = tmpIndex;
            }

            tmpIndex = tmpIndex + ((c >= 65536) ? 2 : 1);
        }
        if (h != 0) {
            ++outputLength;
        }
        var b = h;
        if (firstIndex >= 0) {
            basicsBeforeFirstNonbasic = firstIndex - index;
        } else {

            return endIndex - index;
        }
        while (h < codePointLength) {
            var min = 1114112;
            tmpIndex = firstIndex;
            while (tmpIndex < endIndex) {
                var c = DomainUtility.CodePointAt(str, tmpIndex, endIndex);
                tmpIndex = tmpIndex + ((c >= 65536) ? 2 : 1);
                if (c >= n && c < min) {
                    min = c;
                }
            }
            var d = min - n;
            if (d > ((2147483647 / (h + 1))|0)) {
                return -1;
            }
            d *= h + 1;
            n = min;
            if (d > 2147483647 - delta) {
                return -1;
            }
            delta = delta + (d);
            tmpIndex = firstIndex;
            if (basicsBeforeFirstNonbasic > 2147483647 - delta) {
                return -1;
            }
            delta = delta + (basicsBeforeFirstNonbasic);
            while (tmpIndex < endIndex) {
                var c = DomainUtility.CodePointAt(str, tmpIndex, endIndex);
                tmpIndex = tmpIndex + ((c >= 65536) ? 2 : 1);
                if (c < n) {
                    if (delta == 2147483647) {
                        return -1;
                    }
                    ++delta;
                } else if (c == n) {
                    var q = delta;
                    var k = 36;
                    while (true) {
                        var t;
                        if (k <= bias) {
                            t = 1;
                        } else if (k >= bias + 26) {
                            t = 26;
                        } else {
                            t = k - bias;
                        }
                        if (q < t) {
                            break;
                        }
                        ++outputLength;
                        q -= t;
                        q = ((q / 36)|0) - t;
                        k = k + (36);
                    }
                    ++outputLength;
                    delta = (h == b) ? ((delta / 700)|0) : delta >> 1;
                    delta = delta + ((delta / (h + 1))|0);
                    k = 0;
                    while (delta > 455) {
                        delta = ((delta / 35)|0);
                        k = k + (36);
                    }
                    bias = k + (((36 * delta) / (delta + 38))|0);
                    delta = 0;
                    ++h;
                }
            }
            ++n;
            ++delta;
        }
        return outputLength;
    };
    constructor.valueDigitValues = [-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1];

    constructor['PunycodeDecode'] = function(str, index, endIndex) {
        if (str == null) {
            throw new Error("str");
        }
        if (index < 0) {
            throw new Error("index (" + ((index|0) + "") + ") is less than " + "0");
        }
        if (index > str.length) {
            throw new Error("index (" + ((index|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (endIndex < 0) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is less than " + "0");
        }
        if (endIndex > str.length) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (endIndex < index) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is less than " + ((index|0) + ""));
        }
        if (index == endIndex) {
            return "";
        }
        var lastHyphen = endIndex;
        while (lastHyphen >= index) {
            if (str.charCodeAt(lastHyphen) == 0x2d) {
                break;
            }
            --lastHyphen;
        }
        var i = 0;
        if (lastHyphen >= index) {
            for (i = index; i < lastHyphen; ++i) {
                if (str.charCodeAt(i) >= 128) {
                    return null;
                }
            }
        }

        var builder = [];
        for (var k = index; k < endIndex; ++k) {
            var c = str.charCodeAt(i);
            if (c >= 65 && c <= 90) {
                c = c + (32);
            }
            builder.push(String.fromCharCode(c));
        }
        if (lastHyphen >= index) {
            index = lastHyphen + 1;
        }
        i = 0;
        var n = 128;
        var bias = 72;
        var stringLength = builder.length;
        var chararr = [0, 0];
        while (index < endIndex) {
            var old = index;
            var w = 1;
            var k = 36;
            while (true) {
                if (index >= endIndex) {
                    return null;
                }
                k = k + (36);
                var c = str.charCodeAt(index);
                if (c >= 128) {
                    return null;
                }
                var digit = DomainUtility.valueDigitValues[(c|0)];
                if (digit < 0) {
                    return null;
                }
                if (digit > ((2147483647 / w)|0)) {
                    return null;
                }
                var temp = digit * w;
                if (i > 2147483647 - temp) {
                    return null;
                }
                i -= temp;
                var t = k - bias;
                if (k <= bias) {
                    t = 1;
                } else if (k >= bias + 26) {
                    t = 26;
                }
                if (digit < t) {
                    break;
                }
                temp = 36 - t;
                if (w > ((2147483647 / temp)|0)) {
                    return null;
                }
                w *= temp;
            }
            var futureLength = stringLength + 1;
            var delta = (old == 0) ? (((old - i) / 700)|0) : (old - i) >> 1;
            delta = delta + ((delta / futureLength)|0);
            k = 0;
            while (delta > 455) {
                delta = ((delta / 35)|0);
                k = k + (36);
            }
            bias = k + (((36 * delta) / (delta + 38))|0);
            var idiv = ((i / futureLength)|0);
            if (n > 2147483647 - idiv) {
                return null;
            }
            n = n + (idiv);
            i %= futureLength;
            if (n <= 65535) {
                chararr[0] = String.fromCharCode(n);
                builder.splice(i,0,chararr[0])
            } else if (n <= 1114111) {
                chararr[0] = String.fromCharCode((((n - 65536) >> 10) & 1023) + 55296);
                chararr[1] = String.fromCharCode(((n - 65536) & 1023) + 56320);
                builder.splice(i,0,chararr[0],chararr[1])
            } else {
                return null;
            }
            ++futureLength;
            ++i;
        }
        return builder.join("");
    };
    constructor.valuePunycodeAlphabet = "abcdefghijklmnopqrstuvwxyz0123456789";
    constructor['PunycodeEncode'] = function(str) {
        return DomainUtility.PunycodeEncodePortion(str, 0, str.length);
    };
    constructor['PunycodeEncodePortion'] = function(str, index, endIndex) {
        if (str == null) {
            throw new Error("str");
        }
        if (index < 0) {
            throw new Error("index (" + ((index|0) + "") + ") is less than " + "0");
        }
        if (index > str.length) {
            throw new Error("index (" + ((index|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (endIndex < 0) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is less than " + "0");
        }
        if (endIndex > str.length) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is more than " + (((str.length)|0) + ""));
        }
        if (endIndex < index) {
            throw new Error("endIndex (" + ((endIndex|0) + "") + ") is less than " + ((index|0) + ""));
        }
        var n = 128;
        var delta = 0;
        var bias = 72;
        var h = 0;
        var tmpIndex;
        var firstIndex = -1;
        var codePointLength = 0;
        var basicsBeforeFirstNonbasic = 0;
        var allBasics = true;
        tmpIndex = index;
        while (tmpIndex < endIndex) {
            if (str.charCodeAt(tmpIndex) >= 128) {
                allBasics = false;
                break;
            } else if (str.charCodeAt(tmpIndex) >= 65 && str.charCodeAt(tmpIndex) <= 90) {

                allBasics = false;
                break;
            }
            ++tmpIndex;
        }
        if (allBasics) {
            return str.substring(index, (index) + (endIndex - index));
        }
        var builder = [];
        builder.push("xn--");
        tmpIndex = index;
        while (tmpIndex < endIndex) {
            var c = Idna.CodePointAt(str, tmpIndex);
            ++codePointLength;
            if (c >= 65 && c <= 90) {

                builder.push(String.fromCharCode(c + 32));
                ++h;
            } else if (c < 128) {

                builder.push(String.fromCharCode(c));
                ++h;
            } else if (firstIndex < 0) {
                firstIndex = tmpIndex;
            }
            if (c >= 65536) {
                ++tmpIndex;
            }
            ++tmpIndex;
        }
        var b = h;
        if (firstIndex >= 0) {
            basicsBeforeFirstNonbasic = firstIndex - index;
        } else {

            return builder.toString();
        }
        if (h != 0) {
            builder.push("-");
        }
        while (h < codePointLength) {
            var min = 1114112;
            tmpIndex = firstIndex;
            while (tmpIndex < endIndex) {
                var c = Idna.CodePointAt(str, tmpIndex);
                if (c >= n && c < min) {
                    min = c;
                }
                if (c >= 65536) {
                    ++tmpIndex;
                }
                ++tmpIndex;
            }
            var d = min - n;
            if (d > ((2147483647 / (h + 1))|0)) {
                return null;
            }
            d *= h + 1;
            n = min;
            if (d > 2147483647 - delta) {
                return null;
            }
            delta = delta + (d);
            tmpIndex = firstIndex;
            if (basicsBeforeFirstNonbasic > 2147483647 - delta) {
                return null;
            }
            delta = delta + (basicsBeforeFirstNonbasic);
            while (tmpIndex < endIndex) {
                var c = Idna.CodePointAt(str, tmpIndex);
                if (c >= 65536) {
                    ++tmpIndex;
                }
                ++tmpIndex;
                if (c < n) {
                    if (delta == 2147483647) {
                        return null;
                    }
                    ++delta;
                } else if (c == n) {
                    var q = delta;
                    var k = 36;
                    while (true) {
                        var t;
                        if (k <= bias) {
                            t = 1;
                        } else if (k >= bias + 26) {
                            t = 26;
                        } else {
                            t = k - bias;
                        }
                        if (q < t) {
                            break;
                        }
                        var digit = t + ((q - t) % (36 - t));
                        builder.push(DomainUtility.valuePunycodeAlphabet.charAt(digit));
                        q -= t;
                        q = ((q / 36)|0) - t;
                        k = k + (36);
                    }
                    builder.push(DomainUtility.valuePunycodeAlphabet.charAt(q));
                    delta = (h == b) ? ((delta / 700)|0) : delta >> 1;
                    delta = delta + ((delta / (h + 1))|0);
                    k = 0;
                    while (delta > 455) {
                        delta = ((delta / 35)|0);
                        k = k + (36);
                    }
                    bias = k + (((36 * delta) / (delta + 38))|0);
                    delta = 0;
                    ++h;
                }
            }
            ++n;
            ++delta;
        }
        return builder.join("");
    };
})(DomainUtility,DomainUtility.prototype);

if(typeof exports!=="undefined")exports['DomainUtility']=DomainUtility;
if(typeof window!=="undefined")window['DomainUtility']=DomainUtility;

var Idna = function(){};
(function(constructor,prototype){
    constructor.Unassigned = 0;
    constructor.Disallowed = 2;
    constructor.ContextJ = 3;
    constructor.ContextO = 4;
    constructor.BidiClassL = 0;
    constructor.BidiClassR = 1;
    constructor['BidiClassAL'] = constructor.BidiClassAL = 2;
    constructor['BidiClassEN'] = constructor.BidiClassEN = 3;
    constructor['BidiClassES'] = constructor.BidiClassES = 4;
    constructor['BidiClassET'] = constructor.BidiClassET = 5;
    constructor['BidiClassAN'] = constructor.BidiClassAN = 6;
    constructor['BidiClassCS'] = constructor.BidiClassCS = 7;
    constructor['BidiClassNSM'] = constructor.BidiClassNSM = 8;
    constructor['BidiClassBN'] = constructor.BidiClassBN = 9;
    constructor['BidiClassON'] = constructor.BidiClassON = 10;
    constructor['bidiClasses'] = constructor.bidiClasses = null;
    constructor.joiningTypes = null;
    constructor.scripts = null;
    
    constructor.CodePointBefore = function(str, index) {
        if (str == null) {
            throw new Error("str");
        }
        if (index <= 0) {
            return -1;
        }
        if (index > str.length) {
            return -1;
        }
        var c = str.charCodeAt(index - 1);
        if ((c & 64512) == 56320 && index - 2 >= 0 && str.charCodeAt(index - 2) >= 55296 && str.charCodeAt(index - 2) <= 56319) {

            return 65536 + ((str.charCodeAt(index - 2) - 55296) << 10) + (c - 56320);
        } else if ((c & 63488) == 55296) {

            return 65533;
        } else {
            return c;
        }
    };
    constructor.CodePointAt = function(str, index) {
        if (str == null) {
            throw new Error("str");
        }
        if (index >= str.length) {
            return -1;
        }
        if (index < 0) {
            return -1;
        }
        var c = str.charCodeAt(index);
        if ((c & 64512) == 55296 && index + 1 < str.length && str.charCodeAt(index + 1) >= 56320 && str.charCodeAt(index + 1) <= 57343) {

            c = 65536 + ((c - 55296) << 10) + (str.charCodeAt(index + 1) - 56320);
        } else if ((c & 63488) == 55296) {
            return 65533;
        }
        return c;
    };
    constructor.GetBidiClass = function(ch) {
        var table = null;
        {
            if (Idna.bidiClasses == null) {
                Idna.bidiClasses = ByteData.Decompress(IdnaData.BidiClasses);
            }
            table = Idna.bidiClasses;
        }
        return table.GetByte(ch);
    };
    constructor.GetJoiningType = function(ch) {
        var table = null;
        {
            if (Idna.joiningTypes == null) {
                Idna.joiningTypes = ByteData.Decompress(IdnaData.JoiningTypes);
            }
            table = Idna.joiningTypes;
        }
        return table.GetByte(ch);
    };
    constructor['GetScript'] = constructor.GetScript = function(ch) {
        var table = null;
        {
            if (Idna.scripts == null) {
                Idna.scripts = ByteData.Decompress(IdnaData.IdnaRelevantScripts);
            }
            table = Idna.scripts;
        }
        return table.GetByte(ch);
    };
    constructor['JoiningTypeTransparent'] = constructor.JoiningTypeTransparent = function(ch) {
        return Idna.GetJoiningType(ch) == 1;
    };
    constructor['JoiningTypeLeftOrDual'] = constructor.JoiningTypeLeftOrDual = function(ch) {
        var jtype = Idna.GetJoiningType(ch);
        return jtype == 3 || jtype == 4;
    };
    constructor['JoiningTypeRightOrDual'] = constructor.JoiningTypeRightOrDual = function(ch) {
        var jtype = Idna.GetJoiningType(ch);
        return jtype == 2 || jtype == 4;
    };
    constructor['IsGreek'] = constructor.IsGreek = function(ch) {
        return Idna.GetScript(ch) == 1;
    };
    constructor['IsHebrew'] = constructor.IsHebrew = function(ch) {
        return Idna.GetScript(ch) == 2;
    };
    constructor['IsKanaOrHan'] = constructor.IsKanaOrHan = function(ch) {
        return Idna.GetScript(ch) == 3;
    };
    constructor['IsValidConjunct'] = constructor.IsValidConjunct = function(str, index) {

        var found = false;
        var oldIndex = index;
        while (index > 0) {
            var ch = Idna.CodePointBefore(str, index);
            index -= (ch >= 65536) ? 2 : 1;
            if (Idna.JoiningTypeLeftOrDual(ch)) {
                found = true;
            } else if (!Idna.JoiningTypeTransparent(ch)) {
                return false;
            }
        }
        if (!found) {
            return false;
        }

        index = oldIndex + 1;
        while (index < str.length) {
            var ch = Idna.CodePointAt(str, index);
            index = index + ((ch >= 65536) ? 2 : 1);
            if (Idna.JoiningTypeRightOrDual(ch)) {
                return true;
            } else if (!Idna.JoiningTypeTransparent(ch)) {
                return false;
            }
        }
        return false;
    };
    constructor['HasRtlCharacters'] = constructor.HasRtlCharacters = function(str) {
        for (var i = 0; i < str.length; ++i) {
            if (str.charCodeAt(i) >= 128) {
                var c = Idna.CodePointAt(str, i);
                if (c >= 65536) {
                    ++i;
                }
                var bidiClass = Idna.GetBidiClass(c);
                if (bidiClass == Idna.BidiClassAL || bidiClass == Idna.BidiClassAN || bidiClass == Idna.BidiClassR) {
                    return true;
                }
            }
        }
        return false;
    };

    constructor['EncodeDomainName'] = constructor.EncodeDomainName = function(value) {
        if (value == null) {
            throw new Error("value");
        }
        if (value.length == 0) {
            return "";
        }
        var builder = [];
        var retval = null;
        var lastIndex = 0;
        for (var i = 0; i < value.length; ++i) {
            var c = value.charCodeAt(i);
            if (c == 0x2e) {
                if (i != lastIndex) {
                    retval = DomainUtility.PunycodeEncodePortion(value, lastIndex, i);
                    if (retval == null) {

                        builder.append(value.substring(lastIndex, (lastIndex) + ((i + 1) - lastIndex)));
                    } else {
                        builder.append(retval);
                        builder.append(".");
                    }
                }
                lastIndex = i + 1;
            }
        }
        retval = DomainUtility.PunycodeEncodePortion(value, lastIndex, value.length);
        if (retval == null) {
            builder.append(value.substring(lastIndex, (lastIndex) + (value.length - lastIndex)));
        } else {
            builder.append(retval);
        }
        return builder.join("")
    };
    constructor['IsValidDomainName'] = constructor.IsValidDomainName = function(str, lookupRules) {
        if ((str) == null || (str).length == 0) {
            return false;
        }
        var bidiRule = Idna.HasRtlCharacters(str);
        var lastIndex = 0;
        for (var i = 0; i < str.length; ++i) {
            var c = str.charCodeAt(i);
            if (c == 0x2e) {
                if (i == lastIndex) {

                    return false;
                }
                if (!Idna.IsValidLabel(str.substring(lastIndex, (lastIndex) + (i - lastIndex)), lookupRules, bidiRule)) {
                    return false;
                }
                lastIndex = i + 1;
            }
        }
        if (str.length == lastIndex) {
            return false;
        }
        return Idna.IsValidLabel(str.substring(lastIndex, (lastIndex) + (str.length - lastIndex)), lookupRules, bidiRule);
    };
    constructor['ToLowerCaseAscii'] = constructor.ToLowerCaseAscii = function(str) {
        if (str == null) {
            return null;
        }
        var len = str.length;
        var c = 0;
        var hasUpperCase = false;
        for (var i = 0; i < len; ++i) {
            c = str.charCodeAt(i);
            if (c >= 0x41 && c <= 0x5a) {
                hasUpperCase = true;
                break;
            }
        }
        if (!hasUpperCase) {
            return str;
        }
        var builder = [];
        for (var i = 0; i < len; ++i) {
            c = str.charCodeAt(i);
            if (c >= 0x41 && c <= 0x5a) {
                builder.push(String.fromCharCode(c + 32));
            } else {
                builder.push(String.fromCharCode(c));
            }
        }
        return builder.join("");
    };
    constructor['IsValidLabel'] = constructor.IsValidLabel = function(str, lookupRules, bidiRule) {
        if ((str) == null || (str).length == 0) {
            return false;
        }
        var maybeALabel = false;
        if (str.length >= 4 && (str.charAt(0) == 'x' || str.charAt(0) == 'X') && 
        (str.charAt(0) == 'n' || str.charAt(0) == 'N') && str.charCodeAt(2) == 0x2d && str.charCodeAt(3) == 0x2d) {
            maybeALabel = true;
        }
        for (var i = 0; i < str.length; ++i) {
        // check for alphanumeric or hyphen
            if ((str.charCodeAt(i) >= 0x61 && str.charCodeAt(i) <= 0x7a) || 
                  (str.charCodeAt(i) >= 0x41 && str.charCodeAt(i) <= 0x5a) || 
                  (str.charCodeAt(i) >= 0x30 && str.charCodeAt(i) <= 0x39) || str.charCodeAt(i) == 0x2d) {

                continue;
            } else if (str.charCodeAt(i) >= 128) {

                continue;
            } else {
                return false;
            }
        }
        if (maybeALabel) {
            str = Idna.ToLowerCaseAscii(str);
            var ustr = DomainUtility.PunycodeDecode(str, 4, str.length);
            if (ustr == null) {

                return false;
            }
            if (!Idna.IsValidULabel(ustr, lookupRules, bidiRule)) {
                return false;
            }
            var astr = DomainUtility.PunycodeEncodePortion(ustr, 0, ustr.length);
            if (astr == null) {
                return false;
            }

            return astr.equals(str);
        } else {
            return Idna.IsValidULabel(str, lookupRules, bidiRule);
        }
    };
    constructor['IsValidULabel'] = constructor.IsValidULabel = function(str, lookupRules, bidiRule) {
        if ((str) == null || (str).length == 0) {
            return false;
        }
        if (str.length > 63 && !lookupRules) {

            return false;
        }
        if (!Normalizer.IsNormalized(str, Normalization.NFC)) {
            return false;
        }
        // check "--" at second and third position
        if (str.length >= 4 && str.charCodeAt(2) == 0x2d && str.charCodeAt(3) == 0x2d) {
            return false;
        }
        if (!lookupRules) {
            // check "-" at start and end
            if (str.charCodeAt(0) == 0x2d || str.charCodeAt(str.length - 1) == 0x2d) {
                return false;
            }
        }
        var ch;
        var first = true;
        var haveContextual = false;
        var rtl = false;
        var bidiClass;
        for (var i = 0; i < str.length; ++i) {
            ch = Idna.CodePointAt(str, i);
            if (ch >= 65536) {
                ++i;
            }
            var category = UnicodeDatabase.GetIdnaCategory(ch);
            if (category == Idna.Disallowed || category == Idna.Unassigned) {
                return false;
            }
            if (first) {
                if (UnicodeDatabase.IsCombiningMark(ch)) {
                    return false;
                }
                if (bidiRule) {
                    bidiClass = Idna.GetBidiClass(ch);
                    if (bidiClass == Idna.BidiClassR || bidiClass == Idna.BidiClassAL) {
                        rtl = true;
                    } else if (bidiClass != Idna.BidiClassL) {

                        return false;
                    }
                }
            }
            if (category == Idna.ContextO || category == Idna.ContextJ) {
                haveContextual = true;
            }
            first = false;
        }
        if (haveContextual) {
            var regArabDigits = false;
            var extArabDigits = false;
            var haveKatakanaMiddleDot = false;
            var haveKanaOrHan = false;
            var lastChar = 0;
            for (var i = 0; i < str.length; ++i) {
                var thisChar = Idna.CodePointAt(str, i);
                if (thisChar >= 1632 && thisChar <= 1641) {

                    if (extArabDigits) {
                        return false;
                    }
                    regArabDigits = true;
                } else if (thisChar >= 1776 && thisChar <= 1785) {

                    if (regArabDigits) {
                        return false;
                    }
                    extArabDigits = true;
                } else if (thisChar == 183) {

                    if (!(i - 1 >= 0 && i + 1 < str.length && lastChar == 108 && str.charCodeAt(i + 1) == 108)) {

                        return false;
                    }
                } else if (thisChar == 8205) {

                    if (UnicodeDatabase.GetCombiningClass(lastChar) != 9) {
                        return false;
                    }
                } else if (thisChar == 8204) {

                    if (UnicodeDatabase.GetCombiningClass(lastChar) != 9 && !Idna.IsValidConjunct(str, i)) {
                        return false;
                    }
                } else if (thisChar == 885) {

                    if (i + 1 >= str.length || !Idna.IsGreek(Idna.CodePointAt(str, i + 1))) {
                        return false;
                    }
                } else if (thisChar == 1523 || thisChar == 1524) {

                    if (i <= 0 || !Idna.IsHebrew(lastChar)) {
                        return false;
                    }
                } else if (thisChar == 12539) {
                    haveKatakanaMiddleDot = true;
                } else {
                    var category = UnicodeDatabase.GetIdnaCategory(thisChar);
                    if (category == Idna.ContextJ || category == Idna.ContextO) {

                        return false;
                    }
                }
                if (!haveKanaOrHan && Idna.IsKanaOrHan(thisChar)) {
                    haveKanaOrHan = true;
                }
                if (thisChar >= 65536) {
                    ++i;
                }
                lastChar = thisChar;
            }
            if (haveKatakanaMiddleDot && !haveKanaOrHan) {

                return false;
            }
        }

        if (bidiRule) {
            var found = false;
            for (var i = str.length; i > 0; --i) {
                var c = Idna.CodePointBefore(str, i);
                if (c >= 65536) {
                    --i;
                }
                bidiClass = Idna.GetBidiClass(c);
                if (rtl && (bidiClass == Idna.BidiClassR || bidiClass == Idna.BidiClassAL || bidiClass == Idna.BidiClassAN)) {
                    found = true;
                    break;
                }
                if (!rtl && (bidiClass == Idna.BidiClassL)) {
                    found = true;
                    break;
                }
                if (bidiClass == Idna.BidiClassEN) {
                    found = true;
                    break;
                } else if (bidiClass != Idna.BidiClassNSM) {
                    return false;
                }
            }
            if (!found) {
                return false;
            }
            var haveEN = false;
            var haveAN = false;
            for (var i = 0; i < str.length; ++i) {
                var c = Idna.CodePointAt(str, i);
                if (c >= 65536) {
                    ++i;
                }
                bidiClass = Idna.GetBidiClass(c);
                if (rtl && (bidiClass == Idna.BidiClassR || bidiClass == Idna.BidiClassAL || bidiClass == Idna.BidiClassAN)) {
                    if (bidiClass == Idna.BidiClassAN) {
                        if (haveEN) {
                            return false;
                        }
                        haveAN = true;
                    }
                    continue;
                }
                if (!rtl && (bidiClass == Idna.BidiClassL)) {
                    continue;
                }
                if (bidiClass == Idna.BidiClassEN) {
                    if (rtl) {
                        if (haveAN) {
                            return false;
                        }
                        haveEN = false;
                    }
                    continue;
                } else if (bidiClass == Idna.BidiClassES || bidiClass == Idna.BidiClassCS || bidiClass == Idna.BidiClassET || bidiClass == Idna.BidiClassON || bidiClass == Idna.BidiClassBN || bidiClass == Idna.BidiClassNSM) {
                    continue;
                } else {
                    return false;
                }
            }
        }
        var aceLength = DomainUtility.PunycodeLength(str, 0, str.length);
        if (aceLength < 0) {
            return false;
        }

        if (!lookupRules) {

            if (aceLength > 63) {
                return false;
            }
        }
        return true;
    };
})(Idna,Idna.prototype);

if(typeof exports!=="undefined")exports['Idna']=Idna;
if(typeof window!=="undefined")window['Idna']=Idna;
