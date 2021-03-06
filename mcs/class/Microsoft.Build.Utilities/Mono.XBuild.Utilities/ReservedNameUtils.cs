//
// ReservedNameUtils.cs
//
// Author:
//   Marek Sieradzki (marek.sieradzki@gmail.com)
//
// (C) 2006 Marek Sieradzki
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if NET_2_0

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace Mono.XBuild.Utilities {

	internal static class ReservedNameUtils {
	
		static string[] reservedMetadataNames;
		static Hashtable reservedMetadataHash;
	
		static ReservedNameUtils ()
		{
			reservedMetadataNames = new string [] {
				"FullPath", "RootDir", "Filename", "Extension", "RelativeDir", "Directory",
				"RecursiveDir", "Identity", "ModifiedTime", "CreatedTime", "AccessedTime"};
			reservedMetadataHash = CollectionsUtil.CreateCaseInsensitiveHashtable (ReservedMetadataNameCount);
			foreach (string s in reservedMetadataNames) {
				reservedMetadataHash.Add (s, null);
			}
		}
		
		public static ICollection ReservedMetadataNames {
			get {
				return (ICollection) reservedMetadataNames.Clone ();
			}
		}

		public static int ReservedMetadataNameCount {
			get {
				return reservedMetadataNames.Length;
			}
		}

		public static bool IsReservedMetadataName (string metadataName)
		{
			return reservedMetadataHash.Contains (metadataName);
		}

		public static string GetReservedMetadata (string itemSpec,
						   string metadataName, IDictionary metadata)
		{
			if (metadataName == null)
				throw new ArgumentNullException ();
		
			switch (metadataName.ToLower ()) {
			case "fullpath":
				return Path.GetFullPath (itemSpec);
			case "rootdir":
				if (Path.IsPathRooted (itemSpec))
					return Path.GetPathRoot (itemSpec);
				else
					return Path.GetPathRoot (Environment.CurrentDirectory);
			case "filename":
				return Path.GetFileNameWithoutExtension (itemSpec);
			case "extension":
				return Path.GetExtension (itemSpec);
			case "relativedir":
				return WithTrailingSlash (Path.GetDirectoryName (itemSpec));
			case "directory":
				string fullpath = Path.GetFullPath (itemSpec);
				return WithTrailingSlash (
					 Path.GetDirectoryName (fullpath).Substring (Path.GetPathRoot (fullpath).Length));
			case "recursivedir":
				if (metadata != null && metadata.Contains ("RecursiveDir"))
					return (string)metadata ["RecursiveDir"];
				else
					return String.Empty;
			case "identity":
				return Path.Combine (Path.GetDirectoryName (itemSpec), Path.GetFileName (itemSpec));
			case "modifiedtime":
				if (File.Exists (itemSpec))
					return File.GetLastWriteTime (itemSpec).ToString ();
				else if (Directory.Exists (itemSpec))
					return Directory.GetLastWriteTime (itemSpec).ToString ();
				else
					return String.Empty;
			case "createdtime":
				if (File.Exists (itemSpec))
					return File.GetCreationTime (itemSpec).ToString ();
				else if (Directory.Exists (itemSpec))
					return Directory.GetCreationTime (itemSpec).ToString ();
				else
					return String.Empty;
			case "accessedtime":
				if (File.Exists (itemSpec))
					return File.GetLastAccessTime (itemSpec).ToString ();
				else if (Directory.Exists (itemSpec))
					return Directory.GetLastAccessTime (itemSpec).ToString ();
				else
					return String.Empty;
			default:
				throw new ArgumentException ("Invalid reserved metadata name");
			}
		}

		static string WithTrailingSlash (string path)
		{
			if (path.Length > 0)
				return path + Path.DirectorySeparatorChar;
			else
				return path;
		}
	}
}

#endif
