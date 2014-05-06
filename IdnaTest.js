(function(){

/*
Written in 2014 by Peter O.
Any copyright is dedicated to the Public Domain.
http://creativecommons.org/publicdomain/zero/1.0/
If you like this, you should donate to Peter O.
at: http://upokecenter.com/d/
 */

var idn=require("./Idna.js");
var Normalizer=idn.Normalizer;
var Normalization=idn.Normalization;
var Idna=idn.Idna;
var DomainUtility=idn.DomainUtility;

function stringToCodeUnits(a){
 var builder=[]
 for(var i=0;i<a.length;i++){
  builder.push(a.charCodeAt(i).toString(16))
 }
 return builder.join(" ")
}
function codePointToString(c){
  if(c<=0xffff){
   return (String.fromCharCode(c))
  } else {
   return (String.fromCharCode(((((c - 65536) >> 10) & 1023) + 55296)))+
     (String.fromCharCode(((c - 65536) & 1023) + 56320))
  }
}
function codePointsToString(codepoints){
 codepoints=codepoints.split(" ")
 var builder=[]
 for(var i=0;i<codepoints.length;i++){
  var c=parseInt(codepoints[i],16)
  if(c<=0xffff){
   builder.push(String.fromCharCode(c))
  } else {
   builder.push(String.fromCharCode(((((c - 65536) >> 10) & 1023) + 55296)))
   builder.push(String.fromCharCode(((c - 65536) & 1023) + 56320))
  }
 }
 return builder.join("")
}

// Normalization test

var fs=require("fs"), sys=require("sys")
var codePoints=[]
if(fs.existsSync("./cache/NormalizationTest.txt")){
 var file=fs.readFileSync("./cache/NormalizationTest.txt",'utf-8');
 var lines=file.split(/\r?\n/g)
 var part1=false
 for(var i=0;i<lines.length;i++){
   lines[i]=lines[i].replace(/\s*\#.*$/,"")
   var linedata=lines[i].split(/\s*;\s*/)
   if(lines[i]=="@Part1")part1=true
   else if(lines[i].indexOf("@Part")>=0)part1=false
   if(linedata.length<5)continue
   var codePoint=part1 ? parseInt(linedata[0],16) : 0
   linedata[0]=codePointsToString(linedata[0])
   linedata[1]=codePointsToString(linedata[1])
   linedata[2]=codePointsToString(linedata[2])
   linedata[3]=codePointsToString(linedata[3])
   linedata[4]=codePointsToString(linedata[4])
   var actual;
   actual=Normalizer.Normalize(linedata[0],Normalization.NFC);
   if(actual!=linedata[1]){
    throw lines[i]+", NFC,\n expected "+stringToCodeUnits(linedata[1])+", got "+stringToCodeUnits(actual)
   }
   actual=Normalizer.Normalize(linedata[0],Normalization.NFD);
   if(actual!=linedata[2]){
     throw lines[i]+", NFD,\n expected "+stringToCodeUnits(linedata[2])+", got "+stringToCodeUnits(actual)
   }
   actual=Normalizer.Normalize(linedata[0],Normalization.NFKC);
   if(actual!=linedata[3]){
     throw lines[i]+", NFKC,\n expected "+stringToCodeUnits(linedata[3])+", got "+stringToCodeUnits(actual)
   }
   actual=Normalizer.Normalize(linedata[0],Normalization.NFKD);
   if(actual!=linedata[4]){
     throw lines[i]+", NFKD,\n expected "+stringToCodeUnits(linedata[4])+", got "+stringToCodeUnits(actual)
   }
   if(part1)codePoints[codePoint]=true
 }
 for(var i=0;i<0x110000;i++){
   if(i>=0xd800 && i<=0xdfff)continue;
   if(codePoints[i])continue;
   var cps=codePointToString(i)
   var actual;
   actual=Normalizer.Normalize(cps,Normalization.NFC);
   if(actual!=cps){
    throw i.toString(16)+", NFC,\n expected "+stringToCodeUnits(cps)+", got "+stringToCodeUnits(actual)
   }
   actual=Normalizer.Normalize(cps,Normalization.NFD);
   if(actual!=cps){
     throw i.toString(16)+", NFD,\n expected "+stringToCodeUnits(cps)+", got "+stringToCodeUnits(actual)
   }
   actual=Normalizer.Normalize(cps,Normalization.NFKC);
   if(actual!=cps){
     throw i.toString(16)+", NFKC,\n expected "+stringToCodeUnits(cps)+", got "+stringToCodeUnits(actual)
   }
   actual=Normalizer.Normalize(cps,Normalization.NFKD);
   if(actual!=cps){
     throw i.toString(16)+", NFKD,\n expected "+stringToCodeUnits(cps)+", got "+stringToCodeUnits(actual)
   }  
 }
}


function assertTrue(cond){
  if(!cond)throw new Error("Expected true, got false");
}
function assertFalse(cond){
  if(cond)throw new Error("Expected false, got true");
}
function assertEqual(a, b){
  if(a!=b)throw new Error("Expected "+a+", got "+b);
}

// IDNA tests

    // Tests that all-ASCII strings remain unchanged by
    // PunycodeEncode.
 function testAllAsciiLabels() {
      var tmp;
      tmp="ascii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="-ascii-1-";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="-ascii-1";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="ascii-1-";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="1ascii-1";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="2ascii-1-";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="as.cii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="as&cii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="as`cii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="\rascii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="\nascii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
      tmp="\u007fascii";
      assertEqual(tmp, DomainUtility.PunycodeEncode(tmp));
    }

   function idnaTest() {
      assertTrue(Idna.IsValidDomainName("el\u00b7la",false));
      assertFalse(Idna.IsValidDomainName("-domain",false));
      assertFalse(Idna.IsValidDomainName("domain-",false));
      // Label starting with digit is valid since there are no RTL labels
      assertTrue(Idna.IsValidDomainName("1domain.example",false));
      // Label starting with digit is not valid since there are RTL labels
      assertFalse(Idna.IsValidDomainName("1domain.\u05d0\u05d0",false));
      assertFalse(Idna.IsValidDomainName("\u05d0\u05d0.1domain.example",false));
      assertFalse(Idna.IsValidDomainName("el\u00b7",false));
      assertFalse(Idna.IsValidDomainName("el\u00b7ma",false));
      assertFalse(Idna.IsValidDomainName("em\u00b7la",false));
      // 0x300 is the combining grave accent
      assertFalse(Idna.IsValidDomainName("\u0300xyz",false));
      assertTrue(Idna.IsValidDomainName("x\u0300yz",false));
      // Has white space
      assertFalse(Idna.IsValidDomainName("x\u0300y z",false));
      // 0x323 is dot below, with a lower combining
      // class than grave accent
      assertTrue(Idna.IsValidDomainName("x\u0323\u0300yz",false));
      // Not in NFC, due to the reordered combining marks
      assertFalse(Idna.IsValidDomainName("x\u0300\u0323yz",false));
      // 0xffbf is unassigned as of Unicode 6.3
      assertFalse(Idna.IsValidDomainName("x\uffbfyz",false));
      // 0xffff is a noncharacter
      assertFalse(Idna.IsValidDomainName("x\uffffyz",false));
      // 0x3042 is hiragana A, 0x30a2 is katakana A,
      // and 0x5000 is a Han character
      assertFalse(Idna.IsValidDomainName("xy\u30fb",false));
      assertTrue(Idna.IsValidDomainName("xy\u3042\u30fb",false));
      assertTrue(Idna.IsValidDomainName("xy\u30a2\u30fb",false));
      assertTrue(Idna.IsValidDomainName("xy\u5000\u30fb",false));
      // ZWJ preceded by virama
      assertTrue(Idna.IsValidDomainName("xy\u094d\u200dz",false));
      assertFalse(Idna.IsValidDomainName("xy\u200dz",false));
      assertFalse(Idna.IsValidDomainName("\ua840\u0300\u0300\u200d\u0300\u0300\ua840",false));
      // ZWNJ preceded by virama
      assertTrue(Idna.IsValidDomainName("xy\u094d\u200cz",false));
      assertFalse(Idna.IsValidDomainName("xy\u200cz",false));
      // Dual-joining character (U + A840, Phags-pa KA) on both sides
      assertTrue(Idna.IsValidDomainName("\ua840\u200c\ua840",false));
      // Dual-joining character with intervening T-joining characters
      assertTrue(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\ua840",false));
      assertTrue(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\u0300\u0300\ua840",false));
      // Left-joining character (U + A872, the only such character
      // in Unicode 6.3, with Bidi type L) on left side
      assertTrue(Idna.IsValidDomainName("\ua872\u200c\ua840",false));
      assertTrue(Idna.IsValidDomainName("\ua872\u0300\u0300\u200c\u0300\u0300\ua840",false));
      // Left-joining character on right side
      assertFalse(Idna.IsValidDomainName("\ua840\u200c\ua872",false));
      assertFalse(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\u0300\u0300\ua872",false));
      // Nonjoining character on right side
      assertFalse(Idna.IsValidDomainName("\ua840\u200cx",false));
      assertFalse(Idna.IsValidDomainName("\ua840\u0300\u0300\u200c\u0300\u0300x",false));
      // Nonjoining character on left side
      assertFalse(Idna.IsValidDomainName("x\u200c\ua840",false));
      assertFalse(Idna.IsValidDomainName("x\u0300\u0300\u200c\u0300\u0300\ua840",false));
      // Keraia
      assertTrue(Idna.IsValidDomainName("x\u0375\u03b1",false));  // Greek
      assertFalse(Idna.IsValidDomainName("x\u0375a",false));  // Non-Greek
      // Geresh and gershayim
      assertTrue(Idna.IsValidDomainName("\u05d0\u05f3",false));  // Hebrew
      assertFalse(Idna.IsValidDomainName("\u0627\u05f3",false));  // Arabic (non-Hebrew)
      assertTrue(Idna.IsValidDomainName("\u05d0\u05f4",false));  // Hebrew
      assertFalse(Idna.IsValidDomainName("\u0627\u05f4",false));  // Arabic (non-Hebrew)
      // Bidi Rule: Hebrew and Latin in the same label
      assertFalse(Idna.IsValidDomainName("a\u05d0",false));  // Hebrew
      assertFalse(Idna.IsValidDomainName("\u05d0a",false));  // Hebrew
      // Arabic-indic digits and extended Arabic-indic digits
      assertFalse(Idna.IsValidDomainName("\u0627\u0660\u06f0\u0627",false));
      // Right-joining character (U + 062F; since the only right-joining characters in
      // Unicode have Bidi type R,
      // a different dual-joining character is used, U + 062D, which also has
      // the same Bidi type).
      assertTrue(Idna.IsValidDomainName("\u062d\u200c\u062f",false));
      assertTrue(Idna.IsValidDomainName("\u062d\u0300\u0300\u200c\u0300\u0300\u062f",false));
      // Right-joining character on left side
      assertFalse(Idna.IsValidDomainName("\u062f\u200c\u062d",false));
      assertFalse(Idna.IsValidDomainName("\u062f\u0300\u0300\u200c\u0300\u0300\u062d",false));
    }
  
  testAllAsciiLabels();
  idnaTest();

})();