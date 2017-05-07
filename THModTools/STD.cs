using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace THModTools
{
	public static class STD
	{
		static Dictionary<int, STDInst[]> Inst = new Dictionary<int, STDInst[]>()
		{
			{
				8,
				new STDInst[]
				{
					new STDInst {ID = 0, Name = "setCameraPos", Size = 12, Args = "fff"},
					new STDInst {ID = 1, Name = "setFog", Size = 12, Args = "cff"},
					new STDInst {ID = 2, Name = "setCameraTarget", Size = 12, Args = "fff"},
					new STDInst {ID = 9, Name = "setCameraForward", Size = 12, Args = "fff"},
				}
			},
			{
				15,
				new STDInst[]
				{
					new STDInst {ID = 1, Name = "jump", Size = 16, Args = "lU"},
					new STDInst {ID = 2, Name = "setCameraPos", Size = 20, Args = "fff"},
					new STDInst {ID = 3, Name = "moveCamera", Size = 28, Args = "UUfff"},
					new STDInst {ID = 4, Name = "setCameraTarget", Size = 20, Args = "fff"},
					new STDInst {ID = 5, Name = "moveCameraTarget", Size = 28, Args = "UUfff"},
					new STDInst {ID = 6, Name = "setCameraForward", Size = 20, Args = "fff"},
					new STDInst {ID = 7, Name = "setCameraFOV", Size = 12, Args = "f"},
					new STDInst {ID = 8, Name = "setFog", Size = 20, Args = "cff"},
					new STDInst {ID = 9, Name = "changeFog", Size = 28, Args = "UUcff"},
					new STDInst {ID = 12, Name = "ins_12", Size = 12, Args = "bbbb"},
					new STDInst {ID = 13, Name = "ins_13", Size = 12, Args = "c"},
					new STDInst {ID = 14, Name = "ins_14", Size = 20, Args = "bbbbbbbbbbbb"},
					new STDInst {ID = 16, Name = "ins_16", Size = 12, Args = "c"},
					new STDInst {ID = 18, Name = "ins_18", Size = 28, Args = "bbbbbbbbbbbbbbbbbbbb"},
				}
			}
		};

		static BinaryReader Reader;
		static List<string> Output;
		static List<long> OutputOffset;
		static long ScriptOffset = 0;
		public static string[] Read(string file)
		{
			List<string> output = new List<string>();
			Output = output;
			OutputOffset = new List<long>();
			//int facesOffset;
			//int scriptOffset;
			using (var reader = new BinaryReader(File.OpenRead(file)))
			{
				Reader = reader;
				var objects = (int)reader.ReadUInt16();
				/*var faces = (int)*/reader.ReadUInt16();
				//output.Add("Objects: " + objects);
				//output.Add("Quads: " + faces);

				/*facesOffset =*/ reader.ReadInt32();
				/*scriptOffset =*/ reader.ReadInt32();
				reader.ReadUInt32();

				if (THTK.Version >= 10)
				{
					var anmNameArr = new byte[128];
					reader.Read(anmNameArr, 0, 128);
					var anmName = Encoding.ASCII.GetString(anmNameArr);
					anmName = anmName.Substring(0, anmName.IndexOf('\0'));
					OutputOffset.Add(-1);
					output.Add("ANM: " + anmName);
				}
				else
				{
					byte[] nameArr;
					string nameStr;

					nameArr = new byte[128];
					reader.Read(nameArr, 0, 128);
					nameStr = Encoding.UTF8.GetString(nameArr);
					nameStr = nameStr.Substring(0, nameStr.IndexOf('\0'));
					OutputOffset.Add(-1);
					output.Add("StageName: " + nameStr);

					for (var i = 1; i <= 4; i++)
					{
						nameArr = new byte[128];
						reader.Read(nameArr, 0, 128);
						nameStr = Encoding.UTF8.GetString(nameArr);
						nameStr = nameStr.Substring(0, nameStr.IndexOf('\0'));
						OutputOffset.Add(-1);
						output.Add("SongName" + i + ": " + nameStr);
					}

					for (var i = 1; i <= 4; i++)
					{
						nameArr = new byte[128];
						reader.Read(nameArr, 0, 128);
						nameStr = Encoding.UTF8.GetString(nameArr);
						nameStr = nameStr.Substring(0, nameStr.IndexOf('\0'));
						OutputOffset.Add(-1);
						output.Add("SongPath" + i + ": " + nameStr);
					}
				}

				var objectOffsets = new int[objects];
				for (var i = 0; i < objects; i++)
					objectOffsets[i] = reader.ReadInt32();

				OutputOffset.Add(-1);
				output.Add("");

				for (var i = 0; i < objects; i++)
				{
					OutputOffset.Add(-1);
					output.Add("");
					OutputOffset.Add(-1);
					output.Add("Object: obj_" + reader.ReadUInt16());
					OutputOffset.Add(-1);
					output.Add("Unknown: " + reader.ReadUInt16());
					OutputOffset.Add(-1);
					output.Add("Box Position: " + reader.ReadSingle() + " " +
						reader.ReadSingle() + " " + reader.ReadSingle());
					OutputOffset.Add(-1);
					output.Add("Box Size: " + reader.ReadSingle() + " " +
						reader.ReadSingle() + " " + reader.ReadSingle());
					OutputOffset.Add(-1);
					output.Add("");

					while (true)
					{
						
						var type = reader.ReadUInt16();
						var unknown2 = reader.ReadUInt16();
						if (type == 65535)
						{
							break;
						}
							
						OutputOffset.Add(-1);
						output.Add("Quad");
						OutputOffset.Add(-1);
						output.Add("Type: " + type);
						OutputOffset.Add(-1);
						output.Add("Unknown: " + unknown2);
						OutputOffset.Add(-1);
						output.Add("Script: " + reader.ReadUInt16());
						reader.ReadUInt16();

						OutputOffset.Add(-1);
						output.Add("Position: " + reader.ReadSingle() + " " +
							reader.ReadSingle() + " " + reader.ReadSingle());
						OutputOffset.Add(-1);
						output.Add("Size: " + reader.ReadSingle() + " " + reader.ReadSingle());
						OutputOffset.Add(-1);
						output.Add("");
						OutputOffset.Add(-1);
						output.Add("");
					}
				}

				while (true)
				{
					var id = reader.ReadUInt16();
					var unknown = reader.ReadUInt16();
					var x = reader.ReadSingle();
					var y = reader.ReadSingle();
					var z = reader.ReadSingle();

					if (id == 65535 && unknown == 65535)
						break;

					OutputOffset.Add(-1);
					output.Add("WorldObject");
					OutputOffset.Add(-1);
					output.Add("ObjectID: obj_" + id);
					OutputOffset.Add(-1);
					output.Add("Position: " + x + " " + y + " " + z);
					OutputOffset.Add(-1);
					output.Add("");
				}

				OutputOffset.Add(-1);
				output.Add("");
				OutputOffset.Add(-1);
				output.Add("");
				OutputOffset.Add(-1);
				output.Add("");
				OutputOffset.Add(-1);
				output.Add("Script");

				int oldTime = 0;
				ScriptOffset = reader.BaseStream.Position;
				while (true)
				{
					OutputOffset.Add(reader.BaseStream.Position - ScriptOffset);
					var time = (int)reader.ReadUInt32();
					if (time > oldTime && (uint)time != 0xffffffff)
					{
						OutputOffset.Add(reader.BaseStream.Position - ScriptOffset);
						oldTime = time;
						output.Add(time + ":");
					}
					var id = reader.ReadUInt16();
					var size = reader.ReadUInt16();

					if ((uint)time == 0xffffffff || id == 65535)
						break;

					var inst = InstByID(id);

					if (inst.ID != -1)
					{
						output.Add("    " + inst.Name + "(" + ArgString(inst, reader) + ")");
					}
					else
					{
						Console.WriteLine("Unknown ins " + id + " " + size + " bytes");
						var args = reader.ReadBytes((THTK.Version >= 10 ? size - 8 : size));
						output.Add("    ins_" + id + "(" + string.Join(", ", args) + ")");
					}
				}

				reader.Close();
			}

			//Debug info
			//Console.WriteLine(new FileInfo(file).Name + " contents:");
			//foreach (var line in output)
				//Console.WriteLine(line);
			//Console.WriteLine("");

			return output.ToArray();
		}

		static int Line = -1;
		static string[] Input;
		static String FPath;
		static String FCopy;
		static BinaryWriter Writer;
		static FileStream WriteFileStream;
		static MemoryStream HeaderStream;
		static MemoryStream ObjectStream;
		static MemoryStream ScriptStream;

		public static void Write(string file, string[] input)
		{
			if (File.Exists(file))
				File.Delete(file);
			WriteFileStream = File.OpenWrite(file);
			HeaderStream = new MemoryStream();
			ObjectStream = new MemoryStream();
			ScriptStream = new MemoryStream();
			var headerWriter = new BinaryWriter(HeaderStream);
			var objectWriter = new BinaryWriter(ObjectStream);
			var scriptWriter = new BinaryWriter(ScriptStream);
			FPath = file;
			FCopy = FPath + ".temp";
			if (File.Exists(FCopy))
				File.Delete(FCopy);
			File.Copy(FPath, FCopy);
			Line = 0;
			Input = input;
			var objectIDs = new Dictionary<string, int>();
			var objects = 0;
			var quads = 0;
			int quadsOffset;
			int scriptOffset;
			string found;
			int convI;
			float convF;
			byte convB;
			float[] convF3;
			float[] convF2;
			var objectOffsets = new Dictionary<int, int>();


			Writer = headerWriter;

			Writer.Write((ushort)0);
			Writer.Write((ushort)0);
			Writer.Write((uint)0);
			Writer.Write((uint)0);
			Writer.Write((uint)0);

			if (!Find("ANM:"))
				return;

			if (!FindValue(out found))
				return;
			Writer.Write(Encoding.ASCII.GetBytes(found));
			for (var i = 0; i < 128 - found.Length; i++)
				Writer.Write((byte)0);


			Writer = objectWriter;
			while (true)
			{
				if (!Find("Object:", false))
					break;
				if (!FindValue(out found))
					return;
				objectIDs.Add(found, objects);
				objectOffsets[objects] = (int)Writer.BaseStream.Position;
				Writer.Write((ushort)objects++);

				if (!Find("Unknown:"))
					return;
				if (!FindValue(out found))
					return;
				if (!ConvertInt(found, out convI))
					return;
				Writer.Write((ushort)convI);

				if (!Find("Box Position:"))
					return;
				if (!FindValue(out found))
					return;
				if (!ConvertFloat3(found, out convF3))
					return;
				Writer.Write(convF3[0]);
				Writer.Write(convF3[1]);
				Writer.Write(convF3[2]);

				if (!Find("Box Size:"))
					return;
				if (!FindValue(out found))
					return;
				if (!ConvertFloat3(found, out convF3))
					return;
				Writer.Write(convF3[0]);
				Writer.Write(convF3[1]);
				Writer.Write(convF3[2]);

				while (true)
				{
					if (!Find("Quad", false))
						break;

					Line++;
					quads++;
					if (!Find("Type:"))
						return;
					if (!FindValue(out found))
						return;
					if (!ConvertInt(found, out convI))
						return;
					Writer.Write((ushort)convI);

					if (!Find("Unknown:"))
						return;
					if (!FindValue(out found))
						return;
					if (!ConvertInt(found, out convI))
						return;
					Writer.Write((ushort)convI);

					if (!Find("Script:"))
						return;
					if (!FindValue(out found))
						return;
					if (!ConvertInt(found, out convI))
						return;
					Writer.Write((ushort)convI);

					Writer.Write((ushort)0);

					if (!Find("Position:"))
						return;
					if (!FindValue(out found))
						return;
					if (!ConvertFloat3(found, out convF3))
						return;
					Writer.Write(convF3[0]);
					Writer.Write(convF3[1]);
					Writer.Write(convF3[2]);

					if (!Find("Size:"))
						return;
					if (!FindValue(out found))
						return;
					if (!ConvertFloat2(found, out convF2))
						return;
					Writer.Write(convF2[0]);
					Writer.Write(convF2[1]);
				}

				Writer.Write((ushort)65535);
				Writer.Write((ushort)4);
			}

			quadsOffset = (int)Writer.BaseStream.Position;
			while (true)
			{
				if (!Find("WorldObject", false))
					break;

				Line++;
				if (!Find("ObjectID:"))
					return;
				if (!FindValue(out found))
					return;
				if (!objectIDs.ContainsKey(found))
				{
					Error("\"" + found + "\" does not refer to an object name at line " + Line);
					return;
				}
				Writer.Write((ushort)objectIDs[found]);
				Writer.Write((ushort)256);

				if (!Find("Position:"))
					return;
				if (!FindValue(out found))
					return;
				if (!ConvertFloat3(found, out convF3))
					return;
				Writer.Write(convF3[0]);
				Writer.Write(convF3[1]);
				Writer.Write(convF3[2]);
			}

			for (var i = 0; i < 16; i++)
				Writer.Write((byte)255);


			Writer = scriptWriter;
			var time = 0;

			if (!Find("Script"))
				return;

			Line++;
			scriptOffset = (int)Writer.BaseStream.Position;
			Dictionary<string, int> labels = new Dictionary<string, int>();
			while (Line < input.Length)
			{
				var line = input[Line].Trim();
				while (line.Contains("  "))
					line.Replace("  ", " ");

				if (line != "")
				{
					if (line.EndsWith(":"))
					{
						int newTime;
						var name = line.Remove(line.Length - 1);
						if (!int.TryParse(name, out newTime))
						{
							labels.Add(name, (int)Writer.BaseStream.Position);
						}
						else
							time = newTime;
					}
					else
					{
						if (line.Contains("("))
						{
							if (!line.Contains(")"))
							{
								Error("No matching \")\" for \"(\" at line " + Line);
								return;
							}

							var par1 = line.IndexOf('(');
							var par2 = line.IndexOf(')');

							var functionName = line.Substring(0, par1).TrimEnd();

							var inst = InstByName(functionName);
							if (inst.ID == -1)
							{
								Error("Instruction \"" + functionName + "\" unrecognized at line " + Line);
								return;
							}

							Writer.Write((uint)time);
							Writer.Write((ushort)inst.ID);
							Writer.Write((ushort)inst.Size);

							var args = line.Substring(par1 + 1, (par2 - par1) - 1).Replace(" ", "").Split(',');
							var argTemp = inst.Args;
							var argi = 0;
							foreach (var c in argTemp)
							{
								switch (c)
								{
									case ('l'):
										var lblname = args[argi++];
										if (!labels.ContainsKey(lblname))
										{
											Error("\"" + lblname + "\" not a label for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((uint)labels[lblname]);
										break;
									case ('b'):
										if (!byte.TryParse(args[argi++], out convB))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((byte)convB);
										break;
									case ('s'):
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((short)convI);
										break;
									case ('S'):
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write(convI);
										break;
									case ('u'):
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((ushort)convI);
										break;
									case ('U'):
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((uint)convI);
										break;
									case ('f'):
										if (!args[argi].Contains("."))
											args[argi] = args[argi].Replace("f", "") + ".0";
										if (!float.TryParse(args[argi++].Replace("f", ""), out convF))
										{
											Error("Expected \"float\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write(convF);
										break;
									case ('c'):
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((byte)convI);
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((byte)convI);
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((byte)convI);
										if (!int.TryParse(args[argi++], out convI))
										{
											Error("Expected \"int\" type for argument #" + argi + " at line " + Line);
											return;
										}
										Writer.Write((byte)convI);
										break;
								}
							}
						}
					}
				}

				Line++;
			}

			for (var i = 0; i < 20; i++)
				Writer.Write((byte)255);


			Writer = headerWriter;
			var offsetsStart = Writer.BaseStream.Length;
			Writer.BaseStream.Position = offsetsStart;
			for (var i = 0; i < objects; i++)
				Writer.Write((uint)0);

			var objectsOffset = Writer.BaseStream.Length;

			Writer.BaseStream.Position = offsetsStart;
			for (var i = 0; i < objects; i++)
				Writer.Write((uint)(objectsOffset + objectOffsets[i]));
			
			Writer.BaseStream.Position = 0;
			Writer.Write((ushort)objects);
			Writer.Write((ushort)quads);
			Writer.Write((uint)(objectsOffset + quadsOffset));
			Writer.Write((uint)(objectsOffset + ObjectStream.Length + scriptOffset));
			Writer.Write((uint)0);

			WriteFileStream.Position = 0;
			HeaderStream.WriteTo(WriteFileStream);
			ObjectStream.WriteTo(WriteFileStream);
			ScriptStream.WriteTo(WriteFileStream);

			WriteFileStream.Dispose();
			HeaderStream.Dispose();
			ObjectStream.Dispose();
			ScriptStream.Dispose();

			File.Delete(FCopy);
		}


		public static STDInst InstByID(int id)
		{
			foreach (var inst in Inst[THTK.Version])
			{
				if (inst.ID == id)
					return inst;
			}
			return new STDInst { ID = -1, Name = "" };
		}

		public static STDInst InstByName(string name)
		{
			foreach (var inst in Inst[THTK.Version])
			{
				if (inst.Name == name)
					return inst;
			}
			return new STDInst { ID = -1, Name = "" };
		}

		static string ArgString(STDInst inst, BinaryReader reader)
		{
			List<string> argList = new List<string>();
			var argTemplate = inst.Args;

			foreach (var c in argTemplate)
			{
				switch (c)
				{
					case ('l'):
						argList.Add(AddLabel(reader.ReadUInt32()));
						break;
					case ('b'):
						argList.Add(reader.ReadByte().ToString());
						break;
					case ('s'):
						argList.Add(reader.ReadInt16().ToString());
						break;
					case ('S'):
						argList.Add(reader.ReadInt32().ToString());
						break;
					case ('u'):
						argList.Add(reader.ReadUInt16().ToString());
						break;
					case ('U'):
						argList.Add(reader.ReadUInt32().ToString());
						break;
					case ('f'):
						argList.Add(reader.ReadSingle().ToString() + "f");
						break;
					case ('c'):
						argList.Add(reader.ReadByte().ToString());
						argList.Add(reader.ReadByte().ToString());
						argList.Add(reader.ReadByte().ToString());
						argList.Add(reader.ReadByte().ToString());
						break;
				}
			}

			return String.Join(", ", argList);
		}

		static string AddLabel(uint offset)
		{
			var lblName = "lbl_" + offset;
			for (var i = 0; i < Output.Count; i++)
			{
				if (OutputOffset[i] >= offset)
				{
					OutputOffset.Insert(i, OutputOffset[i]);
					Output.Insert(i, lblName + ":");
					return lblName;
				}
			}
			throw new Exception("Could not find offset " + offset + " for label!");
		}

		static bool Find(string expectedStart, bool error = true)
		{
			while (Input[Line].Trim() == "")
			{
				Line++;
			} 

			if (Input[Line].StartsWith(expectedStart))
				return true;
			if (error)
				Error("thstd: expected \"" + expectedStart + "\" at line " + Line);
			return false;
		}

		static void Error(string message)
		{
			Console.WriteLine(message);

			WriteFileStream.Dispose();
			HeaderStream.Dispose();
			ObjectStream.Dispose();
			ScriptStream.Dispose();

			File.Delete(FPath);
			File.Copy(FCopy, FPath);
			File.Delete(FCopy);
		}

		static bool FindValue(out string str)
		{
			var line = Input[Line];
			if (line.Contains(":"))
			{
				str = line.Substring(line.IndexOf(':') + 1).Trim();
				Input[Line] = "";
				return true;
			}
			Error("thstd: expected a value for property \"" + line + "\" at line " + Line);
			str = "";
			return false;
		}

		static bool ConvertInt(string str, out int result)
		{
			int ri;
			if (int.TryParse(str, out ri))
			{
				result = ri;
				return true;
			}
			Error("thstd: expected value of type \"int\" at line " + Line);
			result = 0;
			return false;
		}

		static bool ConvertFloat(string str, out float result)
		{
			if (str.EndsWith("f"))
				str = str.Remove(str.Length - 1);
			float rf;
			if (float.TryParse(str, out rf))
			{
				result = rf;
				return true;
			}
			Error("thstd: expected value of type \"float\" at line " + Line);
			result = 0f;
			return false;
		}

		static bool ConvertFloat3(string str, out float[] result)
		{
			result = new float[3];
			str = str.Trim();
			while (str.Contains("  "))
				str.Replace("  ", " ");

			string[] arr;
			if (!str.Contains(" "))
			{
				Error("thstd: expected three values of type \"float\" at line " + Line);
				result = new float[3];
				return false;
			}

			arr = str.Split(' ');
			if (arr.Length != 3)
			{
				Error("thstd: expected three values of type \"float\" at line " + Line);
				result = new float[arr.Length];
				return false;
			}

			var rf = new float[arr.Length];
			for (var i = 0; i < arr.Length; i++)
			{
				if (float.TryParse(arr[i], out rf[i]))
				{
					result[i] = rf[i];
				}
				else
				{
					Error("thstd: expected three values of type \"float\" at line " + Line);
					result = new float[arr.Length];
					return false;
				}
			}
			return true;
		}


		static bool ConvertFloat2(string str, out float[] result)
		{
			result = new float[2];
			str = str.Trim();
			while (str.Contains("  "))
				str.Replace("  ", " ");

			string[] arr;
			if (!str.Contains(" "))
			{
				Error("thstd: expected two values of type \"float\" at line " + Line);
				result = new float[2];
				return false;
			}

			arr = str.Split(' ');
			if (arr.Length != 2)
			{
				Error("thstd: expected two values of type \"float\" at line " + Line);
				result = new float[arr.Length];
				return false;
			}

			var rf = new float[arr.Length];
			for (var i = 0; i < arr.Length; i++)
			{
				if (float.TryParse(arr[i], out rf[i]))
				{
					result[i] = rf[i];
				}
				else
				{
					Error("thstd: expected two values of type \"float\" at line " + Line);
					result = new float[arr.Length];
					return false;
				}
			}
			return true;
		}
	}
}

