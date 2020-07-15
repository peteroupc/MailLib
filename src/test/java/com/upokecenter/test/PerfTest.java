package com.upokecenter.test;
import java.util.*;
import java.io.*;
import org.junit.Assert;
import org.junit.Test;
import com.upokecenter.mail.*;
import com.upokecenter.util.*;
import java.math.*;

public class PerfTest {

public String ReadAllText(String fn) throws IOException {
  FileInputStream fs=new FileInputStream(new File(fn));
  BufferedReader br=new BufferedReader(new InputStreamReader(fs));
  StringBuilder sb=new StringBuilder();
  int c=0;
  while ((c=br.read()) >= 0) {
    sb.append((char)c);
  }
  br.close();
  return sb.toString();
}

public void WaitForProfiler() throws IOException {
  File f=new File("/tmp/profiling.dat");
  FileOutputStream fs=new FileOutputStream(f);
  fs.close();
  while(f.exists()) {
    try {
     Thread.sleep(50);
    } catch(InterruptedException ex) {}
  }
}

@Test
public void TestPerf4() throws IOException {
  File[] files=new File("/home/rooster/Documents/SharpDevelopProjects/MailLib/MailLibTest/bin/Debug/mails")
    .listFiles();
  for(int i=0;i<4;i++){
    if(i==1) WaitForProfiler();
    int c=0;
    for(File f:files) {
     if(f.toString().indexOf(".eml")<0)break;
     c++;
try {
InputStream s=new BufferedInputStream(new FileInputStream(f));
Message msg=new Message(s);
msg.GetFormattedBodyString();
msg.Generate();
} catch(MessageDataException ex){
} catch(UnsupportedOperationException ex){
} catch(IOException ex){
}
    }
  }
 }
}
