package com.upokecenter.test;

import java.io.*;
import java.net.*;
import java.util.*;
import org.junit.Assert;

public final class NetHelper {
  private NetHelper(){}
  public static String[] DownloadOrOpenAllLines(String location, String cachedPath){
    try {
     File file = new File(cachedPath);
     if(!file.exists()){
      URLConnection conn = new URL(location).openConnection();
      InputStream stream = null;
      OutputStream output = null;
      conn.connect();
      try { stream = conn.getInputStream();
       try { output = new FileOutputStream(file);
        byte[] buffer=new byte[8192];
        while(true){
         int count=stream.read(buffer,0,buffer.length);
         if(count<0) {
           break;
         }
         output.write(buffer,0,count);
        }
       } finally { output.close(); }
      } finally { stream.close(); }
     }
     ArrayList<String> list = new ArrayList<String>();

     LineNumberReader reader = null;
     try {
       reader = new LineNumberReader(new FileReader(file));
       while(true){
        String line=reader.readLine();
        if(line==null)break;
        list.add(line);
      }
    } finally {
      reader.close();
    }
     return list.toArray(new String[]{});
    } catch(IOException ex){
      Assert.fail(ex.getMessage());
      return null;
    }
  }
}
