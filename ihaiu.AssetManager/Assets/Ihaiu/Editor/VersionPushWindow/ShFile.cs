using UnityEngine;
using System.Collections;
using System.IO;

public class ShFile
{
    private static ShFile _tmp;
    public static ShFile tmp
    {
        get
        {
            if (_tmp == null)
            {
                _tmp = new ShFile(Shell.sh_vertmp);;
            }

            return _tmp;
        }
    }

    public string       path;
    public StringWriter sw = new StringWriter();

    public ShFile()
    {
        path = Shell.sh_tmp;
        Init();
    }

    public ShFile(string file)
    {
        path = file;
        Init();
    }


	public void Init(bool isExpect = false)
	{
		if(isExpect)
		{
			sw.WriteLine("#!/usr/bin/expect  -f   ");
		}
		else
		{
			sw.WriteLine("#!/bin/bash  ");
		}

        sw.WriteLine("");
    }

	public void Clear(bool isExpect = false)
    {
        sw = new StringWriter();
		Init(isExpect);
    }

    public void Read()
    {
        sw = new StringWriter();
        sw.Write(File.ReadAllText(path));
    }

    public void Save()
    {
        File.WriteAllText(path, sw.ToString());
    }

	public void WriteLine(string line, bool logShowCmd = false)
	{
		sw.WriteLine("");
//        sw.WriteLine("echo \"--------\"");
//		if(logShowCmd)
//		{
//			sw.WriteLine("echo \"$ " + line.Replace("\"", "\\\"") + "\" >> " + Shell.txt_vertmp);
//		}
//		else
//		{
//			sw.WriteLine("echo \"$ " + line.Replace("\"", "\\\"") + "\"");
//		}
        sw.WriteLine(line);
    }

	public void WriteLinePassword(string line, string password)
	{
		sw.WriteLine("");
		sw.WriteLine("spawn " + line);
		sw.WriteLine("expect \"Password:\"");
		sw.WriteLine(string.Format("send \"{0}\\r\"", password));
		sw.WriteLine("interact");

	}

	public void WriteLine(string line, bool needPassword, string password)
	{
		if(!needPassword)
		{
			WriteLine(line);
		}
		else
		{
			WriteLinePassword(line, password);
		}
	}

}

