// Decompiled with JetBrains decompiler
// Type: Sword.UtilFile
// Assembly: Libs, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0D761999-E7FD-401B-9784-673A833539CF
// Assembly location: E:\_Proj\UnityProj\client3-clone\TheKingOfTower\Assets\Plugins\Libs.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Sword
{
  public static class UtilFile
  {
    public static void CheckDirExists(string dir)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(dir);
      if (directoryInfo.Exists)
        return;
      directoryInfo.Create();
    }

    public static void CheckDirExistsForFile(string file, string split = "/")
    {
      int length = file.LastIndexOf(split);
      string path = file.Substring(0, length);
      if (Directory.Exists(path))
        return;
      Directory.CreateDirectory(path);
    }

    public static string GetDirectoryName(string file)
    {
      return new FileInfo(file).Directory.Name;
    }

    public static string GetFileName(string file)
    {
      return Path.GetFileName(file);
    }

    public static string GetFileNameWithoutExtention(string file)
    {
      return Path.GetFileNameWithoutExtension(file);
    }

    public static string GetFileExtention(string file)
    {
      return Path.GetExtension(file);
    }

    public static string GetFileWithoutExtention(string file)
    {
      return file.Replace(UtilFile.GetFileExtention(file), "");
    }

    public static System.Type GetAssetTypeByExtension(string extension)
    {
      System.Type type = (System.Type) null;
      switch (extension)
      {
        case ".FBX":
          type = typeof (GameObject);
          break;
        case ".TGA":
          type = typeof (Texture2D);
          break;
        case ".anim":
          type = typeof (AnimationClip);
          break;
        case ".asset":
          type = typeof (UnityEngine.Object);
          break;
        case ".bmp":
          type = typeof (Texture2D);
          break;
        case ".bytes":
          type = typeof (TextAsset);
          break;
        case ".controller":
          type = typeof (Animator);
          break;
        case ".fbx":
          type = typeof (GameObject);
          break;
        case ".jpg":
          type = typeof (Texture2D);
          break;
        case ".lua":
          type = typeof (TextAsset);
          break;
        case ".mat":
          type = typeof (Material);
          break;
        case ".mp3":
          type = typeof (AudioClip);
          break;
        case ".ogg":
          type = typeof (AudioClip);
          break;
        case ".png":
          type = typeof (Texture2D);
          break;
        case ".prefab":
          type = typeof (GameObject);
          break;
        case ".shader":
          type = typeof (Shader);
          break;
        case ".tga":
          type = typeof (Texture2D);
          break;
        case ".ttf":
          type = typeof (Font);
          break;
        case ".txt":
          type = typeof (TextAsset);
          break;
        case ".wav":
          type = typeof (AudioClip);
          break;
        case ".xml":
          type = typeof (TextAsset);
          break;
      }
      return type;
    }

    public static string ReadText(string fileName)
    {
      return File.ReadAllText(fileName, Encoding.UTF8);
    }

    public static void AppendText(string fileName, string content)
    {
      File.AppendAllText(fileName, content, Encoding.UTF8);
    }

    public static void WriteText(string fileName, string content)
    {
      UtilFile.CheckDirExistsForFile(fileName, "/");
      File.WriteAllText(fileName, content, Encoding.UTF8);
    }

    public static void ClearDir(string targetDir)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(targetDir);
      if (!directoryInfo.Exists || (uint) directoryInfo.GetFiles("*.*").Length <= 0U)
        return;
      directoryInfo.Delete(true);
    }

    public static void EnsureDir(string targetDir)
    {
      if (Directory.Exists(targetDir))
        return;
      Directory.CreateDirectory(targetDir);
    }

    public static void CopyFileOrDir(string src, string dst)
    {
      if (Directory.Exists(src))
      {
        foreach (string directory in Directory.GetDirectories(src))
        {
          string dst1 = dst + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(directory);
          UtilFile.CopyFileOrDir(directory, dst1);
        }
        foreach (string file in Directory.GetFiles(src))
          UtilFile.CopyFileOrDir(file, dst);
      }
      else
      {
        if (!File.Exists(src))
          return;
        UtilFile.EnsureDir(dst);
        string destFileName = dst + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(src);
        File.Copy(src, destFileName);
      }
    }

    public static string ReadFile(string sFile)
    {
      using (FileStream fileStream = new FileStream(new FileInfo(sFile).FullName, FileMode.Open))
      {
        using (StreamReader streamReader = new StreamReader((Stream) fileStream))
          return streamReader.ReadToEnd();
      }
    }

    public static byte[] ReadFileContent(string file)
    {
      using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
      {
        byte[] buffer = new byte[fileStream.Length];
        fileStream.Read(buffer, 0, (int) fileStream.Length);
        fileStream.Close();
        return buffer;
      }
    }

    public static void WriteFileContent(string file, byte[] bytes)
    {
      UtilFile.CheckDirExistsForFile(file, "/");
      if (File.Exists(file))
        File.Delete(file);
      using (FileStream fileStream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
      {
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();
      }
    }

    public static byte[] ReadFileByByte(string file)
    {
      return File.ReadAllBytes(file);
    }

    public static void WriteFile(string text, string file)
    {
      UtilFile.CheckDirExistsForFile(file, "/");
      if (File.Exists(file))
        File.Delete(file);
      using (FileStream fileStream = new FileStream(new FileInfo(file).FullName, FileMode.OpenOrCreate))
      {
        fileStream.Seek(0L, SeekOrigin.Begin);
        fileStream.SetLength(0L);
        fileStream.Flush();
        using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.Default))
        {
          streamWriter.Write(text);
          streamWriter.Flush();
          streamWriter.Close();
        }
      }
    }

    public static void WriteFile(byte[] bytes, string file)
    {
      UtilFile.CheckDirExistsForFile(file, "/");
      if (File.Exists(file))
        File.Delete(file);
      File.WriteAllBytes(file, bytes);
    }

    public static bool IsExistFile(string file)
    {
      return File.Exists(file);
    }

    public static void CopyFile(string sourceFile, string dstFile)
    {
      if (!File.Exists(sourceFile))
        return;
      UtilFile.DeleteFile(dstFile);
      File.Copy(sourceFile, dstFile);
    }

    public static void DeleteFile(string file)
    {
      if (!File.Exists(file))
        return;
      File.Delete(file);
    }

    public static void DeleteDirectory(string dir)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(dir);
      if (!Directory.Exists(dir) || UtilFile.IsEmpty(dir))
        return;
      Directory.Delete(dir, true);
    }

    public static bool IsEmpty(string dir)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo(dir);
      return directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length == 0;
    }

    public static void MoveDirectory(string sourceDir, string dstDir)
    {
      if (Directory.Exists(dstDir))
        Directory.Delete(dstDir, true);
      Directory.Move(sourceDir, dstDir);
    }

    public static bool IsExistDirectory(string dir)
    {
      return Directory.Exists(dir);
    }

    public static byte[] StringToByte(string str)
    {
      return Encoding.UTF8.GetBytes(str);
    }

    public static string ByteToString(byte[] data)
    {
      return Encoding.UTF8.GetString(data);
    }

    public static double GetSize(string path)
    {
      return Math.Ceiling((double) new FileInfo(path).Length / 1024.0);
    }

    public static void CopyDirectory(string srcdir, string desdir, List<string> filterList)
    {
      string str1 = srcdir.Substring(srcdir.LastIndexOf("\\") + 1);
      string str2 = desdir + "\\" + str1;
      if (desdir.LastIndexOf("\\") == desdir.Length - 1)
        str2 = desdir + str1;
      foreach (string fileSystemEntry in Directory.GetFileSystemEntries(srcdir))
      {
        if (Directory.Exists(fileSystemEntry))
        {
          string path = str2 + "\\" + fileSystemEntry.Substring(fileSystemEntry.LastIndexOf("\\") + 1);
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
          UtilFile.CopyDirectory(fileSystemEntry, str2, filterList);
        }
        else
        {
          string str3 = fileSystemEntry.Substring(fileSystemEntry.LastIndexOf("\\") + 1);
          string destFileName = str2 + "\\" + str3;
          if (!Directory.Exists(str2))
            Directory.CreateDirectory(str2);
          if (UtilFile.IsVaildFilter(filterList, fileSystemEntry))
            File.Copy(fileSystemEntry, destFileName);
        }
      }
    }

    private static bool IsVaildFilter(List<string> filter, string path)
    {
      if (filter != null)
      {
        foreach (string str in filter)
        {
          if (path.Contains(str))
            return false;
        }
      }
      return true;
    }

    public static string ChangeLocalFile(string path)
    {
      return path.Replace("\\", "/");
    }

    public static string GetCurrentDirectoryName(string path)
    {
      string directoryName = Path.GetDirectoryName(path);
      return directoryName.Substring(directoryName.LastIndexOf('/') + 1);
    }
  }
}
