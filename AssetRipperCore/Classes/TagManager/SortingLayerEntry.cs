﻿using AssetRipper.Core.Project;
using AssetRipper.Core.Parser.Files;
using AssetRipper.Core.IO.Asset;
using AssetRipper.Core.YAML;

namespace AssetRipper.Core.Classes.TagManager
{
	public struct SortingLayerEntry : IAssetReadable, IYAMLExportable
	{
		/// <summary>
		/// Less than 5.0.0
		/// </summary>
		public static bool HasUserID(UnityVersion version) => version.IsLess(5);

		public void Read(AssetReader reader)
		{
			Name = reader.ReadString();
			if (HasUserID(reader.Version))
			{
				UserID = reader.ReadUInt32();
			}
			UniqueID = reader.ReadUInt32();
			reader.AlignStream();
		}

		public YAMLNode ExportYAML(IExportContainer container)
		{
			YAMLMappingNode node = new YAMLMappingNode();
			node.Add(NameName, Name);
			node.Add(UniqueIDName, UniqueID);
			node.Add(LockedName, false);
			return node;
		}

		public string Name { get; set; }
		public uint UserID { get; set; }
		public uint UniqueID { get; set; }

		public const string NameName = "name";
		public const string UniqueIDName = "uniqueID";
		public const string LockedName = "locked";
	}
}
