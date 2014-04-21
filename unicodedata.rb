# Written in 2014 by Peter O.
# Any copyright is dedicated to the Public Domain.
# http://creativecommons.org/publicdomain/zero/1.0/
# 
# If you like this, you should donate to Peter O.
# at: http://upokecenter.com/d/

Dir.chdir(File.dirname(__FILE__))

require 'net/http'

module Enumerable
 def transform
   ret=[]
   self.each{|item| ret.push(yield item) }
   return ret
 end
end

def downloadIfNeeded(localFile,remoteUrl)
  if !FileTest.exist?(localFile)
     uri=URI.parse(remoteUrl)
     http=Net::HTTP.new(uri.host, uri.port)
     http.open_timeout=10
     http.read_timeout=15
     response=nil
     http.start {|http| response=http.request_get(uri.request_uri) }
     File.open(localFile,"wb"){|f| f.write(response.body) }
  end
end

def getCompEx(file)
  ret=[]
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^([A-Fa-f0-9]+)\.\.([A-Fa-f0-9]+)\s*;\s*Full_Composition_Exclusion\b/]
        a=$1.to_i(16)
        b=$2.to_i(16)
        for i in a..b
         ret.push(i)
        end
      elsif ln[/^([A-Fa-f0-9]+)\s*;\s*Full_Composition_Exclusion\b/]
        a=$1.to_i(16)
        ret.push(a)
      end
    end
  }
  return ret
end
def getUnicodeData(file)
  ccc=[]
  decompType=[]
  decompMapping={}
  File.open(file,"rb"){|f|
   while !f.eof?
    ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
    ln=ln.split(";")
    # Get the canonical combining class
    lnccc=ln[3].to_i(10)
    type=""
    mapping=nil
    # Get the decomposition mapping and type
    if ln[5][/^(<[^>]+>\s*)?(.+)/]
      type=$1 || ""
      mapping=$2
      mapping=mapping.split(/\s+/)
      for i in 0...mapping.length
        mapping[i]=mapping[i].gsub(/^\s+|\s+$/,"").to_i(16)
      end
    end
    first=ln[0].hex;
    last=first
    if ln[1].include?("First>")
      ln2=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"").split()
      last=ln2[0].hex
    end
    for i in first..last
      ccc[i]=lnccc
      decompType[i]=type
      decompMapping[i]=mapping if mapping
    end
   end
  }
  return [ccc,decompType,decompMapping]
end

def generateComposites(dtypes,dmappings,compex)
  ret={}
  for i in 0...0x110000
    decomp=dmappings[i]
    dtype=dtypes[i]
    iscompat=(dtype && dtype.length>0)
    if decomp && decomp.length==2 && !iscompat &&
        !compex.include?(i)
        ret[decomp[0]*0x110000+decomp[1]]=i
    end
  end
  return ret
end

module UnicodeDatabase
 def self.getDecomp(ch,compat)
   dm=$DecompMappings[ch]
   return [ch] if !dm || dm.length==0
   if !compat
     dt=$DecompTypes[ch]
     # Return this character, not the mapping,
     # if the mapping is a compatibility mapping
     return [ch] if dt && dt.length!=0
   end
   return dm
 end
 def self.getCombiningClass(ch)
   return $CombiningClasses[ch]||0
 end
 def self.getComposedPair(ch1,ch2)
   ret=$ComposedPairs[ch1*0x110000+ch2]
   return ret ? ret : -1
 end
end

#
#   LZ4 compressor
#

module LZ4
def self.findLongestMatch(s, sOffset, sLength, subOffset, subLength)
  subLength=[subLength,s.length-subOffset].min
  sLength=[sLength,s.length-sOffset].min
  if subLength<4 || sLength<4
      return [0, 0]
  end
  window=s[sOffset,sLength]
  length=0
  lastIndex=nil
  for i in 4...[sLength,subLength].min
    word=s[subOffset,i]
    ri=window.rindex(word)
    if !ri
      return (lastIndex) ? [lastIndex+sOffset,length] : [0,0]
    end
    lastIndex=ri
    length=i
  end
  return (lastIndex) ? [lastIndex+sOffset,length] : [0,0]
end  
def self.compress(x)
    x=x.pack("C*") if x.is_a?(Array)
    offset=0
    ret=""
    lastLiteral=[]
    effectiveLength=x.length-8
    while offset<effectiveLength
      start=[offset-8192,0].max
      token=findLongestMatch(x,start,offset-start,offset,effectiveLength)
      if token[1]==0
        lastLiteral.push(x[offset,1])
        offset+=1
      elsif token[1]<4
        lastLiteral.push(x[start+token[0],token[1]])
        offset+=token[1]
      else
        lastLiteral=lastLiteral.join("")
        tokenByte=(lastLiteral.length>=15) ? 0xF0 : (lastLiteral.length<<4)
        tokenByte|=(token[1]>=19) ? 0x0F : (token[1]-4)
        literalPack=[tokenByte]
        litLength=lastLiteral.length
        oldLitLength=lastLiteral.length
        while litLength>=15
          literalPack.push((litLength-15 >= 255) ? 255 : litLength-15)
          break if litLength-15<255
          litLength-=255
        end
        literalPack=literalPack.pack("C*")
      #  puts "literal "+lastLiteral.unpack("C*").to_s if lastLiteral.length>0
        literalPack+=lastLiteral
        lastLiteral=[]
        matchOffset=offset-token[0]
        matchPack=[matchOffset]
        matchLength=token[1]
       # puts "match "+x[offset,token[1]].unpack("C*").to_s
        while matchLength>=19
          matchPack.push((matchLength-19 >= 255) ? 255 : matchLength-19)
          break if matchLength-19<255
          matchLength-=255
        end
        matchPack=matchPack.pack("vC*")
        ret+=literalPack+matchPack
        offset+=token[1]
      end
    end
    while offset<x.length
        lastLiteral.push(x[offset,1])
        offset+=1
    end    
    if lastLiteral.length>0
        lastLiteral=lastLiteral.join("")
        tokenByte=(lastLiteral.length>=15) ? 0xF0 : (lastLiteral.length<<4)
        literalPack=[tokenByte]
        litLength=lastLiteral.length
        while litLength>=15
          literalPack.push((litLength-15 >= 255) ? 255 : litLength-15)
          break if litLength-15<255
          litLength-=255
        end
        literalPack=literalPack.pack("C*")
        literalPack+=lastLiteral
        ret+=literalPack
    end
    return ret
  end
end

#
#   Simple implementation in Ruby of Unicode normalization.
#

module Normalizer
  @@decomposeToSelf={}
  @@decompositions={}
  @@decomposeToSelfCompat={}
  @@decompositionsCompat={}
  @@foundInDecompMapping=nil
  @@maxAffectedCodePoint=nil
  def self.normalize(chars,form)
    return chars if chars.length==1 && @@maxAffectedCodePoint && chars[0]>@@maxAffectedCodePoint
    return chars if chars.length==1 && (chars[0]>=0xF0000 && chars[0]<=0x10FFFF)
    chars=decompose(chars,form)
    return chars if chars.length<=1
    chars=chars.clone
    chars=reorder(chars)
    chars=compose(chars,form)
    return chars
  end
  def self.isStable(ch,form)
    return true if @@maxAffectedCodePoint && ch>@@maxAffectedCodePoint
    return true if ch>=0xf0000 || (ch>=0xd800 && ch<=0xdfff)
    if ch>=0xac00 && ch<0xac00+11172
      # Special case for Hangul syllables
      return false if form==:NFD || form==:NFKD
    else
      return false if UnicodeDatabase.getCombiningClass(ch)!=0
      result=normalize([ch],form)
      return false if result.length!=1 || result[0]!=ch
    end
    return true if form==:NFD || form==:NFKD
    if !$DecompMappings[ch] && (ch<0xac00 || ch>=0xac00+11172)
      return true if @@foundInDecompMapping && !@@foundInDecompMapping[ch]
      # NOTE: No Hangul syllables occur in decomposition
      # mappings
      for k in $DecompMappings
        compat=$DecompTypes[k[0]]
        next if form==:NFC && compat!=nil && compat.length>0
        return false if k[1].include?(ch)
      end
      if !@@foundInDecompMapping
        @@foundInDecompMapping={}
        for k in $DecompMappings
          for m in k[1]
           @@foundInDecompMapping[m]=true
          end
        end
      end
      return true
    end
   if (ch>=0xac00 && ch<0xac00+11172)
      sIndex=ch-0xAC00;
      trail = 0x11A7 + sIndex % 28;
      if trail!=0x11A7
        return true # This is an LVT Hangul syllable
      else
         # This is an LV Hangul syllable; this is not stable since
         # a T jamo may follow it
        return false
      end
   end
   if !@@maxAffectedCodePoint
      @@maxAffectedCodePoint=0
      for k in $DecompMappings
        next if !k[1]
        @@maxAffectedCodePoint=[k[0],@@maxAffectedCodePoint].concat(k[1]).max
      end
#      p @@maxAffectedCodePoint
   end
#   p [ch,form]
   return false
  end
  def self.decomposeChar(ch,form)
    if(ch>=0xAC00 && ch<0xAC00+11172)
      # Hangul syllable
      sIndex=ch-0xAC00;
      trail = 0x11A7 + sIndex % 28;
      ret=[0x1100 + sIndex / 588, 0x1161 + (sIndex % 588) / 28]
      if(trail!=0x11A7)
        ret.push(trail)
      end
      return ret
    else
      compat=(form==:NFKC || form==:NFKD)
      if compat
        dec=@@decompositionsCompat[ch]
        return dec if dec
        dec=@@decomposeToSelfCompat[ch]
        return [ch] if dec
      else
        dec=@@decompositions[ch]
        return dec if dec
        dec=@@decomposeToSelf[ch]
        return [ch] if dec
      end
      decomp=UnicodeDatabase.getDecomp(ch,compat)
      ret=[]
      if decomp.length==1 && decomp[0]==ch
        ret=decomp
        (compat ? @@decomposeToSelfCompat : @@decomposeToSelf)[ch]=ret
      else
        for d in decomp
         ret.concat(decomposeChar(d,form))
        end
        (compat ? @@decompositionsCompat : @@decompositions)[ch]=ret
      end
      return ret
    end
  end
  
  def self.decompose(buffer,form)
    return decomposeChar(buffer[0],form) if buffer.length==1
    ret=[]
    for c in buffer
      ret.concat(decomposeChar(c,form))
    end
    return ret
  end
  def self.compose(buffer,form)
    return buffer if form==:NFD || form==:NFKD || buffer.length<2
    starterPos = 0;
    retval = buffer.length;
    starterCh = buffer[0];
    lastClass = UnicodeDatabase.getCombiningClass(starterCh);
    if (lastClass != 0)
      lastClass = 256;
    end
    compPos=0;
    endPos=0+buffer.length;
    composed=false;
    for decompPos in compPos...endPos
      ch = buffer[decompPos];
      chClass = UnicodeDatabase.getCombiningClass(ch);
      if(decompPos>compPos)
        lead = starterCh-0x1100;
        if(0 <= lead && lead < 19)
          # Found Hangul L jamo
          vowel = ch-0x1161;
          if(0<=vowel && vowel < 21 && (lastClass < chClass || lastClass == 0))
            starterCh = 0xAC00 + (lead * 21 + vowel) * 28;
            buffer[starterPos] = starterCh;
            buffer[decompPos] = 0x110000;
            composed=true;
            retval-=1;
            next;
          end
        end
        syllable = starterCh - 0xAC00;
        if (0 <= syllable && syllable < 11172 &&
            (syllable % 28) == 0) 
          # Found Hangul LV jamo
          trail = ch-0x11A7;
          if (0 < trail && trail < 28 && (lastClass < chClass || lastClass == 0))
            starterCh +=trail;
            buffer[starterPos] = starterCh;
            buffer[decompPos] = 0x110000;
            composed=true;
            retval-=1;
            next;
          end
        end
      end
      composite=UnicodeDatabase.getComposedPair(starterCh,ch);
      diffClass=lastClass < chClass;
      if(composite>=0 && (diffClass || lastClass == 0)) 
        buffer[starterPos]=composite;
        starterCh = composite;
        buffer[decompPos] = 0x110000;
        composed=true;
        retval-=1;
        next;
      end
      if (chClass == 0)
        starterPos = decompPos;
        starterCh  = ch;
      end
      lastClass = chClass;
    end
    if(composed)
      j=compPos;
      for i in compPos...endPos
        if(buffer[i]!=0x110000)
          buffer[j]=buffer[i];
          j+=1
        end
      end
    end
    return buffer[0,retval];    
  end
  def self.reorder(buffer)
    changed=true
    index=0
    while changed
      changed=false;
      lead=UnicodeDatabase.getCombiningClass(buffer[index]);
      trail=0;
      for i in 1...buffer.length
        offset=index+i;
        trail=UnicodeDatabase.getCombiningClass(buffer[offset]);
        if(trail!=0 && lead > trail)
          c=buffer[offset-1];
          buffer[offset-1]=buffer[offset];
          buffer[offset]=c;
          changed=true;
          # Lead is now at trail's position
        else
          lead=trail;
        end
      end
    end
    return buffer
  end
end

def getCodePoints(ln)
  lns=ln.split(" ")
  for i in 0...lns.length
    lns[i]=lns[i].gsub(/^\s+|\s+$/,"").to_i(16)
  end
  return lns
end

def checkEq(a,b,msg)
  raise [msg,a,b].to_s if a!=b
end


def doNormTest(file)
  ret=[]
  part1=false
  assigned=[]
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^@/]
        part1=(ln=="@Part1")
        next
      end
      next if ln.length==0
      origln=ln
      ln=ln.split(";")
      ln[0]=getCodePoints(ln[0])
      if part1
        assigned[ln[0][0]]=true
      end
      ln[1]=getCodePoints(ln[1]).join(",")
      ln[2]=getCodePoints(ln[2]).join(",")
      ln[3]=getCodePoints(ln[3]).join(",")
      ln[4]=getCodePoints(ln[4]).join(",")
      checkEq(ln[1],Normalizer.normalize(ln[0],:NFC).join(","),origln+" NFC")
      checkEq(ln[2],Normalizer.normalize(ln[0],:NFD).join(","),origln+" NFD")
      checkEq(ln[3],Normalizer.normalize(ln[0],:NFKC).join(","),origln+" NFKC")
      checkEq(ln[4],Normalizer.normalize(ln[0],:NFKD).join(","),origln+" NFKD")
    end
  }
  # Check unassigned and other code points
  for i in 0...0x110000
   next if i>=0xd800 && i<=0xdfff
   if !assigned[i]
     istring=i.to_s
     iarray=[i]
     checkEq(istring,Normalizer.normalize(iarray,:NFC).join(","),istring+" NFC")
     checkEq(istring,Normalizer.normalize(iarray,:NFD).join(","),istring+" NFD")
     checkEq(istring,Normalizer.normalize(iarray,:NFKC).join(","),istring+" NFKC")
     checkEq(istring,Normalizer.normalize(iarray,:NFKD).join(","),istring+" NFKD")
   end
  end
  return assigned
end

def testByteArray(newarr,oldarr)
  for cp in 0...0x110000
    idx=(cp>>9)<<1
    x=newarr[idx+1]
    if ((x&0x80)!=0)
      raise "#{cp}: #{oldarr[cp]||0}" if newarr[idx]!=(oldarr[cp]||0)
    else
      x=(x<<8)|(newarr[idx]&0xff)
      idx=0x1100+(x<<9)+(cp&511)
      raise "#{cp}: #{oldarr[cp]||0}" if newarr[idx]!=(oldarr[cp]||0)
    end
  end
end

def toByteArray(bools,step=512)
    header=[] # size 0x880
    arrays=[]
    index=0;
    i=0;while i<0x110000
      different=false;
      data=[]
      lastValue=-1;
      for j in i...i+step
        thisbool=(bools[j]||0);
        if(j!=i && thisbool!=lastValue)
          different=true;
        end
        lastValue=thisbool;
        data[j-i]=thisbool;
      end
      raise "size not right" if data.length!=step
      if(different)
        ptr=arrays.length
        for k in 0...ptr
          if data.join(",")==arrays[k].join(",")
            ptr=k;
            break;
          end
        end
        header[index]=ptr;
        if(ptr==arrays.length)
          arrays.push(data);
        end
      else
          header[index]=(0x8000|lastValue)&0xFFFF;
      end
      i+=step;
      index+=1
    end
    pack=header.pack("v*")
    for arr in arrays
      pack+=arr.pack("C*")
    end
    return pack.unpack("C*")
end

def toBoolArray(bools)
    header=[] # size 0x880
    arrays=[]
    step=8192
    index=0;
    i=0;while i<0x110000
      different=false;
      data=[]
      lastValue=nil;
      for j in i...i+step
        thisbool=(bools[j] ? true : false);
        if(j!=i && thisbool!=lastValue)
          different=true;
        end
        lastValue=thisbool;
        data[j-i]=thisbool;
      end
      raise "size not right" if data.length!=step
      if(different)
        ptr=arrays.length
        for k in 0...ptr
          if data.join(",")==arrays[k].join(",")
            ptr=k;
            break;
          end
        end
        header[index]=ptr;
        if(ptr==arrays.length)
          arrays.push(data);
        end
      elsif lastValue==false
         header[index]=0xFE;
      elsif lastValue==true
         header[index]=0xFF;
      end
      i+=step;
      index+=1
    end
    pack=header.pack("C*")
    for arr in arrays
      arrpack=[]
      i=0;while i<arr.length
        val=0
        val|=1 if arr[i]
        val|=2 if arr[i+1]
        val|=4 if arr[i+2]
        val|=8 if arr[i+3]
        val|=16 if arr[i+4]
        val|=32 if arr[i+5]
        val|=64 if arr[i+6]
        val|=128 if arr[i+7]
        arrpack.push(val)
        i+=8
      end
      pack+=arrpack.pack("C*")
    end
    return pack.unpack("C*")
end
def linebrokenjoinbytes(arr)
  return linebrokenjoin(arr.unpack("C*").transform{|x| x>=128 ? sprintf("(byte)0x%2x",x) : x})
end

def linebrokenjoin(arr)
 data=arr.join(", ") 
 data=data.gsub(/(.{76}[^,\s]*),\s*/){ "#{$1},\n      " }
 data=data.gsub(/,[ \t]\r?\n/,",\n")
 return data
end

Dir.mkdir("cache") rescue nil
puts "Gathering Unicode data..."
downloadIfNeeded("cache/DerivedNormalizationProps.txt",
 "http://www.unicode.org/Public/UCD/latest/ucd/DerivedNormalizationProps.txt")
compex=getCompEx("cache/DerivedNormalizationProps.txt")
downloadIfNeeded("cache/UnicodeData.txt",
 "http://www.unicode.org/Public/UCD/latest/ucd/UnicodeData.txt")
downloadIfNeeded("cache/NormalizationTest.txt",
 "http://www.unicode.org/Public/UCD/latest/ucd/NormalizationTest.txt")
udata=getUnicodeData("cache/UnicodeData.txt")
$CombiningClasses=udata[0]
$DecompTypes=udata[1]
$DecompMappings=udata[2]
$ComposedPairs=generateComposites($DecompTypes,$DecompMappings,compex)
#puts "Testing normalization..."
#assigned=doNormTest("cache/NormalizationTest.txt")
puts "Generating data files..."
File.open("Text/NormalizationData.cs","wb"){|f|
f.puts("/* Generated by unicodedata.rb from data from the Unicode Character Database (UCD).")
f.puts(" The UCD's copyright owner is Unicode, Inc.")
f.puts(" Licensed under the Unicode License ")
f.puts(" (see http://www.unicode.org/copyright.html Exhibit 1). */")
final="final"
if true
f.puts("namespace PeterO.Text {")
f.puts("  internal class NormalizationData {")
final="readonly"
else
f.puts("package com.upokescenter.internal;")
f.puts("class NormalizationData {")  
end
binary=[]
for key in $ComposedPairs.keys.sort
  a=key/0x110000
  b=key%0x110000
  val=$ComposedPairs[key]
  next if val>=0xAC00 && val<0xAC00+11172
  binary.push(a)
  binary.push(b)
  binary.push(val)
end
f.puts("    public static #{final} int[] ComposedPairs = new int[] {")
f.puts("      "+linebrokenjoin(binary))
f.puts("    };");
f.puts("    public static #{final} byte[] CombiningClasses = new byte[] {")
# Generate a packed byte array for combining classes
cc=toByteArray($CombiningClasses)
# Then compress the array with the LZ4 algorithm
cc=LZ4.compress(cc)
f.puts("      "+linebrokenjoinbytes(cc))
f.puts("    };");
pointers=[]
decomps=[]
compatdecomps=[]
for i in 0..0xEFFFF # iterate to 0xEFFFF because the remaining characters are private use
  next if val>=0xAC00 && val<0xAC00+11172 # Skip Hangul syllables
  decomp=$DecompMappings[i]
  next if !decomp || decomp.length==0 || (decomp.length==1 && decomp[0]==i)
  compat=$DecompTypes[i] && $DecompTypes[i].length>0
  pointer=i
  pointers.push(pointer)
  if decomp.length==1
   # Singleton decomposition
   pointer=decomp[0]
   pointer|=(1<<22)
   # Compatibility decomposition
   pointer|=(1<<23) if compat
   pointers.push(pointer)
   decomp=[]
  else
   pointer=(decomp.length<<24) | (compat ? compatdecomps.length : decomps.length)
   # Compatibility decomposition
   pointer|=(1<<23) if compat
   pointers.push(pointer)
  end
  if compat
    for d in decomp
      compatdecomps.push(d)
    end
  else
    for d in decomp
      decomps.push(d)
    end    
  end
end
binary=[pointers.length/2]
binary.concat(pointers)
binary.concat(decomps)
f.puts("    public static #{final} int[] DecompMappings = GetDecompMappings();")
f.puts("    private static int[] GetDecompMappings(){")
f.puts("      return new int[] {")
data=binary.transform{|x| (x>>31)!=0 ? "unchecked((int)#{x})" : x.to_s }
data=linebrokenjoin(data)
f.puts("      "+data)
f.puts("      };");
f.puts("    }");
f.puts("    public static #{final} int[] CompatDecompMappings = GetCompatDecompMappings();")
f.puts("    private static int[] GetCompatDecompMappings(){")
f.puts("      return new int[] {")
data=compatdecomps.transform{|x| (x>>31)!=0 ? "unchecked((int)#{x})" : x.to_s }
data=linebrokenjoin(data)
f.puts("      "+data)
f.puts("      };");
f.puts("    }");
stablenfc=[]
for i in 0...0x110000; stablenfc.push(Normalizer.isStable(i,:NFC)); end
f.puts("    public static #{final} byte[] StableNFC = new byte[] {")
f.puts("      "+linebrokenjoinbytes(LZ4.compress(toBoolArray(stablenfc))))
f.puts("    };")
stablenfc=[]
for i in 0...0x110000; stablenfc.push(Normalizer.isStable(i,:NFD)); end
f.puts("    public static #{final} byte[] StableNFD = new byte[] {")
f.puts("      "+linebrokenjoinbytes(LZ4.compress(toBoolArray(stablenfc))))
f.puts("    };")
stablenfc=[]
for i in 0...0x110000; stablenfc.push(Normalizer.isStable(i,:NFKC)); end
f.puts("    public static #{final} byte[] StableNFKC = new byte[] {")
f.puts("      "+linebrokenjoinbytes(LZ4.compress(toBoolArray(stablenfc))))
f.puts("    };")
stablenfc=[]
for i in 0...0x110000; stablenfc.push(Normalizer.isStable(i,:NFKD)); end
f.puts("    public static #{final} byte[] StableNFKD = new byte[] {")
f.puts("      "+linebrokenjoinbytes(LZ4.compress(toBoolArray(stablenfc))))
f.puts("    };")
f.puts("  }")
if true
f.puts("}")
end
}
puts "Done"
