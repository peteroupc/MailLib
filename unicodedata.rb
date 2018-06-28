#!/usr/bin/ruby
# Written by Peter O. in 2014.
# Any copyright is dedicated to the Public Domain.
# http://creativecommons.org/publicdomain/zero/1.0/
#
# If you like this, you should donate to Peter O.
# at: http://peteroupc.github.io/

Dir.chdir(File.dirname(__FILE__))

require 'net/http'

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
def getProp(file,name)
  ret={}
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^([A-Fa-f0-9]+)\.\.([A-Fa-f0-9]+)\s*;\s*#{name}\b/]
        a=$1.to_i(16)
        b=$2.to_i(16)
        for i in a..b
         ret[i]=true
        end
      elsif ln[/^([A-Fa-f0-9]+)\s*;\s*#{name}\b/]
        a=$1.to_i(16)
        ret[a]=true
      end
    end
  }
  return ret
end

def getJoiningTypes(file)
  ret={}
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^([A-Fa-f0-9]+)\s*;\s*[^;]*;\s*([^;]+)/]
        a=$1.to_i(16)
        ret[a]=$2
      end
    end
  }
  return ret
end

def getQCNoOrMaybe(file,name)
  ret={}
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^([A-Fa-f0-9]+)\.\.([A-Fa-f0-9]+)\s*;\s*#{name}\s*;\s*(N|M)\b/]
        #p ln
        a=$1.to_i(16)
        b=$2.to_i(16)
        for i in a..b
         ret[i]=true
        end
      elsif ln[/^([A-Fa-f0-9]+)\s*;\s*#{name}\s*;\s*(N|M)\b/]
        #p ln
        a=$1.to_i(16)
        ret[a]=true
      end
    end
  }
  return ret
end

def getMutexProp(file)
  # Reads a file where all the properties are
  # mutually exclusive
  ret={}
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^([A-Fa-f0-9]+)\.\.([A-Fa-f0-9]+)\s*;\s*([A-Za-z0-9_]+)\b/]
        a=$1.to_i(16)
        b=$2.to_i(16)
        value=$3
        for i in a..b
         ret[i]=value
        end
      elsif ln[/^([A-Fa-f0-9]+)\s*;\s*([A-Za-z0-9_]+)\b/]
        a=$1.to_i(16)
        value=$2
        ret[a]=value
      end
    end
  }
  return ret
end

def getCompEx(file)
  return getProp(file,"Full_Composition_Exclusion")
end
def getUnicodeData(file)
  ccc=[]
  decompType=[]
  gencats=[]
  decompMapping={}
  names=[]
  File.open(file,"rb"){|f|
   while !f.eof?
    ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
    ln=ln.split(";")
    # Get the general category
    gencat=ln[2]
    name=ln[1]
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
      gencats[i]=gencat
      names[i]=name
      decompMapping[i]=mapping if mapping
    end
   end
  }
  return [ccc,decompType,decompMapping,gencats,names]
end

def generateComposites(dtypes,dmappings,compex)
  ret={}
  for i in 0...0x110000
    decomp=dmappings[i]
    dtype=dtypes[i]
    iscompat=(dtype && dtype.length>0)
    if decomp && decomp.length==2 && !iscompat &&
        !compex[i]
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
 def self.getGeneralCategory(ch)
   return $GeneralCategories[ch]||"Cn"
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
        literalPack+=lastLiteral
        lastLiteral=[]
        matchOffset=offset-token[0]
        matchPack=[matchOffset]
        matchLength=token[1]
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
  @@canonDecompMappings=nil
  @@compatDecompMappings=nil
  @@foundInDecompMapping=nil
  @@maxAffectedCodePoint=nil
  @@quickChecks={}
  @@quickCheckNames={
   :NFD=>"NFD_QC",
   :NFC=>"NFC_QC",
   :NFKC=>"NFKC_QC",
   :NFKD=>"NFKD_QC"
  }
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
  def self.isQCS(ch,form)
    return true if ch>=0xf0000 || (ch>=0xd800 && ch<=0xdfff)
    return true if @@maxAffectedCodePoint && ch>@@maxAffectedCodePoint
    if !@@quickChecks[form]
      @@quickChecks[form]=getQCNoOrMaybe(
        "cache/DerivedNormalizationProps.txt",@@quickCheckNames[form])
    end
    if !@@quickChecks[form][ch] && UnicodeDatabase.getCombiningClass(ch) != 0
      #p [ch,form,"not QCS",UnicodeDatabase.getCombiningClass(ch)]
    end
    return !@@quickChecks[form][ch] && UnicodeDatabase.getCombiningClass(ch) == 0
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
    starter = buffer[0];
    last = UnicodeDatabase.getCombiningClass(starter);
    if (last != 0)
      last = 256;
    end
    compPos=0;
    endPos=0+buffer.length;
    composed=false;
    for decompPos in compPos...endPos
      ch = buffer[decompPos];
      cc = UnicodeDatabase.getCombiningClass(ch);
      if(decompPos>compPos)
        lead = starter-0x1100;
        if(0 <= lead && lead < 19)
          # Found Hangul L jamo
          vowel = ch-0x1161;
          if(0<=vowel && vowel < 21 && (last < cc || last == 0))
            starter = 0xAC00 + (lead * 21 + vowel) * 28;
            buffer[starterPos] = starter;
            buffer[decompPos] = 0x110000;
            composed=true;
            retval-=1;
            next;
          end
        end
        syllable = starter - 0xAC00;
        if (0 <= syllable && syllable < 11172 &&
            (syllable % 28) == 0)
          # Found Hangul LV jamo
          trail = ch-0x11A7;
          if (0 < trail && trail < 28 && (last < cc || last == 0))
            starter +=trail;
            buffer[starterPos] = starter;
            buffer[decompPos] = 0x110000;
            composed=true;
            retval-=1;
            next;
          end
        end
      end
      composite=UnicodeDatabase.getComposedPair(starter,ch);
      diffClass=last < cc;
      if(composite>=0 && (diffClass || last == 0))
        buffer[starterPos]=composite;
        starter = composite;
        buffer[decompPos] = 0x110000;
        composed=true;
        retval-=1;
        next;
      end
      if (cc == 0)
        starterPos = decompPos;
        starter  = ch;
      end
      last = cc;
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
    return buffer if buffer.length<=1
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
  return linebrokenjoin(arr.unpack("C*").map{|x| x>=128 ? sprintf("(byte)0x%2x",x) : x})
end

def linebrokenjoin(arr)
 return linebrokenjoin(arr.unpack("C*")) if arr.is_a?(String)
 data=arr.join(", ")
 data=data.gsub(/(.{60}[^,\s]*),\s*/){ "#{$1},\n      " }
 data=data.gsub(/,[ \t]\r?\n/,",\n")
 return data
end

def caseFoldingCF(file)
  ret=[]
  File.open(file,"rb"){|f|
    while !f.eof?
      ln=f.gets.gsub(/^\s+|\s+$/,"").gsub(/\s*\#.*$/,"")
      if ln[/^([A-Fa-f0-9]+)\s*;\s*[CF]\s*;\s*([^;]+)\b/]
        a=$1.to_i(16)
        fold=$2
        fold=fold.gsub(/\s+$/,"")
        fold=fold.split(/\s+/).map{|x| x.to_i(16) }
        ret[a]=fold
      end
    end
  }
  return ret
end

def toCaseFold(chars,casefold)
  return chars if chars.length==1 && !casefold[chars[0]]
  ret=[]
  for c in chars
    cf=casefold[c] || [c]
    ret.concat(cf)
  end
  return ret
end
def jsbytes(name, arr)
  ret="#{name} = [\n";
  ret+=linebrokenjoin(arr)+"\n"
  ret+="];\n"
end
def getChunkedFunctions(arr, name, chunkSize)
  j=0
  ret=""
  i=0; while i<arr.length
    ret+=("    private static int[] #{name}#{j}() {\n")
    ret+=("      return new int[] {\n")
    ret+=("        "+linebrokenjoin(arr[i,chunkSize]))+"\n"
    ret+=("      };\n");
    ret+=("    }\n");
    i+=chunkSize
    j+=1
  end
  ret+=("    private static int[] #{name}() {\n")
  ret+=("      int[] ret = new int[#{arr.length}];\n")
  i=0; j=0; while i<arr.length
    alen=[chunkSize,arr.length-i].min
    ret+=("      Array.Copy(#{name}#{j}(), 0, ret, #{i}, #{alen});\n")
    i+=chunkSize
    j+=1
  end
  ret+=("      return ret;\n")
  ret+=("    }\n");
  return ret
end
def getQCSArray(form)
 qcs=[]
 minval=-1;maxval=-1
 for i in 0...0x110000
  qcs.push(Normalizer.isQCS(i,form));
  minval=i if minval==-1 && !qcs[i]
  maxval=i if !qcs[i]
 end
 return [qcs,minval,maxval]
end
def makeQCS(f,fjs,form)
  namestr=form.to_s
  qcs=getQCSArray(form)
  puts "Finding #{namestr} quick-check starters..."
  data=("    public static readonly byte[] QCS#{namestr} = new byte[] {\n")
  bin=LZ4.compress(toBoolArray(qcs[0]))
  data+=("      "+linebrokenjoinbytes(bin))
  data+=("    };\n")
  fjs.puts(jsbytes("NormalizationData['QCS#{namestr}']",bin))
  return [qcs[1],qcs[2],data]
end

Dir.mkdir("cache") rescue nil
puts "Gathering Unicode data..."
downloadIfNeeded("cache/DerivedNormalizationProps.txt",
 "http://www.unicode.org/Public/UNIDATA/DerivedNormalizationProps.txt")
compex=getCompEx("cache/DerivedNormalizationProps.txt")
downloadIfNeeded("cache/UnicodeData.txt",
 "http://www.unicode.org/Public/UNIDATA/UnicodeData.txt")
downloadIfNeeded("cache/ArabicShaping.txt",
 "http://www.unicode.org/Public/UNIDATA/ArabicShaping.txt")
downloadIfNeeded("cache/Scripts.txt",
 "http://www.unicode.org/Public/UNIDATA/Scripts.txt")
downloadIfNeeded("cache/DerivedBidiClass.txt",
 "http://www.unicode.org/Public/UNIDATA/extracted/DerivedBidiClass.txt")
downloadIfNeeded("cache/HangulSyllableType.txt",
 "http://www.unicode.org/Public/UNIDATA/HangulSyllableType.txt")
downloadIfNeeded("cache/DerivedCoreProperties.txt",
 "http://www.unicode.org/Public/UNIDATA/DerivedCoreProperties.txt")
downloadIfNeeded("cache/NormalizationTest.txt",
 "http://www.unicode.org/Public/UNIDATA/NormalizationTest.txt")
downloadIfNeeded("cache/PropList.txt",
 "http://www.unicode.org/Public/UNIDATA/PropList.txt")
downloadIfNeeded("cache/CaseFolding.txt",
 "http://www.unicode.org/Public/UNIDATA/CaseFolding.txt")
udata=getUnicodeData("cache/UnicodeData.txt")
$CombiningClasses=udata[0]
$DecompTypes=udata[1]
$DecompMappings=udata[2]
$GeneralCategories=udata[3]
$CaseFolding=caseFoldingCF("cache/CaseFolding.txt")
$ComposedPairs=generateComposites($DecompTypes,$DecompMappings,compex)
$CharacterNames=udata[4]
puts "Testing normalization..."
#doNormTest("cache/NormalizationTest.txt")
puts "Generating data files..."
fileHeader=("/* Generated by unicodedata.rb from data from the Unicode Character Database.\n")
fileHeader+=(" Licensed under the Unicode License\n")
fileHeader+=(" (see LICENSE.md in the source code root or visit\n")
fileHeader+=(" http://www.unicode.org/copyright.html Exhibit 1). */\n")
File.open("NormalizationData.js","wb"){|fjs|
File.open("MailLib/PeterO/Text/NormalizationData.cs","wb"){|f|
f.puts(fileHeader)
fjs.puts(fileHeader)
fjs.puts("var NormalizationData = {};\n")
final="final"
f.puts("using System;")
f.puts("namespace PeterO.Text {")
f.puts("  internal static class NormalizationData {")
f.puts("    public static readonly string Notice = \"The NormalizationData class was \" +\n"+
"\"generated by unicodedata.rb from data from the Unicode Character Database. \" +\n"+
"\"The NormalizationData class is licensed under the Unicode License \" +\n"+
"\"(see LICENSE.md in the source code root or visit \" +\n"+
"\"http://www.unicode.org/copyright.html Exhibit 1).\";")
final="readonly"
#-------------------

qcs1=makeQCS(f,fjs,:NFC);
qcs2=makeQCS(f,fjs,:NFD);
qcs3=makeQCS(f,fjs,:NFKC);
qcs4=makeQCS(f,fjs,:NFKD);
f.puts("    public const int QCSNFCMin = #{qcs1[0]};")
f.puts("    public const int QCSNFCMax = #{qcs1[1]};")
f.puts("    public const int QCSNFDMin = #{qcs2[0]};")
f.puts("    public const int QCSNFDMax = #{qcs2[1]};")
f.puts("    public const int QCSNFKCMin = #{qcs3[0]};")
f.puts("    public const int QCSNFKCMax = #{qcs3[1]};")
f.puts("    public const int QCSNFKDMin = #{qcs4[0]};")
f.puts("    public const int QCSNFKDMax = #{qcs4[1]};")
f.puts(qcs1[2])
f.puts(qcs2[2])
f.puts(qcs3[2])
f.puts(qcs4[2])

#-------------------
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

f.puts("    public static #{final} int[] ComposedPairs = GetComposedPairs();")
binbytes=binary.pack("V*").unpack("C*")
puts "ComposedPairs bytes: #{binbytes.length}"
binbytes=LZ4.compress(binbytes)
puts "ComposedPairs lz4bytes: #{binbytes.length}"
f.puts(getChunkedFunctions(binary, "GetComposedPairs", 4000))
fjs.puts("NormalizationData.ComposedPairs = [")
fjs.puts(linebrokenjoin(binary))
fjs.puts("]; if(typeof Uint32Array!='undefined')NormalizationData.ComposedPairs = new Uint32Array(NormalizationData.ComposedPairs);")
f.puts("    public static #{final} byte[] CombiningClasses = new byte[] {")
f.puts("      "+linebrokenjoinbytes(LZ4.compress(toByteArray($CombiningClasses))))
f.puts("    };");
fjs.puts("NormalizationData.CombiningClasses = [")
fjs.puts("      "+linebrokenjoin(LZ4.compress(toByteArray($CombiningClasses))))
fjs.puts("]; if(typeof Uint8Array!='undefined')NormalizationData.CombiningClasses = new Uint32Array(NormalizationData.CombiningClasses);")
pointers=[]
complexdecomps=[]
complexcp=0
paircp=0
for i in 0..0xEFFFF # iterate to 0xEFFFF because the remaining characters are private use
  next if val>=0xAC00 && val<0xAC00+11172 # Skip Hangul syllables
  decomp=$DecompMappings[i]
  next if !decomp || decomp.length==0 || (decomp.length==1 && decomp[0]==i)
  compat=$DecompTypes[i] && $DecompTypes[i].length>0
  pointer=i
  if decomp.length==1
   pointer|=(1<<22) # Singleton decomposition
   # Compatibility decomposition
   pointer|=(1<<23) if compat
   pointers.push(pointer)
   pointers.push(decomp[0])
   next
  elsif decomp.length==2 && decomp[0]<0x10000 && decomp[1]<0x10000
   paircp+=1
   pointer|=(1<<24) # Pair of BMP code points
   # Compatibility decomposition
   pointer|=(1<<23) if compat
   pointers.push(pointer)
   cp=decomp[0]|(decomp[1]<<16)
   pointers.push(cp)
   next
  else
   complexcp+=1
   # Compatibility decomposition
   pointer|=(1<<23) if compat
   pointers.push(pointer)
   pointer=complexdecomps.length|(decomp.length<<24)
   pointers.push(pointer)
   for d in decomp
    complexdecomps.push(d)
   end
  end
end
binary=[]
binary.concat(pointers)
f.puts("    public static #{final} int[] DecompMappings = GetDecompMappings();")
binbytes=binary.pack("V*").unpack("C*")
puts "ComposedPairs bytes: #{binbytes.length}"
binbytes=LZ4.compress(binbytes)
puts "ComposedPairs lz4bytes: #{binbytes.length}"
puts "CompatPointers: #{pointers.length}"
puts "ComplexCP: #{complexcp}"
puts "ComplexDecomps: #{complexdecomps.length}"
puts "PairCP: #{paircp}"
data=binary.map{|x| (x>>31)!=0 ? "unchecked((int)#{x})" : x.to_s }
f.puts(getChunkedFunctions(data, "GetDecompMappings", 4000))
f.puts("    public static #{final} int[] ComplexDecompMappings = GetComplexDecompMappings();")
f.puts("    private static int[] GetComplexDecompMappings() {")
f.puts("      return new int[] {")
data=complexdecomps.map{|x| (x>>31)!=0 ? "unchecked((int)#{x})" : x.to_s }
data=linebrokenjoin(data)
f.puts("      "+data)
f.puts("      };");
f.puts("    }");
fjs.puts("NormalizationData.DecompMappings = [")
fjs.puts(linebrokenjoin(binary))
fjs.puts("]; if(typeof Uint32Array!='undefined')NormalizationData.DecompMappings = new Uint32Array(NormalizationData.DecompMappings);")
# TODO: Update NormJS to handle changed normalization data format
fjs.puts("NormalizationData.ComplexDecompMappings = [")
fjs.puts(linebrokenjoin(complexdecomps))
fjs.puts("]; if(typeof Uint32Array!='undefined')NormalizationData.ComplexDecompMappings = new Uint32Array(NormalizationData.ComplexDecompMappings);")
f.puts("  }")
if true
f.puts("}")
end
fjs.puts("if(typeof exports!=='undefined')exports['NormalizationData']=NormalizationData;")
fjs.puts("if(typeof window!=='undefined')window['NormalizationData']=NormalizationData;")
}}
puts "Generating IDNA data..."
letterDigits=[]
idnaCategories=[]
combiningMarks=[]
defaultIgnore=getProp("cache/DerivedCoreProperties.txt",
  "Default_Ignorable_Code_Point")
whiteSpace=getProp("cache/PropList.txt",
  "White_Space")
noncharacterCP=getProp("cache/PropList.txt","Noncharacter_Code_Point")
joinControls=getProp("cache/PropList.txt","Join_Control")
hangulL=getProp("cache/HangulSyllableType.txt","L")
hangulV=getProp("cache/HangulSyllableType.txt","V")
hangulT=getProp("cache/HangulSyllableType.txt","T")
categories=%w( Ll Lu Lo Lm Nd Mn Mc )
for i in 0...0x110000
 cat=UnicodeDatabase.getGeneralCategory(i)
 if cat.include?("M")
   combiningMarks[i]=true
 end
 if [0xdf,0x3c2,0x6fd,0x6fe,0xf0b,0x3007].include?(i)
   idnaCategories[i]=1 # PVALID
   next
 end
 if [0xb7,0x375,0x5f3,0x5f4,0x30fb].include?(i) ||
      (i>=0x660 && i<=0x669) || (i>=0x6f0 && i<=0x6f9)
   idnaCategories[i]=4; next # CONTEXTO
   next
 end
 if [0x640,0x7f,0x302e,0x302f,0x3031,0x3032,0x3033,0x3034,
       0x3035,0x303b].include?(i)
   idnaCategories[i]=2; next # DISALLOWED
 end
 # Unassigned
 if cat=="Cn" && !noncharacterCP[i]
   idnaCategories[i]=0; next # UNASSIGNED
   next
 end
 # LDH
 if i==0x2d || (i>=0x30 && i<=0x39) || (i>=0x61 && i<=0x7a)
   idnaCategories[i]=1; next # PVALID
 end
 # Join Controls
 if joinControls[i]
   idnaCategories[i]=3; next # CONTEXTJ
 end
 # Ignorable properties
 if defaultIgnore[i] || whiteSpace[i] || noncharacterCP[i]
   idnaCategories[i]=2; next # DISALLOWED
 end
 # Ignorable blocks
 if (i>=0x20d0 && i<=0x20ff) || (i>=0x1d100 && i<=0x1d24f)
   idnaCategories[i]=2; next # DISALLOWED
 end
 # Hangul jamo
 if hangulL[i] || hangulV[i] || hangulT[i]
   idnaCategories[i]=2; next # DISALLOWED
 end
 # Unstable
 temp=Normalizer.normalize([i],:NFKC)
 temp=toCaseFold(temp,$CaseFolding)
 temp=Normalizer.normalize(temp,:NFKC)
 if temp.length!=1 || temp[0]!=i
   idnaCategories[i]=2; next # DISALLOWED
 end
 if categories.include?(cat)
   idnaCategories[i]=1; next # PVALID
 end
 idnaCategories[i]=2; next # DISALLOWED
end

File.open("IdnaData.js","wb"){|fjs|
File.open("MailLib/PeterO/Text/IdnaData.cs","wb"){|f|
f.puts(fileHeader)
fjs.puts(fileHeader)
f.puts("// Character data required for IDNA2008 (RFC 5890-5894)")
f.puts("namespace PeterO.Text {")
f.puts("  internal static class IdnaData {")
f.puts("    public static readonly string Notice = \"The IdnaData class was \" +\n"+
"\"generated by unicodedata.rb from data from the Unicode Character Database. \" +\n"+
"\"The IdnaData class is licensed under the Unicode License \" +\n"+
"\"(see LICENSE.md in the source code root or visit \" +\n"+
"\"http://www.unicode.org/copyright.html Exhibit 1).\";")
final="readonly"
bidi=getMutexProp("cache/DerivedBidiClass.txt")
bidivalues=%w( L R AL EN ES ET AN CS NSM BN ON B S WS
  LRE LRO RLE RLO PDF LRI RLI FSI PDI )
bidivalueshash={}
for i in 0...bidivalues.length; bidivalueshash[bidivalues[i]]=i; end
for b in bidi.keys; bidi[b]=bidivalueshash[bidi[b]]; end
scripts=getMutexProp("cache/Scripts.txt")
idnaScripts={}
for b in scripts.keys
  value=scripts[b]
  s=0
  s=1 if value=="Greek"
  s=2 if value=="Hebrew"
  s=3 if value=="Han" || value=="Katakana" || value=="Hiragana"
  idnaScripts[b]=s
end
joining=getJoiningTypes("cache/ArabicShaping.txt")
for b in joining.keys
 value=joining[b]
 s=0
 s=1 if value=="T"
 s=2 if value=="R"
 s=3 if value=="L"
 s=4 if value=="D"
 s=5 if value=="C"
 joining[b]=s
end
for i in 0...0xF0000
 gc=UnicodeDatabase.getGeneralCategory(i)
 if gc=="Mn" || gc=="Cf" || gc=="Me"
   joining[i]=1 if !joining[i]
 end
end
fjs.puts("var IdnaData = {};")
f.puts("    public static #{final} byte[] IdnaCategories = new byte[] {")
bin=LZ4.compress(toByteArray(idnaCategories))
f.puts("      "+linebrokenjoinbytes(bin))
f.puts("    };")
fjs.puts(jsbytes("IdnaData['IdnaCategories']",bin))
f.puts("    // 0=Other; 1=Greek; 2=Hebrew; 3=Han/Hiragana/Katakana")
f.puts("    public static #{final} byte[] IdnaRelevantScripts = new byte[] {")
bin=LZ4.compress(toByteArray(idnaScripts))
f.puts("      "+linebrokenjoinbytes(bin))
f.puts("    };")
fjs.puts(jsbytes("IdnaData['IdnaRelevantScripts']",bin))
f.puts("    public static #{final} byte[] BidiClasses = new byte[] {")
bin=LZ4.compress(toByteArray(bidi))
f.puts("      "+linebrokenjoinbytes(bin))
f.puts("    };")
fjs.puts(jsbytes("IdnaData['BidiClasses']",bin))
f.puts("    public static #{final} byte[] JoiningTypes = new byte[] {")
bin=LZ4.compress(toByteArray(joining))
f.puts("      "+linebrokenjoinbytes(bin))
f.puts("    };")
fjs.puts(jsbytes("IdnaData['JoiningTypes']",bin))
f.puts("    public static #{final} byte[] CombiningMarks = new byte[] {")
bin=LZ4.compress(toBoolArray(combiningMarks))
f.puts("      "+linebrokenjoinbytes(bin))
f.puts("    };")
fjs.puts(jsbytes("IdnaData['CombiningMarks']",bin))
f.puts("  }")
f.puts("}")
fjs.puts("if(typeof exports!=='undefined')exports['IdnaData']=IdnaData;")
fjs.puts("if(typeof window!=='undefined')window['IdnaData']=IdnaData;")
}}
puts "Done"
